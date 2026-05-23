using ScavLib.gui;
using UnityEngine;

namespace CoyoteUnknown
{
    public class CoyoteMenu : MenuWindow
    {
        public override string Title => "Coyote Unknown 连接助手";
        public override KeyCode ToggleKey => KeyCode.F6;
        public override float Width => 320f;

        protected override void DrawContent()
        {
            var server = CoyoteUnknownPlugin.GlabServer;
            if (server == null)
            {
                MenuBuilder.Label("本地微型服务端未启动");
                return;
            }

            string ip = server.GetLocalIP();
            string qrUrl = $"https://www.dungeon-lab.com/app-download.php#DGLAB-SOCKET#ws://{ip}:{DGLabServer.Port}/{server.ClientId}";


            string statusColor = server.IsBound ? "green" : "orange";
            string statusText = server.IsBound ? "已配对成功" : "等待 APP 扫码...";
            MenuBuilder.Label($"当前状态: <color={statusColor}><b>{statusText}</b></color>");
            MenuBuilder.Label($"电脑局域网 IP: {ip}");

            MenuBuilder.Separator();


            MenuBuilder.Label("配对链接（点击下方按钮复制）：");
            if (MenuBuilder.Button("一键复制配对链接"))
            {
                GUIUtility.systemCopyBuffer = qrUrl;
                Debug.Log("<color=green>[Coyote Unknown] 配对链接已成功复制到系统剪贴板！</color>");
            }

            MenuBuilder.Separator();


            MenuBuilder.ToggleConfig("开启受击/自残瞬间反馈", CoyoteUnknownPlugin.EnableInstant);
            MenuBuilder.SliderConfig("通道 A 瞬间上限", CoyoteUnknownPlugin.MaxStrengthA, 0f, 100f);

            MenuBuilder.Separator();

            MenuBuilder.ToggleConfig("开启生理持续反馈", CoyoteUnknownPlugin.EnableContinuous);
            MenuBuilder.SliderConfig("通道 B 背景上限", CoyoteUnknownPlugin.MaxStrengthB, 0f, 100f);
        }
    }
}