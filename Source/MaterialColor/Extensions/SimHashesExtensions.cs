using System.Collections.Generic;
using ONI_Common.Json;

namespace MaterialColor.Extensions
{
    using JetBrains.Annotations;

    using Helpers;

    using ONI_Common;
    using ONI_Common.Data;

    using UnityEngine;

    public static class SimHashesExtensions
    {
        public static Color GetMaterialColorForType(this SimHashes material, string objectTypeName)
        {
            if (!ColorHelper.TryGetTypeStandardColor(objectTypeName, out Color32 typeStandardColor))
            {
                if (HarmonyPatches.ConfiguratorState.ShowMissingTypeColorOffsets)
                {
                    Debug.LogError($"Can't find <{objectTypeName}> type color");
                    return typeStandardColor;
                }
            }

            Color32 colorOffsetForWhite = typeStandardColor.TintToWhite();

            if (HarmonyPatches.ConfiguratorState.ShowBuildingsAsWhite)
            {
                return colorOffsetForWhite;
            }

            ElementColorInfo elementColorInfo = material.GetMaterialColorInfo();

            // UnityEngine.Debug.Log("About to multiply - "+ objectTypeName+"-"  + material + "-" + elementColorInfo.ColorMultiplier.Red + "-"+ elementColorInfo.Brightness);
            Color32 multiply      = colorOffsetForWhite.Multiply(elementColorInfo.ColorMultiplier);
            Color32 materialColor = multiply.SetBrightness(elementColorInfo.Brightness);

            return materialColor;
        }
        private static Dictionary<SimHashes, ElementColorInfo> _elementColorInfos;
        [NotNull]
        private static readonly JsonFileLoader JsonLoader = new JsonFileLoader(new JsonManager(), Logger);
        [NotNull]
        public static ONI_Common.IO.Logger Logger => _logger ?? (_logger = new ONI_Common.IO.Logger(Paths.MaterialColorLogFileName));
        private static ONI_Common.IO.Logger _logger;
        public static bool TryReloadElementColorInfos()
        {
            if (!JsonLoader.TryLoadElementColorInfos(out Dictionary<SimHashes, ElementColorInfo> colorInfos))
            {
                return false;
            }

            ElementColorInfos = colorInfos;

            return true;
        }
        [NotNull]
        public static Dictionary<SimHashes, ElementColorInfo> ElementColorInfos
        {
            get
            {
                if (_elementColorInfos != null)
                {
                    return _elementColorInfos;
                }

                // Dictionary<SimHashes, ElementColorInfo> colorInfos;
                JsonLoader.TryLoadElementColorInfos(out _elementColorInfos);

                return _elementColorInfos;
            }

            private set => _elementColorInfos = value;
        }

        [NotNull]
        public static ElementColorInfo GetMaterialColorInfo(this SimHashes materialHash)
        {
            if (ElementColorInfos.TryGetValue(materialHash, out ElementColorInfo elementColorInfo))
            {
                return elementColorInfo;
            }

            if (!HarmonyPatches.ConfiguratorState.ShowMissingElementColorInfos)
            {
                return new ElementColorInfo(Color32Multiplier.One);
            }

            Debug.LogError($"Can't find <{materialHash}> color info");
            return new ElementColorInfo(new Color32Multiplier(1, 0, 1));
        }

        public static Color ToCellMaterialColor(this SimHashes material)
        {
            ElementColorInfo colorInfo = material.GetMaterialColorInfo();

            Color result = new Color(
                                     colorInfo.ColorMultiplier.Red,
                                     colorInfo.ColorMultiplier.Green,
                                     colorInfo.ColorMultiplier.Blue) * colorInfo.Brightness;

            result.a = byte.MaxValue;

            return result;
        }

        public static Color32 ToDebugColor(this SimHashes material)
        {
            Element element = ElementLoader.FindElementByHash(material);

            if (element?.substance != null)
            {
                Color32 debugColor = element.substance.debugColour;

                debugColor.a = byte.MaxValue;

                return debugColor;
            }

            return Color.white;
        }
    }
}