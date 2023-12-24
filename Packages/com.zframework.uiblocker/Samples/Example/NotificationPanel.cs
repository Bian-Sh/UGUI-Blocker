using System;
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
            transform.localScale = Vector3.one * 0.1f;
            gameObject.SetActive(true);
            // must blocker first, other wise you may click the other button before the panel fadein
            // delay 0.1f means wait for panel show about 0.1f then blocker start fadein
            // You set the block fade-in duration to 0.3f and delay to 0.1f, so the blocker will appear along with the panel suddenly.
            if (useBlocker) await this.BlockAsync(Color.black, 0.8f, 0.3f, 0.1f);

            await transform.DoScaleAsync(Vector3.one, 0.5f, Ease.OutBack);
            var index = await TaskExtension.WhenAny(confirmButton.OnClickAsync(cts.Token), cancelButton.OnClickAsync(cts.Token));
            // if you want blocker fadeout along with panel , you should use "_= " to make them run in parallel
            _ = transform.DoScaleAsync(Vector3.one * 0.01f, 0.5f, Ease.InBack);
            //If the panel fadeout duration is less than that of the blocker, the blocker will fade out first and then the panel will suddenly become inactive.
            // so that blocker fadeout duration should same to panel fadeout duration
            await this.UnblockAsync(0.5f);
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