using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

namespace PsychoGarden.Utils
{
    public static class ResolutionHelper
    {
        public static List<Resolution> AvailableResolutions { get; private set; } = new();

        public static void PopulateResolutionDropdown(TMP_Dropdown dropdown, out int currentResolutionIndex)
        {
            dropdown.ClearOptions();
            AvailableResolutions.Clear();

            Resolution[] allResolutions = Screen.resolutions;
            HashSet<string> uniqueResolutions = new();
            List<string> options = new();

            currentResolutionIndex = 0;

            int maxWidth = Display.main.systemWidth;
            int maxHeight = Display.main.systemHeight;

            for (int i = 0; i < allResolutions.Length; i++)
            {
                Resolution res = allResolutions[i];

                // Check 16:9 aspect
                float aspect = (float)res.width / res.height;
                if (!Mathf.Approximately(aspect, 16f / 9f))
                    continue;

                // Limit to current display bounds
                if (res.width > maxWidth || res.height > maxHeight)
                    continue;

                string resString = $"{res.width} x {res.height}";
                if (uniqueResolutions.Contains(resString))
                    continue;

                uniqueResolutions.Add(resString);
                AvailableResolutions.Add(res);
                options.Add(resString);

                if (res.width == Screen.currentResolution.width &&
                    res.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.Count - 1;
                }
            }

            dropdown.AddOptions(options);
        }

        public static Resolution GetResolutionByIndex(int index)
        {
            if (index < 0 || index >= AvailableResolutions.Count)
                return Screen.currentResolution;

            return AvailableResolutions[index];
        }
    }

    public static class FrameRateHelper
    {
        private static readonly List<int> s_supportedRates = new();

        public static IReadOnlyList<int> SupportedRates => s_supportedRates;

        /// <summary>
        /// Popola la lista di frame rate in base al refresh rate massimo del display.
        /// </summary>
        public static List<string> GetAvailableOptions()
        {
            s_supportedRates.Clear();
            List<string> options = new();

            int maxRefresh = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);

            AddRate(30, maxRefresh, options);
            AddRate(60, maxRefresh, options);
            AddRate(120, maxRefresh, options);
            AddRate(144, maxRefresh, options);

            s_supportedRates.Add(-1);
            options.Add("Unlimited");

            return options;
        }

        private static void AddRate(int rate, int max, List<string> options)
        {
            if (rate <= max)
            {
                s_supportedRates.Add(rate);
                options.Add(rate.ToString());
            }
        }

        public static int IndexToRate(int index)
        {
            if (index < 0 || index >= s_supportedRates.Count)
                return -1;

            return s_supportedRates[index];
        }

        public static int RateToIndex(int rate)
        {
            return s_supportedRates.IndexOf(rate);
        }
    }

    public static class AudioHelper
    {
        public static float NormalizedToDecibels(float normalized)
        {
            // Clamp first
            normalized = Mathf.Clamp01(normalized);

            // Convert to decibels, log scale (-80dB = mute, 0dB = full volume)
            return normalized > 0f ? Mathf.Lerp(-80f, 0f, Mathf.Log10(normalized * 9 + 1)) : -80f;
        }
    }
}
