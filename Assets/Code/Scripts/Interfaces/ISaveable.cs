namespace NoFeedProtocol.Runtime.Save
{
    public interface ISaveable<T>
    {
        T ToSaveData();
    }
}
