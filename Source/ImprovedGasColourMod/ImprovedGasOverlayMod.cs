﻿using Harmony;
using MaterialColor.Extensions;
using UnityEngine;

namespace ImprovedGasColourMod
{
    public static class HarmonyPatches
    {
        private static readonly Color NotGasColor = new Color(0.6f, 0.6f, 0.6f);

        [HarmonyPatch(typeof(SimDebugView), "GetOxygenMapColour")]
        public static class ImprovedGasOverlayMod
        {
            public const float EarPopFloat = 2.5f;
            public static float GasPressureStart { get; set; } = 0.1f;
            public static float GasPressureEnd
            {
                get
                {
                    return _gasPressureEnd;
                }

                set
                {
                    _gasPressureEnd = _gasPressureEnd <= 0 ? float.Epsilon : value;
                }
            }
            private static float _gasPressureEnd = 2.5f;

            public static bool Prefix(int cell, ref Color __result)
            {
              //  ModSettings settings = ONI_Common.ModdyMcModscreen
                float minMass = GasPressureStart;
                float maxMass = GasPressureEnd;

                Element element = Grid.Element[cell];

                if (!element.IsGas)
                {
                    __result = NotGasColor;
                    return false;
                }

                Color gasColor = GetCellOverlayColor(cell);

                float gasMass = Grid.Mass[cell];

                gasMass -= minMass;

                if (gasMass < 0)
                {
                    gasMass = 0;
                }

                maxMass -= minMass;

                if (maxMass < float.Epsilon)
                {
                    maxMass = float.Epsilon;
                }

                float intensity;
                ColorHSV gasColorHSV = gasColor;
                float mass = Grid.Mass[cell];
                float maxMarker;
                float minMarker;
                    minMarker = SimDebugView.minimumBreathable;
                    maxMarker = SimDebugView.optimallyBreathable;
                if (element.id == SimHashes.Oxygen || element.id == SimHashes.ContaminatedOxygen)
                {

                    // // To red for thin air
                    // if (intensity < 1f)
                    // {
                    //     gasColorHSV.V = Mathf.Min(gasColorHSV.V + 1f - intensity, 0.9f);
                    // }
                }
                else
                {
                    maxMarker *= 2f;

                    //intensity = GetGasColorIntensity(gasMass, maxMass);
                    //intensity = Mathf.Max(intensity, 0.15f);

                }
                intensity = Mathf.Max(0.05f, Mathf.InverseLerp(minMarker, maxMarker, mass));

                // Pop ear drum marker
                if (mass > EarPopFloat)
                {
                    gasColorHSV.H += 0.02f * Mathf.InverseLerp(EarPopFloat, 3.5f, mass);
                    if (gasColorHSV.H > 1f)
                    {
                        gasColorHSV.H -= 1f;
                    }

                    float intens = Mathf.InverseLerp(EarPopFloat, 20f, mass);

                    float modifier = 1f - intens / 2;

                    gasColorHSV.V *= modifier;

                }

                // New code, use the saturation of a color for the pressure
                gasColorHSV.S *= intensity;
                __result = gasColorHSV;

                return false;

                // gasColor *= intensity;
                // gasColor.a = 1;
                // __result = gasColor;
            }

            public static Color GetCellOverlayColor(int cellIndex)
            {
                Element element = Grid.Element[cellIndex];
                Substance substance = element.substance;

                Color32 overlayColor = substance.overlayColour;

                overlayColor.a = byte.MaxValue;

                return overlayColor;
            }
            public static float MinimumGasColorIntensity { get; set; } = 0.25f;

            private static float GetGasColorIntensity(float mass, float maxMass)
            {
                float minIntensity = MinimumGasColorIntensity;

                float intensity = mass / maxMass;

                intensity = Mathf.Sqrt(intensity);

                intensity = Mathf.Clamp01(intensity);
                intensity *= 1 - minIntensity;
                intensity += minIntensity;

                return intensity;
            }
        }
    }
}