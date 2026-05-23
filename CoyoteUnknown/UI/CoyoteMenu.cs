using ScavLib.gui;
using UnityEngine;
using CoyoteUnknown.Config;
using CoyoteUnknown.Network;

namespace CoyoteUnknown.UI
{
    public class CoyoteMenu : MenuWindow
    {
        public override string Title => "Coyote Unknown 极简联动控制";
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
            string localWebUrl = $"http://localhost:{DGLabServer.Port}"; 

            string statusColor = server.IsBound ? "green" : "orange";
            string statusText = server.IsBound ? "已配对成功" : "等待 APP 扫码...";
            MenuBuilder.Label($"当前状态: <color={statusColor}><b>{statusText}</b></color>");
            MenuBuilder.Label($"电脑内网 IP: {ip}");

            MenuBuilder.Separator();

            
            MenuBuilder.Label("<color=orange><b>【游戏内主控滑块】</b></color>");
            MenuBuilder.SliderConfig("全局电击倍率", ModConfig.GlobalMultiplier, 0.5f, 5.0f);

            float maxSliderLimit = ModConfig.UseAbsoluteValue.Value ? 200f : 100f;
            string unitLabel = ModConfig.UseAbsoluteValue.Value ? "" : "%";
            MenuBuilder.SliderConfig($"通道 A 强度限制 ({unitLabel})", ModConfig.MaxStrengthA, 0f, maxSliderLimit);
            MenuBuilder.SliderConfig($"通道 B 强度限制 ({unitLabel})", ModConfig.MaxStrengthB, 0f, maxSliderLimit);

            MenuBuilder.Separator();

            
            if (MenuBuilder.Button("一键复制控制台链接 (推荐)"))
            {
                GUIUtility.systemCopyBuffer = localWebUrl;
                Debug.Log($"<color=green>[Coyote Unknown] 网页控制台链接已复制到剪贴板: {localWebUrl}</color>");
            }

            if (MenuBuilder.Button("一键复制手机扫码链接"))
            {
                GUIUtility.systemCopyBuffer = qrUrl;
                Debug.Log("<color=green>[Coyote Unknown] 手机 App 扫码配对链接已成功复制到系统剪贴板！</color>");
            }

            MenuBuilder.Separator();

            
            MenuBuilder.ToggleConfig("开启受击/自残瞬间反馈", ModConfig.EnableInstant);
            MenuBuilder.ToggleConfig("开启生理持续反馈", ModConfig.EnableContinuous);

            if (ModConfig.IsProfessionalMode.Value)
            {
                MenuBuilder.Separator();
                MenuBuilder.Label("<color=grey>提示: 您已在网页端开启了高阶专业联动如需修改辐射、骨折、失温等细节参数，请通过浏览器控制面板进行调节</color>");
            }
        }
    }
}