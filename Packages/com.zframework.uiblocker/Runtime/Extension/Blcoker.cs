using UnityEngine;
using zFramework.UI;
using System.Threading.Tasks;

// 小技巧：
// 1. color = Color.clear 时，Blocker 完全透明
// 2. alpha = 0 时，Blocker 完全透明
// 3. duration = 0 时，Blocker 的 Color 设置立即生效
// 4. delay = 0 时，你会看到先显示 Blocker 再显示 Target Panel
// 5. delay >0 时，你可以通过控制 delay 来控制 Blocker 和 Target Panel 的显示先后顺序
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
        /// <param name="delay">阻塞器的延迟时间</param>
        /// <returns></returns>
        public static async Task BlockAsync(this IBlockable blockable, Color color, float alpha, float duration, float delay)
        {
            var blocker = new Blocker(blockable, color);
            await blocker.ShowAsync(alpha, duration, delay);
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
                await blocker.CloseAsync(duration);
            }
            else
            {
                Debug.LogWarning($"Blocker of {blockable} is not exist");
            }
        }
    }
}