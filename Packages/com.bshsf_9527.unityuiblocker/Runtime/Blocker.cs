using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.ComponentModel.Design.Serialization;
using Codice.Client.BaseCommands;
using Object = UnityEngine.Object;
using Cysharp.Threading.Tasks;

namespace zFramework.UI
{
    public class Blocker
    {
        // 支持 Blocker 嵌套
        private static Dictionary<IBlockable, Blocker> blockers = new();
        [RuntimeInitializeOnLoadMethod]
        private static void Init() => UnityEngine.SceneManagement.SceneManager.sceneLoaded += (_, _) => blockers?.Clear();

        private IBlockable target;
        private Button button;
        private Image background;
        private Canvas innercanvas;
        private Canvas rootCanvas;
        private GraphicRaycaster raycaster;

        private float alpha = 0.5f;
        private Color color = Color.clear;
        private bool canCloseTarget = false;
        private GameObject blocker;

        private bool IsFadeout => alpha > 0; // we need fadeout if user useto fadein

        public Blocker(IBlockable target, Color color, bool canCloseTarget)
        {
            this.target = target;
            this.color = color;
            this.canCloseTarget = canCloseTarget;
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
            raycaster = go.gameObject.AddComponent<GraphicRaycaster>();
            innercanvas.overrideSorting = true;
            innercanvas.sortingOrder = 25000 + blockers.Count;
            
            // 5. Set the sorting layer of blocker's Canvas to be Lower just one unit than the target panel's Canvas.
            canvas.sortingLayerID = innercanvas.sortingLayerID;
            canvas.sortingOrder = innercanvas.sortingOrder - 1;
            background = blocker.AddComponent<Image>();
            color.a = 0f;
            background.color = color;
            button = blocker.AddComponent<Button>();
            button.onClick.AddListener(OnButtonClicked);
            blocker.hideFlags = HideFlags.HideInHierarchy;
        }

        private async void OnButtonClicked()
        {
            if (canCloseTarget)
            {
                await target.CloseAsync();
                Destroy();
            }
        }

        /// <summary>
        ///  渐显此 Blocker
        /// </summary>
        /// <param name="color">为背景板加入颜色 </param>
        /// <param name="alpha"> 最后的 透明值 </param>
        /// <param name="duration"> 渐显持续时长 </param>
        /// <returns></returns>
        public async UniTask ShowAsync(float alpha, float duration, float delay)
        {
            this.alpha = alpha;
            async void DoDelayFadein()
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
                await background.DOFade(alpha, duration).SetEase(Ease.InOutQuad);
            }
            if (delay > 0)
            {
                // 可以通过 delay 实现 Blocker 和 Target Panel 的显示先后顺序
                DoDelayFadein();
            }
            else
            {
                // 必须等到 blocker 展示出来后才能展示 panel
                await background.DOFade(alpha, duration).SetEase(Ease.InOutQuad);
            }
        }

        /// <summary>
        ///  渐隐并销毁此 Blocker 
        /// </summary>
        /// <param name="duration"> 渐隐持续时长 </param>
        /// <returns></returns>
        public async UniTask CloseAsync(float duration = 0.5f)
        {
            await background.DOFade(0f, duration).SetEase(Ease.InOutQuad);
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
            }
        }
    }
}