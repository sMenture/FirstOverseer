namespace FirstOverseer.Global.SaveSystem
{
    public interface ISaveable
    {
        string UniqueId { get; }
        object GetSaveData();
        void LoadSaveData(object data);
    }
}