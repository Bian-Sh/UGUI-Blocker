using System;
using UnityEngine;
using UnityEngine.UI;
using zFramework.Example;

public class PanelController : MonoBehaviour
{
    public Button button;
    public Button button2;
    public NotificationPanel panel;
    public NotificationPanel panel2;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
        button2.onClick.AddListener(OnClick2);
    }

    // Open a panel which is not blocked by blocker
    private async void OnClick2()
    {
        if (!panel2.gameObject.activeSelf)
        {
            var title = "Panel without a Blocker";
            var content = "This panel will be overlaid by other UI as it does not use a blocker！";
            var idx = await panel2.ShowAsync(title, content);

            Debug.Log("user selected : " +idx + (idx == 0 ? "确定" : (idx == -1 ? "用户取消操作" : "取消")));
        }
    }

    // Open a panel which is blocked by blocker
    public async void OnClick()
    {
        if (!panel.gameObject.activeSelf)
        {
            var title = "Panel with a Blocker";
            var content = "This panel will be rendered on the top layer！";
            var idx = await panel.ShowAsync(title, content);
            Debug.Log("user selected : " + idx + (idx == 0 ? "确定" : (idx == -1 ? "用户取消操作" : "取消")));
        }
    }
}
