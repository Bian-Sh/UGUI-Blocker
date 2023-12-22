using UnityEngine;
using UnityEngine.UI;
using zFramework.Example;

public class PanelController : MonoBehaviour
{
    public Button button;
    public NotificationPanel panel;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public async void OnClick()
    {
        Debug.Log("Clicked");
        if (!panel.gameObject.activeSelf)
        {
            var title = "Title";
            var content = "Content 啊！";
            var idx = await panel.ShowAsync(title, content);
            Debug.Log("idx: " + idx);
        }
    }

}
