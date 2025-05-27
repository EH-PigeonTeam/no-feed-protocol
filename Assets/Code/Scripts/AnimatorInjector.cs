using System.Collections.Generic;
using UnityEngine;

namespace NoFeedProtocol.Shared.Utilities
{
    /// <summary>
    /// Utility to inject animation clip overrides into an Animator Controller.
    /// </summary>
    public static class AnimatorInjector
    {
        private const string ANIMATOR_INJECTOR_TAG = "[<color=yellow>AnimatorInjector</color>]";

        /// <summary>
        /// Injects animation clip overrides into the given controller.
        /// </summary>
        /// <param name="baseController">Animator controller (can be override or base).</param>
        /// <param name="overrides">Dictionary of state-name to AnimationClip overrides.</param>
        /// <returns>AnimatorOverrideController with applied overrides.</returns>
        public static AnimatorOverrideController InjectOverrides(
            RuntimeAnimatorController baseController,
            Dictionary<string, AnimationClip> overrides)
        {
            var sourceController = baseController is AnimatorOverrideController ovr
                ? ovr.runtimeAnimatorController
                : baseController;

            var overrideController = new AnimatorOverrideController(sourceController);

            var overrideList = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            overrideController.GetOverrides(overrideList);

            Debug.Log($"{ANIMATOR_INJECTOR_TAG} Total override targets: {overrideList.Count}");

            for (int i = 0; i < overrideList.Count; i++)
            {
                var originalClip = overrideList[i].Key;

                if (originalClip == null)
                {
                    Debug.LogWarning($"{ANIMATOR_INJECTOR_TAG} Null clip in controller override list at index {i}.");
                    continue;
                }

                var name = originalClip.name;

                if (overrides.TryGetValue(name, out var newClip))
                {
                    if (newClip != null)
                    {
                        Debug.Log($"{ANIMATOR_INJECTOR_TAG} Overriding '{name}' with '{newClip.name}'");
                        overrideList[i] = new KeyValuePair<AnimationClip, AnimationClip>(originalClip, newClip);
                    }
                    else
                    {
                        Debug.LogWarning($"{ANIMATOR_INJECTOR_TAG} New clip for '{name}' is null.");
                    }
                }
                else
                {
                    Debug.Log($"{ANIMATOR_INJECTOR_TAG} No override found for '{name}' — keeping original.");
                }
            }

            overrideController.ApplyOverrides(overrideList);
            return overrideController;
        }
    }
}
