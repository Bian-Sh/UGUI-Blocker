using System.Threading.Tasks;
namespace zFramework.UI
{
    public interface IBlockable
    {
        /// <summary>
        ///  交由用户处理 Blocker 被点击的事件
        /// </summary>
        void HandleBlockClickedAsync();
    }
}

