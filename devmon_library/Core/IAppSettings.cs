namespace devmon_library.Core
{
    public interface IAppSettings
    {
        object this[string key] { get; }
    }
}
