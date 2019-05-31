namespace Models {
    public interface ITxtConfigData<TKey> : IConfigData<TKey> {
        bool FormatText(string line);
    }
}
