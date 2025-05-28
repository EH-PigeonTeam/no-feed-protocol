#if UNITY_EDITOR
#endif

namespace NoFeedProtocol.Authoring.Characters
{
    public enum TargetStrategy
    {
        RandomSplit,
        PreferLowestHP,
        PreferHighestHP,
        All
    }
}