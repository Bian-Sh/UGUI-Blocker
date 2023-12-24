using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
namespace zFramework.Ex
{
    public static class ButtonExtension
    {
        public static Task OnClickAsync(this Button button, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => tcs.TrySetCanceled(), false);
            void RegistOnClickedCallback()
            {
                tcs.TrySetResult(true);
                button.onClick.RemoveListener(RegistOnClickedCallback);
            }
            button.onClick.AddListener(RegistOnClickedCallback);
            return tcs.Task;
        }
    }
}
