using BepInEx.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CoyoteUnknown
{
    public class DGLabServer
    {
        private readonly ManualLogSource log;
        private TcpListener listener;
        private TcpClient connectedAppClient;
        private NetworkStream appStream;
        private CancellationTokenSource cts;

        public const int Port = 9999;
        public string ClientId { get; private set; } = "CoyoteUnknown";
        public string TargetId { get; private set; } = "";
        public bool IsBound { get; private set; } = false;

        private readonly List<string> webLogs = new List<string>();
        private readonly object logLock = new object();

        public DGLabServer(ManualLogSource logger)
        {
            this.log = logger;
        }

        public void AddWebLog(string message)
        {
            lock (logLock)
            {
                webLogs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                if (webLogs.Count > 15) webLogs.RemoveAt(0);
            }
        }

        private string GetWebLogsJson()
        {
            lock (logLock)
            {
                return JArray.FromObject(webLogs).ToString();
            }
        }

        public void Start()
        {
            try
            {
                Stop();
                cts = new CancellationTokenSource();
                listener = new TcpListener(IPAddress.Any, Port);
                listener.Start();
                log.LogInfo($"[DGLab-Server] 服务端已在端口 {Port} 启动！");
                AddWebLog($"本地服务端已在端口 {Port} 启动！");
                _ = Task.Run(() => AcceptClientsLoop(cts.Token));
            }
            catch (Exception ex)
            {
                log.LogError($"[DGLab-Server] 启动失败: {ex.Message}");
            }
        }

        public void Stop()
        {
            cts?.Cancel();
            listener?.Stop();
            CloseAppConnection();
        }

        private void CloseAppConnection()
        {
            appStream?.Close();
            connectedAppClient?.Close();
            appStream = null;
            connectedAppClient = null;
            IsBound = false;
            TargetId = "";
        }

        private async Task AcceptClientsLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleHandshakeAndRouting(client, token));
                }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex)
                {
                    log.LogError($"[DGLab-Server] 连接异常: {ex.Message}");
                }
            }
        }

        private async Task HandleHandshakeAndRouting(TcpClient client, CancellationToken token)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024 * 4];

            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);


                if (Regex.IsMatch(request, "Upgrade: websocket", RegexOptions.IgnoreCase))
                {
                    var match = Regex.Match(request, @"sec-websocket-key:\s*([^\r\n]+)", RegexOptions.IgnoreCase);
                    string key = match.Success ? match.Groups[1].Value.Trim() : "";
                    string acceptKey = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11")));

                    string response = "HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\n" +
                                      $"Sec-WebSocket-Accept: {acceptKey}\r\n\r\n";

                    await stream.WriteAsync(Encoding.UTF8.GetBytes(response), 0, response.Length, token);
                    CloseAppConnection();
                    connectedAppClient = client;
                    appStream = stream;

                    log.LogInfo("[DGLab-Server] 手机 App 握手成功，开始同步配对");
                    AddWebLog("手机 App 握手成功，开始同步配对");

                    string initHandshake = $"{{\"type\":\"bind\",\"clientId\":\"{ClientId}\",\"targetId\":\"{ClientId}\",\"message\":\"targetId\"}}";
                    await SendMessageAsync(initHandshake);
                    await ReceiveLoop(stream, token);
                }

                else if (request.Contains("GET /api/status"))
                {
                    string jsonResponse = $"{{\"isBound\":{IsBound.ToString().ToLower()},\"logs\":{GetWebLogsJson()}}}";
                    SendHttpResponse(stream, "application/json", jsonResponse);
                    client.Close();
                }

                else if (request.Contains("GET /api/test"))
                {
                    string channel = request.Contains("channel=B") ? "B" : "A";
                    TriggerTestPulse(channel);
                    SendHttpResponse(stream, "application/json", $"{{\"status\":\"sent\",\"channel\":\"{channel}\"}}");
                    client.Close();
                }

                else if (request.Contains("GET /api/setstrength"))
                {
                    string channel = request.Contains("channel=B") ? "B" : "A";
                    var matchValue = Regex.Match(request, @"value=(\d+)");
                    int value = matchValue.Success ? int.Parse(matchValue.Groups[1].Value) : 0;

                    SetAppStrength(channel, value);
                    SendHttpResponse(stream, "application/json", $"{{\"status\":\"ok\",\"value\":{value}}}");
                    client.Close();
                }

                else if (request.Contains("GET /api/getconfig"))
                {
                    JObject cfgJson = new JObject
                    {
                        ["enableInstant"] = CoyoteUnknownPlugin.EnableInstant.Value,
                        ["enableContinuous"] = CoyoteUnknownPlugin.EnableContinuous.Value,
                        ["maxStrengthA"] = CoyoteUnknownPlugin.MaxStrengthA.Value,
                        ["maxStrengthB"] = CoyoteUnknownPlugin.MaxStrengthB.Value,
                        ["damageMultiplier"] = CoyoteUnknownPlugin.DamageMultiplier.Value,
                        ["minHeartRate"] = CoyoteUnknownPlugin.MinHeartRateToShock.Value
                    };
                    SendHttpResponse(stream, "application/json", cfgJson.ToString());
                    client.Close();
                }

                else if (request.Contains("GET /api/setconfig"))
                {
                    var matchKey = Regex.Match(request, @"key=([^&\s]+)");
                    var matchVal = Regex.Match(request, @"value=([^&\s]+)");
                    if (matchKey.Success && matchVal.Success)
                    {
                        string k = matchKey.Groups[1].Value;
                        string v = matchVal.Groups[1].Value;
                        CoyoteUnknownPlugin.Instance.UpdateConfigFromWeb(k, v);
                    }
                    SendHttpResponse(stream, "application/json", "{\"status\":\"saved\"}");
                    client.Close();
                }

                else if (request.Contains("GET /panel"))
                {
                    SendHttpResponse(stream, "text/html; charset=utf-8", DGLabWebTemplates.GetPanelHtml(GetLocalIP()));
                    client.Close();
                }

                else if (request.Contains("GET / ") || request.Contains("GET /index.html"))
                {
                    string ip = GetLocalIP();
                    string qrUrl = $"https://www.dungeon-lab.com/app-download.php#DGLAB-SOCKET#ws://{ip}:{Port}/{ClientId}";
                    SendHttpResponse(stream, "text/html; charset=utf-8", DGLabWebTemplates.GetIndexHtml(ip, qrUrl));
                    client.Close();
                }
                else
                {
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                log.LogWarning($"[DGLab-Server] 路由处理异常: {ex.Message}");
                client.Close();
            }
        }

        private static async Task<bool> ReadExactlyAsync(NetworkStream stream, byte[] buffer, int count, CancellationToken token)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int read = await stream.ReadAsync(buffer, totalRead, count - totalRead, token);
                if (read <= 0) return false;
                totalRead += read;
            }
            return true;
        }

        private async Task ReceiveLoop(NetworkStream stream, CancellationToken token)
        {
            byte[] header = new byte[2];
            while (!token.IsCancellationRequested && connectedAppClient != null && connectedAppClient.Connected)
            {
                try
                {
                    if (!await ReadExactlyAsync(stream, header, 2, token)) break;
                    int b1 = header[0];
                    int b2 = header[1];

                    if (b1 == 0x88) break;

                    bool isMasked = (b2 & 0x80) != 0;
                    int payloadLen = b2 & 0x7F;

                    if (payloadLen == 126)
                    {
                        byte[] extLen = new byte[2];
                        if (!await ReadExactlyAsync(stream, extLen, 2, token)) break;
                        payloadLen = (extLen[0] << 8) | extLen[1];
                    }

                    byte[] masks = new byte[4];
                    if (isMasked && !await ReadExactlyAsync(stream, masks, 4, token)) break;

                    byte[] payload = new byte[payloadLen];
                    if (!await ReadExactlyAsync(stream, payload, payloadLen, token)) break;

                    if (isMasked)
                    {
                        for (int i = 0; i < payloadLen; i++) payload[i] = (byte)(payload[i] ^ masks[i % 4]);
                    }


                    if (b1 == 0x89)
                    {
                        await SendPongAsync(payload);
                        continue;
                    }

                    HandleAppMessage(Encoding.UTF8.GetString(payload));
                }
                catch { break; }
            }
            log.LogWarning("[DGLab-Server] APP 连接断开");
            AddWebLog("APP 连接断开");
            CloseAppConnection();
        }

        private async Task SendPongAsync(byte[] payload)
        {
            if (appStream == null || !connectedAppClient.Connected) return;
            try
            {
                byte[] frame = new byte[2 + payload.Length];
                frame[0] = 0x8A;
                frame[1] = (byte)payload.Length;
                Array.Copy(payload, 0, frame, 2, payload.Length);
                await appStream.WriteAsync(frame, 0, frame.Length);
            }
            catch { }
        }

        private void HandleAppMessage(string rawMessage)
        {
            try
            {
                JObject json = JObject.Parse(rawMessage);
                string type = json["type"]?.ToString();
                string targetId = json["targetId"]?.ToString();
                string message = json["message"]?.ToString();

                if (type == "bind" && message == "DGLAB")
                {
                    IsBound = true;
                    TargetId = targetId;
                    log.LogInfo($"[DGLab-Server] 配对绑定成功，(TargetID: {TargetId})");
                    AddWebLog("成功建立与官方 APP 的放电绑定通道");

                    string response = $"{{\"type\":\"bind\",\"clientId\":\"{ClientId}\",\"targetId\":\"{TargetId}\",\"message\":\"200\"}}";
                    _ = SendMessageAsync(response);
                }
                else if (type == "msg" && message != null && message.StartsWith("strength-"))
                {
                    AddWebLog($"App 电流反馈强度改变: {message}");
                }
            }
            catch { }
        }

        public async Task SendMessageAsync(string jsonPayload)
        {
            if (appStream == null || !connectedAppClient.Connected) return;
            try
            {
                byte[] payload = Encoding.UTF8.GetBytes(jsonPayload);
                byte[] frame;
                if (payload.Length < 126)
                {
                    frame = new byte[2 + payload.Length];
                    frame[0] = 0x81;
                    frame[1] = (byte)payload.Length;
                    Array.Copy(payload, 0, frame, 2, payload.Length);
                }
                else
                {
                    frame = new byte[4 + payload.Length];
                    frame[0] = 0x81;
                    frame[1] = 126;
                    frame[2] = (byte)((payload.Length >> 8) & 0xFF);
                    frame[3] = (byte)(payload.Length & 0xFF);
                    Array.Copy(payload, 0, frame, 4, payload.Length);
                }
                await appStream.WriteAsync(frame, 0, frame.Length);
            }
            catch
            {
                CloseAppConnection();
            }
        }

        private void TriggerTestPulse(string channel)
        {
            if (!IsBound) return;
            log.LogInfo($"[DGLab-Server] 测试通道 {channel}");
            AddWebLog($"[Web-Panel] 主动向通道 {channel} 触发 1 秒安全调试脉冲");
            string testJson = $"{{\"type\":\"msg\",\"clientId\":\"{ClientId}\",\"targetId\":\"{TargetId}\",\"message\":\"pulse-{channel}:[\\\"1414141419191919\\\"]\"}}";
            _ = SendMessageAsync(testJson);
        }

        public void SetAppStrength(string channel, int value)
        {
            if (!IsBound) return;
            int chanId = (channel.ToUpper() == "B") ? 2 : 1;
            int safeVal = Math.Max(0, Math.Min(200, value));

            string strengthJson = $"{{\"type\":\"msg\",\"clientId\":\"{ClientId}\",\"targetId\":\"{TargetId}\",\"message\":\"strength-{chanId}+2+{safeVal}\"}}";
            _ = SendMessageAsync(strengthJson);
        }

        private void SendHttpResponse(NetworkStream stream, string contentType, string content)
        {
            try
            {
                byte[] contentBytes = Encoding.UTF8.GetBytes(content);
                string header = "HTTP/1.1 200 OK\r\n" +
                               $"Content-Type: {contentType}\r\n" +
                               $"Content-Length: {contentBytes.Length}\r\n" +
                               "Connection: close\r\n\r\n";
                byte[] headerBytes = Encoding.UTF8.GetBytes(header);
                stream.Write(headerBytes, 0, headerBytes.Length);
                stream.Write(contentBytes, 0, contentBytes.Length);
            }
            catch { }
        }

        public string GetLocalIP()
        {
            string ip = CoyoteUnknownPlugin.OverrideLocalIp.Value.Trim();
            return string.IsNullOrEmpty(ip) ? GetLocalIPAddress() : ip;
        }

        private static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ipStr = ip.ToString();
                        if (ipStr.StartsWith("127.") || ipStr.StartsWith("198.18.") || ipStr.StartsWith("169.254.") || ipStr.StartsWith("25.") || ipStr.StartsWith("26."))
                        {
                            continue;
                        }
                        if (ipStr.StartsWith("192.168.") || ipStr.StartsWith("10.")) return ipStr;
                    }
                }
            }
            catch { }
            return "127.0.0.1";
        }
    }
}