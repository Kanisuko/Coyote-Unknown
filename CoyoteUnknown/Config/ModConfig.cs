using BepInEx.Configuration;

namespace CoyoteUnknown.Config
{
    public static class ModConfig
    {
        
        public static ConfigEntry<string> WsUrl { get; private set; }
        public static ConfigEntry<string> OverrideLocalIp { get; private set; }

        
        public static ConfigEntry<bool> IsProfessionalMode { get; private set; }
        public static ConfigEntry<string> StrengthControlMode { get; private set; }
        public static ConfigEntry<bool> UseAbsoluteValue { get; private set; }
        public static ConfigEntry<float> GlobalMultiplier { get; private set; }      
        public static ConfigEntry<bool> EnableModClamp { get; private set; }         
        public static ConfigEntry<float> ModClampA { get; private set; }             
        public static ConfigEntry<float> ModClampB { get; private set; }             

        
        public static ConfigEntry<bool> EnableInstant { get; private set; }
        public static ConfigEntry<bool> EnableContinuous { get; private set; }
        public static ConfigEntry<string> InstantChannel { get; private set; }
        public static ConfigEntry<string> ContinuousChannel { get; private set; }

        
        public static ConfigEntry<float> MaxStrengthA { get; private set; }
        public static ConfigEntry<float> MaxStrengthB { get; private set; }

        
        public static ConfigEntry<float> DamageMultiplier { get; private set; }
        public static ConfigEntry<float> MinHeartRateToShock { get; private set; }

        
        public static ConfigEntry<string> BloodLossWaveform { get; private set; }
        public static ConfigEntry<string> PainWaveform { get; private set; }
        public static ConfigEntry<string> HeartRateWaveform { get; private set; }
        public static ConfigEntry<string> AdrenalineWaveform { get; private set; }
        public static ConfigEntry<string> RadiationWaveform { get; private set; }
        public static ConfigEntry<string> ColdWaveform { get; private set; }
        public static ConfigEntry<string> HypoxiaWaveform { get; private set; }
        public static ConfigEntry<string> FractureWaveform { get; private set; }
        public static ConfigEntry<string> PoisonWaveform { get; private set; }

        
        public static ConfigEntry<bool> EnableRadiation { get; private set; }
        public static ConfigEntry<float> RadiationMultiplier { get; private set; }
        public static ConfigEntry<bool> EnableCold { get; private set; }
        public static ConfigEntry<float> ColdMultiplier { get; private set; }
        public static ConfigEntry<bool> EnableHypoxia { get; private set; }
        public static ConfigEntry<float> HypoxiaMultiplier { get; private set; }
        public static ConfigEntry<bool> EnableFracture { get; private set; }
        public static ConfigEntry<float> FractureMultiplier { get; private set; }
        public static ConfigEntry<bool> EnablePoison { get; private set; }
        public static ConfigEntry<float> PoisonMultiplier { get; private set; }

        public static void Bind(ConfigFile config)
        {
            
            WsUrl = config.Bind("1. Network", "ServerAddress", "ws://127.0.0.1:9999", "备用中继");
            OverrideLocalIp = config.Bind("1. Network", "OverrideLocalIP", "", "手动 IP");

            
            IsProfessionalMode = config.Bind("2. Modes", "IsProfessionalMode", false, "专业模式开关");
            StrengthControlMode = config.Bind("2. Modes", "StrengthControlMode", "FollowClient", "强度控制逻辑");
            UseAbsoluteValue = config.Bind("2. Modes", "UseAbsoluteValue", false, "滑块显示刻度");
            GlobalMultiplier = config.Bind("2. Modes", "GlobalMultiplier", 1.0f, "全局放电倍率 (对所有输出强度统一进行缩放)");
            EnableModClamp = config.Bind("2. Modes", "EnableModClamp", true, "启用 Mod 端绝对限幅保护");
            ModClampA = config.Bind("2. Modes", "ModClampA", 20f, "A 通道绝对限幅 (无论手机怎么设，均强制卡死在此数值内)");
            ModClampB = config.Bind("2. Modes", "ModClampB", 20f, "B 通道绝对限幅 (无论手机怎么设，均强制卡死在此数值内)");

            
            EnableInstant = config.Bind("3. Features", "EnableInstantShock", true, "瞬间反馈");
            EnableContinuous = config.Bind("3. Features", "EnableContinuousFeedback", true, "持续反馈");
            InstantChannel = config.Bind("3. Features", "InstantChannel", "A", "瞬间通道映射");
            ContinuousChannel = config.Bind("3. Features", "ContinuousChannel", "B", "持续通道映射");

            
            MaxStrengthA = config.Bind("4. Safety Clamps", "MaxStrengthA", 20f, "A 通道上限配置");
            MaxStrengthB = config.Bind("4. Safety Clamps", "MaxStrengthB", 15f, "B 通道上限配置");

            
            DamageMultiplier = config.Bind("5. Tuning", "DamageMultiplier", 1.5f, "失血伤害倍率");
            MinHeartRateToShock = config.Bind("5. Tuning", "MinHeartRateToShock", 100f, "背景触发最低心率");

            
            BloodLossWaveform = config.Bind("6. Waveforms", "BloodLossWaveform", "HeavyShock", "瞬间失血波形");
            PainWaveform = config.Bind("6. Waveforms", "PainWaveform", "Sting", "瞬间剧痛波形");
            HeartRateWaveform = config.Bind("6. Waveforms", "HeartRateWaveform", "Heartbeat", "持续心率波形");
            AdrenalineWaveform = config.Bind("6. Waveforms", "AdrenalineWaveform", "SoftBuzz", "背景肾上腺素波形");
            RadiationWaveform = config.Bind("6. Waveforms", "RadiationWaveform", "SoftBuzz", "持续辐射波形");
            ColdWaveform = config.Bind("6. Waveforms", "ColdWaveform", "Sting", "持续失温波形");
            HypoxiaWaveform = config.Bind("6. Waveforms", "HypoxiaWaveform", "HeavyShock", "持续缺氧波形");
            FractureWaveform = config.Bind("6. Waveforms", "FractureWaveform", "HeavyShock", "肢体受伤骨折波形");
            PoisonWaveform = config.Bind("6. Waveforms", "PoisonWaveform", "SoftBuzz", "持续中毒波形");

            
            EnableRadiation = config.Bind("7. Advanced Debuffs", "EnableRadiation", true, "开启辐射病联动反馈");
            RadiationMultiplier = config.Bind("7. Advanced Debuffs", "RadiationMultiplier", 1.0f, "辐射持续放电权重系数");
            EnableCold = config.Bind("7. Advanced Debuffs", "EnableCold", true, "开启失温症联动反馈");
            ColdMultiplier = config.Bind("7. Advanced Debuffs", "ColdMultiplier", 1.2f, "失温持续放电权重系数");
            EnableHypoxia = config.Bind("7. Advanced Debuffs", "EnableHypoxia", true, "开启缺氧/窒息联动反馈");
            HypoxiaMultiplier = config.Bind("7. Advanced Debuffs", "HypoxiaMultiplier", 1.5f, "缺氧持续放电权重系数");
            EnableFracture = config.Bind("7. Advanced Debuffs", "EnableFracture", true, "开启骨折/脱臼状态反馈");
            FractureMultiplier = config.Bind("7. Advanced Debuffs", "FractureMultiplier", 1.2f, "骨折肢体放电权重系数");
            EnablePoison = config.Bind("7. Advanced Debuffs", "EnablePoison", true, "开启中毒/毒液反馈");
            PoisonMultiplier = config.Bind("7. Advanced Debuffs", "PoisonMultiplier", 1.0f, "中毒持续放电权重系数");
        }
    }
}