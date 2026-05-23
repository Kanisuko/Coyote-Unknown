using System;

namespace CoyoteUnknown.Network
{
    public static class DGLabWebTemplates
    {
        public static string GetIndexHtml(string ip, string qrUrl)
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

        public static string GetPanelHtml(string ip)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Coyote Unknown 控制面板</title>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, sans-serif; background: #121212; color: #e0e0e0; max-width: 600px; margin: 20px auto; padding: 20px; text-align: center; }}
        .card {{ background: #1e1e1e; padding: 28px; border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.6); border: 1px solid #333; text-align: left; }}
        h1 {{ color: #ff9800; font-size: 24px; margin: 0 0 10px 0; text-align: center; }}
        .status {{ margin-bottom: 20px; text-align: center; font-size: 14px; color: #aaa; }}
        .section {{ background: #262626; padding: 18px; border-radius: 10px; margin-bottom: 18px; border: 1px solid #3c3c3c; }}
        h3 {{ margin: 0 0 14px 0; font-size: 16px; color: #ff9800; border-left: 4px solid #ff9800; padding-left: 8px; }}

        .row {{ display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; }}
        .row:last-child {{ margin-bottom: 0; }}
        label {{ font-size: 14px; font-weight: bold; width: 180px; }}
        input[type=range] {{ flex-grow: 1; margin: 0 16px; accent-color: #ff9800; }}
        .val {{ font-size: 14px; font-weight: bold; color: #ff9800; width: 45px; text-align: right; }}

        .btn {{ background: #2196f3; color: white; border: none; padding: 8px 16px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s; }}
        .btn:hover {{ background: #42a5f5; }}
        .btn-test {{ background: #e53935; }}
        .btn-test:hover {{ background: #ef5350; }}
        .switch-input {{ cursor: pointer; transform: scale(1.2); }}

        .log-console {{ background: #000; border: 1px solid #444; border-radius: 8px; height: 140px; padding: 12px; font-family: ""Consolas"", monospace; font-size: 12px; overflow-y: auto; color: #00ff00; box-sizing: border-box; }}
        .log-line {{ margin-bottom: 4px; }}
    </style>
</head>
<body onload=""initPage()"">
    <div class=""card"">
        <h1>Coyote Unknown 控制面板</h1>
        <div class=""status"">
            局域网探测 IP: <strong style=""color: #2196f3;"">{ip}</strong> | 绑定状态: <strong style=""color: green;"">已配对</strong>
        </div>

        <div class=""section"">
            <h3>手动调节与物理测试</h3>
            <div class=""row"">
                <label>通道 A 实时强度</label>
                <input type=""range"" id=""slider-a"" min=""0"" max=""100"" value=""0"" onchange=""setStrength('A', this.value)"" oninput=""updateValText('val-a', this.value)"">
                <span class=""val"" id=""val-a"">0</span>
            </div>
            <div class=""row"">
                <label>通道 B 实时强度</label>
                <input type=""range"" id=""slider-b"" min=""0"" max=""100"" value=""0"" onchange=""setStrength('B', this.value)"" oninput=""updateValText('val-b', this.value)"">
                <span class=""val"" id=""val-b"">0</span>
            </div>
            <div class=""row"" style=""margin-top:14px; justify-content: flex-start; gap: 10px;"">
                <button class=""btn btn-test"" onclick=""testChannel('A')"">测试放电 A</button>
                <button class=""btn btn-test"" onclick=""testChannel('B')"">测试放电 B</button>
            </div>
        </div>

        <div class=""section"">
            <h3>游戏联动参数实时配置 (自动保存)</h3>
            <div class=""row"">
                <label>开启受击/自残瞬间电击</label>
                <input type=""checkbox"" id=""cfg-instant"" class=""switch-input"" onchange=""setConfig('EnableInstantShock', this.checked)"">
            </div>
            <div class=""row"">
                <label>开启生理体征持续反馈</label>
                <input type=""checkbox"" id=""cfg-continuous"" class=""switch-input"" onchange=""setConfig('EnableContinuousFeedback', this.checked)"">
            </div>
            <div class=""row"">
                <label>受击强度折算比例</label>
                <input type=""range"" id=""cfg-dmg-mult"" min=""0.5"" max=""5.0"" step=""0.1"" onchange=""setConfig('DamageMultiplier', this.value)"" oninput=""updateValText('val-cfg-dmg', this.value)"">
                <span class=""val"" id=""val-cfg-dmg"">1.5</span>
            </div>
            <div class=""row"">
                <label>心跳背景震动最低阈值</label>
                <input type=""range"" id=""cfg-min-hr"" min=""60"" max=""150"" step=""5"" onchange=""setConfig('MinHeartRateToShock', this.value)"" oninput=""updateValText('val-cfg-hr', this.value)"">
                <span class=""val"" id=""val-cfg-hr"">100</span>
            </div>
            <div class=""row"">
                <label>安全电击上限 A</label>
                <input type=""range"" id=""cfg-max-a"" min=""0"" max=""100"" onchange=""setConfig('MaxStrengthA', this.value)"" oninput=""updateValText('val-cfg-max-a', this.value)"">
                <span class=""val"" id=""val-cfg-max-a"">20</span>
            </div>
            <div class=""row"">
                <label>安全电击上限 B</label>
                <input type=""range"" id=""cfg-max-b"" min=""0"" max=""200"" onchange=""setConfig('MaxStrengthB', this.value)"" oninput=""updateValText('val-cfg-max-b', this.value)"">
                <span class=""val"" id=""val-cfg-max-b"">15</span>
            </div>
        </div>

        <div class=""section"">
            <h3>联动系统控制台日志</h3>
            <div class=""log-console"" id=""console-box"">
                <div class=""log-line"">[System] 正在读取系统日志流...</div>
            </div>
        </div>
    </div>

    <script>
        function setStrength(ch, val) {{
            fetch('/api/setstrength?channel=' + ch + '&value=' + val);
        }}
        function updateValText(id, val) {{
            document.getElementById(id).innerText = val;
        }}
        function testChannel(ch) {{
            fetch('/api/test?channel=' + ch);
        }}

        function setConfig(key, val) {{
            fetch('/api/setconfig?key=' + key + '&value=' + val);
        }}

        function initPage() {{
            fetch('/api/getconfig')
                .then(r => r.json())
                .then(cfg => {{
                    document.getElementById('cfg-instant').checked = cfg.enableInstant;
                    document.getElementById('cfg-continuous').checked = cfg.enableContinuous;

                    document.getElementById('cfg-dmg-mult').value = cfg.damageMultiplier;
                    document.getElementById('val-cfg-dmg').innerText = cfg.damageMultiplier;

                    document.getElementById('cfg-min-hr').value = cfg.minHeartRate;
                    document.getElementById('val-cfg-hr').innerText = cfg.minHeartRate;

                    document.getElementById('cfg-max-a').value = cfg.maxStrengthA;
                    document.getElementById('val-cfg-max-a').innerText = cfg.maxStrengthA;

                    document.getElementById('cfg-max-b').value = cfg.maxStrengthB;
                    document.getElementById('val-cfg-max-b').innerText = cfg.maxStrengthB;
                }});
        }}

        setInterval(() => {{
            fetch('/api/status')
                .then(r => r.json())
                .then(data => {{
                    if (!data.isBound) {{
                        window.location.href = '/';
                        return;
                    }}
                    const consoleBox = document.getElementById('console-box');
                    consoleBox.innerHTML = '';
                    data.logs.forEach(logLine => {{
                        const div = document.createElement('div');
                        div.className = 'log-line';
                        div.innerText = logLine;
                        consoleBox.appendChild(div);
                    }});
                    consoleBox.scrollTop = consoleBox.scrollHeight;
                }});
        }}, 1000);
    </script>
</body>
</html>";
        }
    }
}