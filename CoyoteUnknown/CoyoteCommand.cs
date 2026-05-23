using ScavLib.command;
using System.Collections.Generic;
using UnityEngine;

namespace CoyoteUnknown
{
    public class CoyoteCommand : BaseCommand
    {
        public override string Name => "coyote";
        public override string Description => "显示 Coyote Unknown 连接状态或调出 UI (F6)";

        public override Dictionary<int, List<string>> ArgAutofill =>
            new Dictionary<int, List<string>>
            {
                {
                    0, new List<string> { "ui", "status", "start", "stop" }
                }
            };

        public override (string, string)[] ArgDescription => new (string, string)[]
        {
            ("action", "可选操作: ui (开闭浮窗), status (查看状态), start (启动服务), stop (关闭服务)")
        };

        public override void Execute(string[] args)
        {
            if (CoyoteUnknownPlugin.GlabServer == null)
            {
                CoyoteUnknownPlugin.LogToGameConsole("<color=orange>[Coyote Unknown] 本地服务端尚未初始化</color>");
                return;
            }

            var server = CoyoteUnknownPlugin.GlabServer;

            if (args != null && args.Length > 1)
            {
                string cmd = args[1].ToLower();
                switch (cmd)
                {
                    case "ui":
                        CoyoteUnknownPlugin.MenuInstance?.Toggle();
                        return;

                    case "stop":
                        server.Stop();
                        CoyoteUnknownPlugin.LogToGameConsole("<color=yellow>[Coyote Unknown] 本地电击服务端已安全关闭</color>");
                        return;

                    case "start":
                        server.Start();
                        CoyoteUnknownPlugin.LogToGameConsole("<color=green>[Coyote Unknown] 本地服务端尝试启动中...</color>");
                        return;

                    case "status":
                        string curIp = server.GetLocalIP();
                        CoyoteUnknownPlugin.LogToGameConsole("--------------------------------------------------");
                        CoyoteUnknownPlugin.LogToGameConsole($"[Coyote Unknown] 服务状态: {(server.IsBound ? "<color=green>已配对并在线</color>" : "<color=orange>等待配对中...</color>")}");
                        CoyoteUnknownPlugin.LogToGameConsole($"运行端口: {DGLabServer.Port}");
                        CoyoteUnknownPlugin.LogToGameConsole($"本机内网 IP: {curIp}");
                        if (server.IsBound)
                        {
                            CoyoteUnknownPlugin.LogToGameConsole($"手机 APP ID: {server.TargetId}");
                        }
                        CoyoteUnknownPlugin.LogToGameConsole("--------------------------------------------------");
                        return;
                }
            }

            string ip = server.GetLocalIP();
            string qrUrl = $"https://www.dungeon-lab.com/app-download.php#DGLAB-SOCKET#ws://{ip}:{DGLabServer.Port}/{server.ClientId}";

            GUIUtility.systemCopyBuffer = qrUrl;

            CoyoteUnknownPlugin.LogToGameConsole("--------------------------------------------------");
            CoyoteUnknownPlugin.LogToGameConsole($"[Coyote Unknown] 配对状态: {(server.IsBound ? "<color=green>已连接</color>" : "<color=orange>等待配对中...</color>")}");
            CoyoteUnknownPlugin.LogToGameConsole($"电脑内网 IP: {ip}");
            CoyoteUnknownPlugin.LogToGameConsole($"配对链接: {qrUrl}");
            CoyoteUnknownPlugin.LogToGameConsole("<color=green>[配对链接已自动复制到您的电脑剪贴板]</color>");
            CoyoteUnknownPlugin.LogToGameConsole("控制台可用指令： <color=yellow>coyote ui</color> | <color=yellow>coyote status</color> | <color=yellow>coyote stop</color> | <color=yellow>coyote start</color>");
            CoyoteUnknownPlugin.LogToGameConsole("--------------------------------------------------");
        }
    }
}