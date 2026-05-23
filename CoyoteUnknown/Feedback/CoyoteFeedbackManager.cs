using UnityEngine;
using ScavLib.util;
using CoyoteUnknown.Config;
using CoyoteUnknown.Network;

namespace CoyoteUnknown.Feedback
{
    public class CoyoteFeedbackManager
    {
        private float lastBloodVolume = 100f;
        private float lastAveragePain = 0f;

        public float CurrentContinuousIntensity { get; private set; } = 0f;

        private int lastSentA = -1;
        private int lastSentB = -1;
        private bool isFirstInGameFrame = true;
        private float pulseCooldownTimer = 0f;

        public void UpdateFeedback(DGLabServer server)
        {
            if (server == null || !server.IsBound) return;

            if (!GameUtil.IsInGame || !GameUtil.IsWorldLoaded)
            {
                if (!isFirstInGameFrame)
                {
                    ResetOutputs(server);
                    isFirstInGameFrame = true;
                }
                return;
            }

            if (pulseCooldownTimer > 0f)
            {
                pulseCooldownTimer -= Time.deltaTime;
            }

            if (GameUtil.TryGetBody(out Body body))
            {
                if (isFirstInGameFrame)
                {
                    lastBloodVolume = body.bloodVolume;
                    lastAveragePain = body.averagePain;
                    CurrentContinuousIntensity = 0f;
                    isFirstInGameFrame = false;
                    return;
                }

                float currentBlood = body.bloodVolume;
                float currentPain = body.averagePain;

                
                if (ModConfig.EnableInstant.Value && pulseCooldownTimer <= 0f)
                {
                    float bloodLoss = lastBloodVolume - currentBlood;
                    float painGain = currentPain - lastAveragePain;

                    if (bloodLoss > 0.05f)
                    {
                        string instChan = ModConfig.InstantChannel.Value.ToUpper();
                        string waveName = ModConfig.BloodLossWaveform.Value;
                        server.SendPulse(instChan, WaveformLibrary.Get(waveName));
                        pulseCooldownTimer = 0.5f;
                    }
                    else if (painGain > 1.0f)
                    {
                        string instChan = ModConfig.InstantChannel.Value.ToUpper();
                        string waveName = ModConfig.PainWaveform.Value;
                        server.SendPulse(instChan, WaveformLibrary.Get(waveName));
                        pulseCooldownTimer = 0.5f;
                    }
                }

                
                float targetContinuous = 0f;
                if (ModConfig.EnableContinuous.Value)
                {
                    
                    float hr = body.heartRate;
                    float hrThreshold = ModConfig.MinHeartRateToShock.Value;
                    float maxHR = 200f;
                    float heartFactor = (hr > hrThreshold) ? Mathf.Clamp01((hr - hrThreshold) / (maxHR - hrThreshold)) : 0f;

                    float painFactor = Mathf.Clamp01(currentPain / 100f);
                    float adrenalineFactor = Mathf.Clamp01(body.curAdrenaline / 100f);

                    float baselineStress = (heartFactor * 0.5f) + (painFactor * 0.3f) + (adrenalineFactor * 0.2f);

                    
                    float radFactor = 0f;
                    if (ModConfig.EnableRadiation.Value)
                    {
                        radFactor = Mathf.Clamp01(body.radiationSickness / 100f) * ModConfig.RadiationMultiplier.Value;
                    }

                    
                    float coldFactor = 0f;
                    if (ModConfig.EnableCold.Value && body.temperature < 35.5f)
                    {
                        coldFactor = Mathf.Clamp01((35.5f - body.temperature) / 8.5f) * ModConfig.ColdMultiplier.Value;
                    }

                    
                    float hypoxiaFactor = 0f;
                    if (ModConfig.EnableHypoxia.Value && body.bloodOxygen < 90f)
                    {
                        hypoxiaFactor = Mathf.Clamp01((90f - body.bloodOxygen) / 40f) * ModConfig.HypoxiaMultiplier.Value;
                    }

                    
                    float poisonFactor = 0f;
                    if (ModConfig.EnablePoison.Value)
                    {
                        poisonFactor = Mathf.Clamp01(body.venomCurrent / 100f) * ModConfig.PoisonMultiplier.Value;
                    }

                    
                    float fractureFactor = 0f;
                    if (ModConfig.EnableFracture.Value && body.limbs != null)
                    {
                        foreach (var limb in body.limbs)
                        {
                            if (limb != null && (limb.broken || limb.dislocated))
                            {
                                fractureFactor = 1.0f * ModConfig.FractureMultiplier.Value; 
                                break;
                            }
                        }
                    }

                    
                    float combinedStress = baselineStress + radFactor + coldFactor + hypoxiaFactor + poisonFactor + fractureFactor;
                    combinedStress = Mathf.Clamp01(combinedStress);

                    string contChan = ModConfig.ContinuousChannel.Value.ToUpper();
                    float maxLimit = (contChan == "A") ? ModConfig.MaxStrengthA.Value : ModConfig.MaxStrengthB.Value;

                    targetContinuous = combinedStress * maxLimit;
                }

                CurrentContinuousIntensity = Mathf.Lerp(CurrentContinuousIntensity, targetContinuous, Time.deltaTime * 4f);

                string contChannel = ModConfig.ContinuousChannel.Value.ToUpper();

                float outA = 0f;
                float outB = 0f;

                if (contChannel == "A") outA += CurrentContinuousIntensity;
                if (contChannel == "B") outB += CurrentContinuousIntensity;

                
                outA *= ModConfig.GlobalMultiplier.Value;
                outB *= ModConfig.GlobalMultiplier.Value;

                
                int finalA = 0;
                int finalB = 0;

                int limitA = server.AppLimitA;
                int limitB = server.AppLimitB;

                if (ModConfig.StrengthControlMode.Value == "FollowClient")
                {
                    float scaleA = ModConfig.MaxStrengthA.Value / 100f;
                    float scaleB = ModConfig.MaxStrengthB.Value / 100f;
                    finalA = Mathf.RoundToInt((outA / Mathf.Max(1f, ModConfig.MaxStrengthA.Value)) * limitA * scaleA);
                    finalB = Mathf.RoundToInt((outB / Mathf.Max(1f, ModConfig.MaxStrengthB.Value)) * limitB * scaleB);
                }
                else
                {
                    finalA = Mathf.Clamp(Mathf.RoundToInt(outA), 0, Mathf.Min(Mathf.RoundToInt(ModConfig.MaxStrengthA.Value), limitA));
                    finalB = Mathf.Clamp(Mathf.RoundToInt(outB), 0, Mathf.Min(Mathf.RoundToInt(ModConfig.MaxStrengthB.Value), limitB));
                }

                
                if (ModConfig.EnableModClamp.Value)
                {
                    finalA = Mathf.Min(finalA, Mathf.RoundToInt(ModConfig.ModClampA.Value));
                    finalB = Mathf.Min(finalB, Mathf.RoundToInt(ModConfig.ModClampB.Value));
                }

                
                if (finalA != lastSentA)
                {
                    server.SetAppStrength("A", finalA);
                    lastSentA = finalA;
                }
                if (finalB != lastSentB)
                {
                    server.SetAppStrength("B", finalB);
                    lastSentB = finalB;
                }

                lastBloodVolume = currentBlood;
                lastAveragePain = currentPain;
            }
        }

        public void ResetOutputs(DGLabServer server)
        {
            CurrentContinuousIntensity = 0f;
            lastSentA = 0;
            lastSentB = 0;
            server?.SetAppStrength("A", 0);
            server?.SetAppStrength("B", 0);
        }
    }
}