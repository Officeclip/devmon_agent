using System.Threading;

namespace devmon_library.Core
{
    public interface ICancellation
    {
        CancellationToken Token { get; }
        void Cancel();
    }
}
