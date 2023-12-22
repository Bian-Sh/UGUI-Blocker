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
            // reset panel 
            transform.localScale = Vector3.one*0.1f;
            gameObject.SetActive(true);
            await this.BlockAsync(Color.black, 0.8f, 0.5f, 0.3f); // must blocker first
            await transform.DoScaleAsync(Vector3.one, 0.5f,Ease.OutBack);

            this.title.text = title;
            this.content.text = content;
            var index = await UniTask.WhenAny(confirmButton.OnClickAsync(), cancelButton.OnClickAsync());
            await this.UnblockAsync(0.5f);
            gameObject.SetActive(false);
            return index;
        }

        public async UniTask<bool> HandleBlockClickedAsync()
        {
            await transform.DoShackPositionAsync(0.3f, Vector3.one);
            return false;
        }
    }
}