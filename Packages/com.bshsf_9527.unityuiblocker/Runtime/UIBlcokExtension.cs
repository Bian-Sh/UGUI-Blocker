using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITASK_EABLED
using Task = Cysharp.Threading.Tasks.UniTask;
#else
using System.Threading.Tasks;
#endif

// 小技巧：
// 1. color = Color.clear 时，Blocker 完全透明
// 2. alpha = 0 时，Blocker 完全透明
// 3. duration = 0 时，Blocker 的 Color 设置立即生效
// 4. delay = 0 时，你会看到先显示 Blocker 再显示 Target Panel
// 5. delay >0 时，你可以通过控制 delay 来控制 Blocker 和 Target Panel 的显示先后顺序
namespace zFramework.UI
{
    public static class UIBlockExtension
    {

        public static Task BlockAsync(this IBlockable blockable, string message = null)
        {

            return Task.CompletedTask;
        }

    }
}