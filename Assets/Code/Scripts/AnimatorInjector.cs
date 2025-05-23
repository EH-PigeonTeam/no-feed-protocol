using System.Collections.Generic;
using UnityEngine;

namespace NoFeedProtocol.Shared.Utilities
{
    public static class AnimatorInjector
    {
        /// <summary>
        /// Makes the animator override controller.
        /// </summary>
        /// <param name="baseController">The base controller.</param>
        /// <param name="overrides">Dictionary of overrides.</param>
        /// <returns>The animator override controller.</returns>
        public static AnimatorOverrideController InjectOverrides(
            RuntimeAnimatorController baseController,
            Dictionary<string, AnimationClip> overrides)
        {
            var overrideController = new AnimatorOverrideController(baseController);

            foreach (var entry in overrides)
            {
                overrideController[entry.Key] = entry.Value;
            }

            return overrideController;
        }
    }
}
