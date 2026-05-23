using BepInEx;
using BepInEx.Configuration;
using System;
using UnityEngine;
using ScavLib.mods;
using ScavLib.command;
using ScavLib.gui;

namespace CoyoteUnknown
{
    [BepInDependency("com.kanisuko.scavlib", "0.2.0")]
    [BepInPlugin(MyGUID, MyName, MyVersion)]
    public class CoyoteUnknownPlugin : BaseUnityPlugin
    {
        public const string MyGUID = "com.kanisuko.coyoteunknown";
        public const string MyName = "Coyote Unknown";
        public const string MyVersion = "0.2.1";

        public static CoyoteUnknownPlugin Instance { get; private set; }


        public static ConfigEntry<string> WsUrl { get; private set; }
        public static ConfigEntry<string> OverrideLocalIp { get; private set; }
        public static ConfigEntry<bool> EnableInstant { get; private set; }
        public static ConfigEntry<bool> EnableContinuous { get; private set; }
        public static ConfigEntry<float> MaxStrengthA { get; private set; }
        public static ConfigEntry<float> MaxStrengthB { get; private set; }
        public static ConfigEntry<float> DamageMultiplier { get; private set; }
        public static ConfigEntry<float> MinHeartRateToShock { get; private set; }

        public static ConfigEntry<string> InstantChannel { get; private set; }
        public static ConfigEntry<string> ContinuousChannel { get; private set; }

        public static DGLabServer GlabServer { get; private set; }
        public static CoyoteMenu MenuInstance { get; private set; }


        private CoyoteFeedbackManager feedbackManager;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo("正在初始化 Coyote Unknown (未知郊狼) 模块化版本...");

            try
            {
                BindConfigurations();
                feedbackManager = new CoyoteFeedbackManager();

                ModRegistry.Register(new ModInfo(
                    name: MyName,
                    version: MyVersion,
                    description: "DG-Lab Coyote Local Server Link.",
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
                        EnableInstant.Value = bool.Parse(value);
                        break;
                    case "EnableContinuousFeedback":
                        EnableContinuous.Value = bool.Parse(value);
                        break;
                    case "DamageMultiplier":
                        DamageMultiplier.Value = float.Parse(value);
                        break;
                    case "MinHeartRateToShock":
                        MinHeartRateToShock.Value = float.Parse(value);
                        break;
                    case "MaxStrengthA":
                        MaxStrengthA.Value = float.Parse(value);
                        break;
                    case "MaxStrengthB":
                        MaxStrengthB.Value = float.Parse(value);
                        break;
                }


                Config.Save();
                Logger.LogInfo($"[Coyote Unknown] 网页端已成功修改参数: {key} -> {value} 并安全存盘");
                GlabServer.AddWebLog($"[Web] 修改游戏参数: {key} -> {value}");
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
                Logger.LogInfo("Coyote Unknown 服务端已成功关闭");
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

        private void BindConfigurations()
        {
            WsUrl = Config.Bind("1. Network", "ServerAddress", "ws://127.0.0.1:9999", "备用中继");
            OverrideLocalIp = Config.Bind("1. Network", "OverrideLocalIP", "", "手动 IP");

            EnableInstant = Config.Bind("2. Features", "EnableInstantShock", true, "瞬间反馈");
            EnableContinuous = Config.Bind("2. Features", "EnableContinuousFeedback", true, "持续反馈");

            InstantChannel = Config.Bind("2. Features", "InstantChannel", "A", "瞬间通道映射");
            ContinuousChannel = Config.Bind("2. Features", "ContinuousChannel", "B", "持续通道映射");

            MaxStrengthA = Config.Bind("3. Safety Clamps", "MaxStrengthA", 20f, "安全电击上限 A");
            MaxStrengthB = Config.Bind("3. Safety Clamps", "MaxStrengthB", 15f, "安全电击上限 B");
            DamageMultiplier = Config.Bind("4. Tuning", "DamageMultiplier", 1.5f, "失血伤害电击倍率");
            MinHeartRateToShock = Config.Bind("4. Tuning", "MinHeartRateToShock", 100f, "触发背景心率");
        }
    }
}