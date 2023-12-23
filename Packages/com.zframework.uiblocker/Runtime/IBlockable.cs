using Cysharp.Threading.Tasks;
namespace zFramework.UI
{
    public interface IBlockable
    {
        /// <summary>
        ///  交由用户处理 Blocker 被点击的事件
        ///  如果你的面板中 await 了很多Task，请在关闭前完全取消他们
        /// </summary>
        /// <returns>是否关闭 blocker ，ture =关闭</returns>
        UniTask<Blocker.Context> HandleBlockClickedAsync();

        // 取消 Task 的一个可能性演示
        /*
        public class Foo : MonoBehaviour
        {
            private CancellationTokenSource cts;
            private async UniTask<int> ShowAsync(string title, string content)
            {
                cts = new CancellationTokenSource();
                var index = await UniTask.WhenAny(confirmButton.OnClickAsync(cts.Token), cancelButton.OnClickAsync(cts.Token));
                return index;
            }

            public async UniTask<Blocker.Context> HandleBlockClickedAsync()
            {
                await DoSomethingAsync();
                cts.Cancel();
                return new Blocker.Context { close = true };
            }
        }
         */
    }
}

