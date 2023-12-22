using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using zFramework.Anim;
using zFramework.UI;

namespace zFramework.Example
{
    public class NotificationPanel : MonoBehaviour, IBlockable
    {
        public Text title;
        public Text content;
        public Button confirmButton;
        public Button cancelButton;

        public async UniTask<int> ShowAsync(string title, string content)
        {
            this.title.text = title;
            this.content.text = content;
            // reset panel 
            transform.localScale = Vector3.one * 0.1f;
            gameObject.SetActive(true);
            await this.BlockAsync(Color.black, 0.8f, 0.5f, 0.3f); // must blocker first
            await transform.DoScaleAsync(Vector3.one, 0.5f, Ease.OutBack);

            var index = await UniTask.WhenAny(confirmButton.OnClickAsync(), cancelButton.OnClickAsync());
            await transform.DoScaleAsync(Vector3.one * 0.1f, 0.5f, Ease.InBack);
            gameObject.SetActive(false);
            _ = this.UnblockAsync(0.5f);
            return index; // result should never be wait
        }

        public async UniTask<bool> HandleBlockClickedAsync()
        {
            await transform.DoShackPositionAsync(0.3f, Vector3.one * 20);
            return false;
        }
    }
}