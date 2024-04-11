using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using zFramework.Ex;
using zFramework.UI;

namespace zFramework.Example
{
    public class NotificationPanel : MonoBehaviour, IBlockable
    {
        public Text title;
        public Text content;
        public Button confirmButton;
        public Button cancelButton;
        public Toggle toggle;
        private CancellationTokenSource cts;

        private void Start() => toggle.onValueChanged.AddListener((value) => closeByBlock = value);

        public bool closeByBlock = false;
        public bool useBlocker = true;
        public async Task<int> ShowAsync(string title, string content)
        {
            cts = new CancellationTokenSource();
            this.title.text = title;
            this.content.text = content;

            // reset panel 
            transform.localScale = Vector3.one * 0.001f;
            gameObject.SetActive(true);
            // must blocker first, other wise you may click the other button before the panel fadein
            // delay 0.1f means wait 0.1f then blocker start fadein，and blockasync will act as a sync function
            // so by setting fade-in duration to 0.3f and delay to 0.1f, we can see the blocker appear along with the panel instead of appear one after another.
            if (useBlocker) await this.BlockAsync(Color.black, 0.8f, 0.3f, 0.1f);

            await transform.DoScaleAsync(Vector3.one, 0.5f, Ease.OutBack);
            var index = await TaskExtension.WhenAny(confirmButton.OnClickAsync(cts.Token), cancelButton.OnClickAsync(cts.Token));

            // if you want blocker fadeout along with panel , you should use "_= " to make them run in "parallel"
            //The blocker will fade out if its alpha > 0, it will fade out if it was previously faded in. Otherwise, it will be destroyed immediately.
            //The blocker should hide behind the panel after a delay of 0.3 seconds, which is perfect for this case.
            _ = this.UnblockAsync(0.5f);
            await transform.DoScaleAsync(Vector3.one * 0.0001f, 0.3f, Ease.InBack);
            gameObject.SetActive(false);
            cts?.Dispose();
            return index; // result should never be wait 
        }

        public async void HandleBlockClickedAsync()
        {
            if (closeByBlock)
            {
                cts?.Cancel();
            }
            else
            {
                await transform.DoShackPositionAsync(0.3f, Vector3.one * 20);
            }
        }
    }
}