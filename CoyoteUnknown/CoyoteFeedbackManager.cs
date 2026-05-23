using UnityEngine;
using ScavLib.util;

namespace CoyoteUnknown
{
    public class CoyoteFeedbackManager
    {
        private float lastBloodVolume = 100f;
        private float lastAveragePain = 0f;

        public float CurrentShockIntensity { get; private set; } = 0f;
        public float CurrentContinuousIntensity { get; private set; } = 0f;

        private int lastSentA = -1;
        private int lastSentB = -1;
        private bool isFirstInGameFrame = true;

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

            if (GameUtil.TryGetBody(out Body body))
            {

                if (isFirstInGameFrame)
                {
                    lastBloodVolume = body.bloodVolume;
                    lastAveragePain = body.averagePain;
                    CurrentShockIntensity = 0f;
                    CurrentContinuousIntensity = 0f;
                    isFirstInGameFrame = false;
                    return;
                }

                float currentBlood = body.bloodVolume;
                float currentPain = body.averagePain;


                if (CoyoteUnknownPlugin.EnableInstant.Value)
                {
                    float bloodLoss = lastBloodVolume - currentBlood;
                    float painGain = currentPain - lastAveragePain;
                    float instantTrigger = 0f;

                    if (bloodLoss > 0.05f)
                    {
                        instantTrigger += bloodLoss * CoyoteUnknownPlugin.DamageMultiplier.Value;
                    }
                    if (painGain > 1.0f)
                    {
                        instantTrigger += painGain * 0.4f;
                    }

                    if (instantTrigger > 0.5f)
                    {
                        string instChan = CoyoteUnknownPlugin.InstantChannel.Value.ToUpper();
                        float maxLimit = (instChan == "B") ? CoyoteUnknownPlugin.MaxStrengthB.Value : CoyoteUnknownPlugin.MaxStrengthA.Value;

                        CurrentShockIntensity = Mathf.Min(maxLimit, CurrentShockIntensity + instantTrigger);
                    }
                }


                if (CurrentShockIntensity > 0f)
                {
                    CurrentShockIntensity = Mathf.MoveTowards(CurrentShockIntensity, 0f, Time.deltaTime * 35f);
                }


                float targetContinuous = 0f;
                if (CoyoteUnknownPlugin.EnableContinuous.Value)
                {
                    float hr = body.heartRate;
                    float hrThreshold = CoyoteUnknownPlugin.MinHeartRateToShock.Value;
                    float maxHR = 200f;
                    float heartFactor = 0f;

                    if (hr > hrThreshold)
                    {
                        heartFactor = Mathf.Clamp01((hr - hrThreshold) / (maxHR - hrThreshold));
                    }

                    float painFactor = Mathf.Clamp01(currentPain / 100f);
                    float adrenalineFactor = Mathf.Clamp01(body.curAdrenaline / 100f);


                    float combinedStress = (heartFactor * 0.6f) + (painFactor * 0.3f) + (adrenalineFactor * 0.1f);

                    string contChan = CoyoteUnknownPlugin.ContinuousChannel.Value.ToUpper();
                    float maxLimit = (contChan == "A") ? CoyoteUnknownPlugin.MaxStrengthA.Value : CoyoteUnknownPlugin.MaxStrengthB.Value;

                    targetContinuous = combinedStress * maxLimit;
                }


                CurrentContinuousIntensity = Mathf.Lerp(CurrentContinuousIntensity, targetContinuous, Time.deltaTime * 4f);


                string instChannel = CoyoteUnknownPlugin.InstantChannel.Value.ToUpper();
                string contChannel = CoyoteUnknownPlugin.ContinuousChannel.Value.ToUpper();

                float outA = 0f;
                float outB = 0f;

                if (instChannel == "A") outA += CurrentShockIntensity;
                if (instChannel == "B") outB += CurrentShockIntensity;

                if (contChannel == "A") outA += CurrentContinuousIntensity;
                if (contChannel == "B") outB += CurrentContinuousIntensity;


                int finalA = Mathf.Clamp(Mathf.RoundToInt(outA), 0, Mathf.RoundToInt(CoyoteUnknownPlugin.MaxStrengthA.Value));
                int finalB = Mathf.Clamp(Mathf.RoundToInt(outB), 0, Mathf.RoundToInt(CoyoteUnknownPlugin.MaxStrengthB.Value));


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
            CurrentShockIntensity = 0f;
            CurrentContinuousIntensity = 0f;
            lastSentA = 0;
            lastSentB = 0;
            server?.SetAppStrength("A", 0);
            server?.SetAppStrength("B", 0);
        }
    }
}