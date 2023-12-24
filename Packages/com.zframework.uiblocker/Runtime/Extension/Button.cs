using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

public static class ButtonExtension
{
    public static Task OnClickAsync(this Button button, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();
        cancellationToken.Register(() => tcs.TrySetCanceled(), false);
        button.onClick.AddListener(() =>
        {
            tcs.TrySetResult(true);
        });
        return tcs.Task;
    }
}
