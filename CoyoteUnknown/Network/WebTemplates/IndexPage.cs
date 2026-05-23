using System;

namespace CoyoteUnknown.Network.WebTemplates
{
    public static class IndexPage
    {
        public static string GetHtml(string ip, string qrUrl)
        {
            string encodedQrUrl = Uri.EscapeDataString(qrUrl);
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Coyote Unknown 连接引导</title>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background: #121212; color: #e0e0e0; max-width: 500px; margin: 40px auto; padding: 20px; text-align: center; }}
        .card {{ background: #1e1e1e; padding: 28px; border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.6); border: 1px solid #333; }}
        h1 {{ color: #ff9800; font-size: 24px; margin-bottom: 4px; font-weight: 600; }}
        img {{ border: 10px solid white; border-radius: 8px; margin: 24px 0; box-shadow: 0 4px 10px rgba(0,0,0,0.5); }}
    </style>
</head>
<body>
    <div class=""card"">
        <h1>Coyote Unknown</h1>
        <img src=""https://api.qrserver.com/v1/create-qr-code/?size=240x240&data={encodedQrUrl}"" alt=""QR Code"">
        <div style=""margin: 20px 0; font-size: 15px; line-height: 1.6;"">
            局域网探测 IP: <strong style=""color: #2196f3;"">{ip}</strong><br>
            连接状态: <strong id=""status-val"" style=""color: orange;"">等待 APP 扫码...</strong>
        </div>
    </div>
    <script>
        setInterval(() => {{
            fetch('/api/status')
                .then(r => r.json())
                .then(data => {{
                    if (data.isBound) {{ window.location.href = '/panel'; }}
                }});
        }}, 1000);
    </script>
</body>
</html>";
        }
    }
}