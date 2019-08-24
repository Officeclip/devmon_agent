using System.Threading;

namespace Geheb.DevMon.Agent.Core
{
    public interface ICancellation
    {
        CancellationToken Token { get; }
        void Cancel();
    }
}
