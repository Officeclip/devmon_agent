namespace Geheb.DevMon.Agent.Core
{
    public interface IAppSettings
    {
        object this[string key] { get; }
    }
}
