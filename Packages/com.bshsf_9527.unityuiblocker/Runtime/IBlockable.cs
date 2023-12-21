#if UNITASK_EABLED
using Task = Cysharp.Threading.Tasks.UniTask;
#else
using System.Threading.Tasks;
#endif

namespace zFramework.UI
{
    public interface IBlockable
    {
        Task CloseAsync();
    }
}
