using UnityEngine;
using zFramework.UI;
using System.Threading.Tasks;

// 小技巧：
// 1. color = Color.clear 时，Blocker 完全透明
// 2. alpha = 0 时，Blocker 完全透明
// 3. duration = 0 时，Blocker 的 Color 设置立即生效
// 4. delay = 0 时，先显示 Blocker 再显示 Target Panel
// 5. delay >0 时，先显示 Target Panel 后显示 Blocker 
// 6. delay >0 时，等待和 blocker 创建都是异步的， await 反而不会卡整体流程

// 蒙版的显示和隐藏是通过 CanvasGroup 的 BlockRaycast 来控制的，如果你的 IBlockable 对象没有 CanvasGroup 组件，那么会自动添加一个
// 鉴于以上原因，如果你的 IBlockable 对象已经有 CanvasGroup 组件，请确信不会与 Blocker 的 CanvasGroup 处理逻辑冲突
namespace zFramework.Ex
{
    public static class UIBlockExtension
    {
        /// <summary>
        /// 阻塞指定的 IBlockable 对象，并设置阻塞器的颜色、透明度、持续时间和延迟时间。
        /// </summary>
        /// <param name="blockable">要阻塞的对象</param>
        /// <param name="color">阻塞器的颜色</param>
        /// <param name="alpha">阻塞器的透明度</param>
        /// <param name="duration">阻塞器的持续时间</param>
        /// <param name="delay">延迟多久后加载阻塞器，该值</param>
        /// <returns></returns>
        public static async Task BlockAsync(this IBlockable blockable, Color color, float alpha, float duration, float delay)
        {
            var blocker = new Blocker(blockable, color);
            // 如果 blockable 存在 CanvasGroup 组件，则记录 origin block raycast 值，然后设置为 false ，完成后恢复
            var cg = (blockable as MonoBehaviour).GetComponent<CanvasGroup>();
            bool? originBlockRaycast = null;
            if (!cg)
            {
                cg = (blockable as MonoBehaviour).gameObject.AddComponent<CanvasGroup>();
            }
            // 延迟 delay + duration 秒后才能够处理 BlockRaycast
            async void DelayHandleBlockRaycastAsync()
            {
                await Task.Delay((int)((delay + duration) * 1000));
                if (cg)
                {
                    if (originBlockRaycast.HasValue)
                    {
                        cg.blocksRaycasts = originBlockRaycast.Value;
                    }
                    else
                    {
                        Object.Destroy(cg);
                    }
                }
            }
            await blocker.ShowAsync(alpha, duration, delay);
            DelayHandleBlockRaycastAsync();
        }

        /// <summary>
        ///  关闭 Blocker ，此API 不会对调用方 IBlockable 进行任何操作,请自行处理隐匿问题
        /// </summary>
        /// <param name="blockable">调用方</param>
        /// <param name="duration">渐隐的时长</param>
        /// <returns></returns>
        public static async Task UnblockAsync(this IBlockable blockable, float duration)
        {
            if (Blocker.TryGet(blockable, out var blocker))
            {
                // 使用 CanvasGroup 控制 BlockRaycast,避免Blocker 隐藏过程中用户误触面板
                var cg = (blockable as MonoBehaviour).GetComponent<CanvasGroup>();
                bool? originBlockRaycast = null;
                if (!cg)
                {
                    cg = (blockable as MonoBehaviour).gameObject.AddComponent<CanvasGroup>();
                }
                else
                {
                    originBlockRaycast = cg.blocksRaycasts;
                }
                cg.blocksRaycasts = false;
                await blocker.CloseAsync(duration);
                if (originBlockRaycast.HasValue)
                {
                    cg.blocksRaycasts = originBlockRaycast.Value;
                }
                else
                {
                    Object.Destroy(cg);
                }
            }
            else
            {
                Debug.LogWarning($"Blocker of {blockable} is not exist");
            }
        }
    }
}