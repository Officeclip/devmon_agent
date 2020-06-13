using System.Threading.Tasks;
using WUApiLib;

namespace devmon_library.Core
{
    internal sealed class UpdateSearcherCallback : ISearchCompletedCallback
    {
        private readonly IUpdateSearcher _updateSearcher;
        private readonly TaskCompletionSource<ISearchResult> _taskCompletionSource;

        public UpdateSearcherCallback(IUpdateSearcher updateSearcher, TaskCompletionSource<ISearchResult> taskCompletionSource)
        {
            _updateSearcher = updateSearcher;
            _taskCompletionSource = taskCompletionSource;
        }

        public void Invoke(ISearchJob searchJob, ISearchCompletedCallbackArgs callbackArgs)
        {
            ISearchResult result = _updateSearcher.EndSearch(searchJob);
            _taskCompletionSource.SetResult(result);
        }
    }
}
