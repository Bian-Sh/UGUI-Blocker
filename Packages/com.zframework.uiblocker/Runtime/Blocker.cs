using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using zFramework.Ex;
using System.Threading.Tasks;

namespace zFramework.UI
{
    public class Blocker
    {
        // 支持 Blocker 嵌套
        [RuntimeInitializeOnLoadMethod]
        private static void Init() => UnityEngine.SceneManagement.SceneManager.sceneLoaded += (_, _) => blockers?.Clear();

        readonly IBlockable target;
        readonly Button button;
        readonly Image background;
        readonly Canvas innercanvas;
        readonly Canvas rootCanvas;
        readonly GraphicRaycaster raycaster;

        private float alpha = 0.5f;
        readonly GameObject blocker;
        private bool ShouldFadeout => alpha > 0; // we need fadeout if user useto fadein
        static readonly Dictionary<IBlockable, Blocker> blockers = new();

        public Blocker(IBlockable target, Color color)
        {
            this.target = target;
            blockers.Add(target, this);

            // check target wether its UI or not 
            var go = target as MonoBehaviour;
            var rect = go.GetComponent<RectTransform>();
            if (!go || !rect)
            {
                throw new Exception("target must be a UI component");
            }

            // should not blocked before
            innercanvas = go.GetComponent<Canvas>();
            if (innercanvas && innercanvas.enabled)
            {
                throw new Exception("target should not be blocked before");
            }

            // get target's root canvas
            rootCanvas = go.GetComponentsInParent<Canvas>()
                .Where(c => c.isRootCanvas)
                .FirstOrDefault();
            if (!rootCanvas)
            {
                throw new Exception("target must be in a canvas");
            }

            // 1. Create blocker GameObject.
            blocker = new GameObject("Blocker", typeof(RectTransform));

            // 2. Set blocker's RectTransform properties.
            var rectTransform = blocker.GetComponent<RectTransform>();
            rectTransform.SetParent(rootCanvas.transform, false);
            rectTransform.SetAsLastSibling();
            rectTransform.anchorMin = Vector3.zero;
            rectTransform.anchorMax = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;

            // 3. Add Canvas component.
            Canvas canvas = blocker.AddComponent<Canvas>();
            blocker.AddComponent<GraphicRaycaster>();
            canvas.overrideSorting = true;

            // 4. Add Canvas component for target panel.
            innercanvas = go.gameObject.AddComponent<Canvas>();
            innercanvas.overrideSorting = true;
            innercanvas.sortingOrder = 25000 + blockers.Count;
            raycaster = go.gameObject.AddComponent<GraphicRaycaster>();

            // 5. Set the sorting layer of blocker's Canvas to be Lower just one unit than the target panel's Canvas.
            canvas.sortingLayerID = innercanvas.sortingLayerID;
            canvas.sortingOrder = innercanvas.sortingOrder - 1;
            background = blocker.AddComponent<Image>();
            color.a = 0f;
            background.color = color;
            button = blocker.AddComponent<Button>();
            button.onClick.AddListener(target.HandleBlockClickedAsync);
            blocker.hideFlags = HideFlags.HideInHierarchy;
        }

        /// <summary>
        ///  渐显此 Blocker
        /// </summary>
        /// <param name="color">为背景板加入颜色 </param>
        /// <param name="alpha"> 最后的 透明值 </param>
        /// <param name="duration"> 渐显持续时长 </param>
        /// <returns></returns>
        internal async Task ShowAsync(float alpha, float duration, float delay)
        {
            this.alpha = alpha;
            async void DoDelayFadein()
            {
                await Task.Delay(TimeSpan.FromSeconds(delay));
                await background.DoFadeAsync(alpha, duration, Ease.OutBack);
            }
            if (delay > 0)
            {
                // 可以通过 delay 实现 Blocker 和 Target Panel 的显示先后顺序
                DoDelayFadein();
            }
            else
            {
                // 必须等到 blocker 展示出来后才能展示 panel
                await background.DoFadeAsync(alpha, duration, Ease.OutBack);
            }
        }

        /// <summary>
        ///  渐隐并销毁此 Blocker 
        /// </summary>
        /// <param name="duration"> 渐隐持续时长 </param>
        /// <returns></returns>
        internal async Task CloseAsync(float duration = 0.5f)
        {
            if (ShouldFadeout)
            {
                await background.DoFadeAsync(0f, duration, Ease.InBack);
            }
            Destroy();
        }

        private void Destroy()
        {
            if (blocker)
            {
                //销毁的顺序必须的要对，否则：Can't remove Canvas because GraphicRaycaster (Script) depends on it
                Object.DestroyImmediate(raycaster);
                Object.DestroyImmediate(innercanvas);
                Object.DestroyImmediate(blocker);
                blockers.Remove(target);
            }
        }

        internal static bool TryGet(IBlockable blockable, out Blocker blocker) => blockers.TryGetValue(blockable, value: out blocker);
    }
}