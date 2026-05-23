using BepInEx;
using System;
using UnityEngine;
using ScavLib.mods;
using ScavLib.command;
using ScavLib.gui;
using CoyoteUnknown.Config;
using CoyoteUnknown.Feedback;
using CoyoteUnknown.Network;   
using CoyoteUnknown.UI;        

namespace CoyoteUnknown
{
    [BepInDependency("com.kanisuko.scavlib", "0.2.2")]
    [BepInPlugin(MyGUID, MyName, MyVersion)]
    public class CoyoteUnknownPlugin : BaseUnityPlugin
    {
        public const string MyGUID = "com.kanisuko.coyoteunknown";
        public const string MyName = "Coyote Unknown";
        public const string MyVersion = "0.3.0";

        public static CoyoteUnknownPlugin Instance { get; private set; }
        public static DGLabServer GlabServer { get; private set; }
        public static CoyoteMenu MenuInstance { get; private set; }

        private CoyoteFeedbackManager feedbackManager;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo("正在初始化 Coyote Unknown (未知郊狼) 模块化重构版...");

            try
            {
                
                ModConfig.Bind(Config);

                
                feedbackManager = new CoyoteFeedbackManager();

                
                ModRegistry.Register(new ModInfo(
                    name: MyName,
                    version: MyVersion,
                    description: "DG-Lab Coyote Local Server Link. (Modularized Framework)",
                    author: "Kanisuko"
                ));

                CommandRegistry.Register(new CoyoteCommand());
                MenuInstance = new CoyoteMenu();
                MenuManager.Register(MenuInstance);

                
                GlabServer = new DGLabServer(Logger);
                GlabServer.Start();

                string webAddress = $"http://127.0.0.1:{DGLabServer.Port}";
                GUIUtility.systemCopyBuffer = webAddress;

                Logger.LogMessage($"==============================================================");
                Logger.LogMessage($"[Coyote Unknown] 模块化服务已就绪！网页控制台: {webAddress}");
                Logger.LogMessage($"==============================================================");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Coyote Unknown 初始化失败: {ex.Message}");
            }
        }

        private void Update()
        {
            feedbackManager?.UpdateFeedback(GlabServer);
        }

        
        
        
        public void UpdateConfigFromWeb(string key, string value)
        {
            try
            {
                switch (key)
                {
                    case "EnableInstantShock":
                        ModConfig.EnableInstant.Value = bool.Parse(value);
                        break;
                    case "EnableContinuousFeedback":
                        ModConfig.EnableContinuous.Value = bool.Parse(value);
                        break;
                    case "DamageMultiplier":
                        ModConfig.DamageMultiplier.Value = float.Parse(value);
                        break;
                    case "MinHeartRateToShock":
                        ModConfig.MinHeartRateToShock.Value = float.Parse(value);
                        break;
                    case "MaxStrengthA":
                        ModConfig.MaxStrengthA.Value = float.Parse(value);
                        break;
                    case "MaxStrengthB":
                        ModConfig.MaxStrengthB.Value = float.Parse(value);
                        break;
                    case "IsProfessionalMode":
                        ModConfig.IsProfessionalMode.Value = bool.Parse(value);
                        break;
                    case "StrengthControlMode":
                        ModConfig.StrengthControlMode.Value = value;
                        break;
                    case "UseAbsoluteValue":
                        ModConfig.UseAbsoluteValue.Value = bool.Parse(value);
                        break;
                    case "BloodLossWaveform":
                        ModConfig.BloodLossWaveform.Value = value;
                        break;
                    case "PainWaveform":
                        ModConfig.PainWaveform.Value = value;
                        break;
                    case "HeartRateWaveform":
                        ModConfig.HeartRateWaveform.Value = value;
                        break;
                    case "AdrenalineWaveform":
                        ModConfig.AdrenalineWaveform.Value = value;
                        break;
                }

                Config.Save();
                Logger.LogInfo($"[Coyote Unknown] 网页端修改参数并安全盘存: {key} -> {value}");
                GlabServer.AddWebLog($"[Web] 修改参数: {key} -> {value}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Coyote Unknown] 网页写入参数 {key} 失败: {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            if (GlabServer != null)
            {
                feedbackManager?.ResetOutputs(GlabServer);
                GlabServer.Stop();
                Logger.LogInfo("Coyote Unknown 服务端已安全关闭");
            }
        }

        public static void ShowInGameMenu()
        {
            MenuInstance?.Toggle();
        }

        public static void LogToGameConsole(string message)
        {
            if (ConsoleScript.instance != null)
            {
                try
                {
                    var method = typeof(ConsoleScript).GetMethod("LogToConsole",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    method?.Invoke(ConsoleScript.instance, new object[] { message });
                }
                catch { }
            }
        }
    }
}