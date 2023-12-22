using Cysharp.Threading.Tasks;

namespace zFramework.UI
{
    public interface IBlockable
    {
        /// <summary>
        ///  交由用户处理 Blocker 被点击的事件
        /// </summary>
        /// <returns>是否关闭 blocker ，ture =关闭</returns>
        UniTask<bool> HandleBlockClickedAsync();
    }
}
