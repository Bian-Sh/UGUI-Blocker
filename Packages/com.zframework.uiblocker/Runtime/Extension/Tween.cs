using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace zFramework.Ex
{
    // 实现 Image.DoFadeAsync
    // 实现 Transform.DOShakePosition
    // 实现 Transform.DoScaleAsync
    public static class TweenExtension
    {
        public static async Task DoFadeAsync(this Image image, float endValue, float duration, Ease ease)
        {
            // duration 为 0 时，直接设置为 endValue
            if (duration == 0)
            {
                var color = image.color;
                color[3] = endValue;
                image.color = color;
                return;
            }

            var startValue = image.color.a;
            var time = 0f;
            var func = Easing.Get(ease);
            while (time < duration && image)
            {
                time += Time.deltaTime;
                var t = func(time / duration);
                var color = image.color;
                color[3] = Mathf.Lerp(startValue, endValue, t);
                image.color = color;
                await Task.Yield();
            }
        }
        public static async Task DoScaleAsync(this Transform target, Vector3 endValue, float duration, Ease ease)
        {
            // duration 为 0 时，直接设置为 endValue
            if (duration == 0)
            {
                target.localScale = endValue;
                return;
            }

            var startValue = target.localScale;
            var time = 0f;
            var func = Easing.Get(ease);
            while (time < duration && target)
            {
                time += Time.deltaTime;
                var t = func(time / duration);
                target.localScale = Vector3.Lerp(startValue, endValue, t);
                await Task.Yield();
            }
        }

        public static async Task DoShackPositionAsync(this Transform target, float duration, Vector3 strength, int vibrato = 50, Space space = Space.Self)
        {
            if (duration == 0)
            {
                return;
            }
            var origin = space == Space.Self ? target.localPosition : target.position;
            var time = 0f;
            while (time < duration && target)
            {
                time += Time.deltaTime;
                var randomVector = Random.insideUnitSphere;
                var pos = origin + new Vector3(randomVector.x * strength.x, randomVector.y * strength.y, randomVector.z * strength.z);
                if (space == Space.Self)
                {
                    target.localPosition = Vector3.Lerp(target.localPosition, pos, vibrato * Time.deltaTime);
                }
                else
                {
                    target.position = Vector3.Lerp(target.position, pos, vibrato * Time.deltaTime);
                }
                await Task.Yield();
            }
            if (space == Space.Self)
            {
                target.localPosition = origin;
            }
            else
            {
                target.position = origin;
            }
        }
    }

    public enum Ease
    {
        Linear,
        OutBack,
        InBack,
        InBounce,
        InCirc,
        InCubic,
        InElastic,
        InExpo,
        InQuad,
        InQuart,
        InQuint,
        InSine,
        OutBounce,
        OutCirc,
        OutCubic,
        OutElastic,
        OutExpo,
        OutQuad,
        OutQuart,
        OutQuint,
        OutSine,
        InOutBack,
        InOutBounce,
        InOutCirc,
        InOutCubic,
        InOutElastic,
        InOutExpo,
        InOutQuad,
        InOutQuart,
        InOutQuint,
        InOutSine,
    }
    public delegate float EasingFunction(float t);
    // reference :https://github.com/setchi/EasingCore/blob/master/EasingCore.cs
    public static class Easing
    {
        public static EasingFunction Get(Ease type)
        {
            return type switch
            {
                Ease.Linear => linear,
                Ease.InBack => inBack,
                Ease.InBounce => inBounce,
                Ease.InCirc => inCirc,
                Ease.InCubic => inCubic,
                Ease.InElastic => inElastic,
                Ease.InExpo => inExpo,
                Ease.InQuad => inQuad,
                Ease.InQuart => inQuart,
                Ease.InQuint => inQuint,
                Ease.InSine => inSine,
                Ease.OutBack => outBack,
                Ease.OutBounce => outBounce,
                Ease.OutCirc => outCirc,
                Ease.OutCubic => outCubic,
                Ease.OutElastic => outElastic,
                Ease.OutExpo => outExpo,
                Ease.OutQuad => outQuad,
                Ease.OutQuart => outQuart,
                Ease.OutQuint => outQuint,
                Ease.OutSine => outSine,
                Ease.InOutBack => inOutBack,
                Ease.InOutBounce => inOutBounce,
                Ease.InOutCirc => inOutCirc,
                Ease.InOutCubic => inOutCubic,
                Ease.InOutElastic => inOutElastic,
                Ease.InOutExpo => inOutExpo,
                Ease.InOutQuad => inOutQuad,
                Ease.InOutQuart => inOutQuart,
                Ease.InOutQuint => inOutQuint,
                Ease.InOutSine => inOutSine,
                _ => linear,
            };
            float linear(float t) => t;
            float inBack(float t) => t * t * t - t * Mathf.Sin(t * Mathf.PI);
            float outBack(float t) => 1f - inBack(1f - t);
            float inOutBack(float t) =>
                t < 0.5f
                    ? 0.5f * inBack(2f * t)
                    : 0.5f * outBack(2f * t - 1f) + 0.5f;
            float inBounce(float t) => 1f - outBounce(1f - t);
            float outBounce(float t) =>
                t < 4f / 11.0f ?
                    (121f * t * t) / 16.0f :
                t < 8f / 11.0f ?
                    (363f / 40.0f * t * t) - (99f / 10.0f * t) + 17f / 5.0f :
                t < 9f / 10.0f ?
                    (4356f / 361.0f * t * t) - (35442f / 1805.0f * t) + 16061f / 1805.0f :
                    (54f / 5.0f * t * t) - (513f / 25.0f * t) + 268f / 25.0f;

            float inOutBounce(float t) =>
                t < 0.5f
                    ? 0.5f * inBounce(2f * t)
                    : 0.5f * outBounce(2f * t - 1f) + 0.5f;

            float inCirc(float t) => 1f - Mathf.Sqrt(1f - (t * t));
            float outCirc(float t) => Mathf.Sqrt((2f - t) * t);
            float inOutCirc(float t) =>
                t < 0.5f
                    ? 0.5f * (1 - Mathf.Sqrt(1f - 4f * (t * t)))
                    : 0.5f * (Mathf.Sqrt(-((2f * t) - 3f) * ((2f * t) - 1f)) + 1f);

            float inCubic(float t) => t * t * t;
            float outCubic(float t) => inCubic(t - 1f) + 1f;
            float inOutCubic(float t) =>
                t < 0.5f
                    ? 4f * t * t * t
                    : 0.5f * inCubic(2f * t - 2f) + 1f;

            float inElastic(float t) => Mathf.Sin(13f * (Mathf.PI * 0.5f) * t) * Mathf.Pow(2f, 10f * (t - 1f));
            float outElastic(float t) => Mathf.Sin(-13f * (Mathf.PI * 0.5f) * (t + 1)) * Mathf.Pow(2f, -10f * t) + 1f;
            float inOutElastic(float t) =>
                t < 0.5f
                    ? 0.5f * Mathf.Sin(13f * (Mathf.PI * 0.5f) * (2f * t)) * Mathf.Pow(2f, 10f * ((2f * t) - 1f))
                    : 0.5f * (Mathf.Sin(-13f * (Mathf.PI * 0.5f) * ((2f * t - 1f) + 1f)) * Mathf.Pow(2f, -10f * (2f * t - 1f)) + 2f);

            float inExpo(float t) => Mathf.Approximately(0.0f, t) ? t : Mathf.Pow(2f, 10f * (t - 1f));
            float outExpo(float t) => Mathf.Approximately(1.0f, t) ? t : 1f - Mathf.Pow(2f, -10f * t);
            float inOutExpo(float v) =>
                Mathf.Approximately(0.0f, v) || Mathf.Approximately(1.0f, v)
                    ? v
                    : v < 0.5f
                        ? 0.5f * Mathf.Pow(2f, (20f * v) - 10f)
                        : -0.5f * Mathf.Pow(2f, (-20f * v) + 10f) + 1f;

            float inQuad(float t) => t * t;
            float outQuad(float t) => -t * (t - 2f);
            float inOutQuad(float t) =>
                t < 0.5f
                    ? 2f * t * t
                    : -2f * t * t + 4f * t - 1f;

            float inQuart(float t) => t * t * t * t;
            float outQuart(float t)
            {
                var u = t - 1f;
                return u * u * u * (1f - t) + 1f;
            }

            float inOutQuart(float t) =>
                t < 0.5f
                    ? 8f * inQuart(t)
                    : -8f * inQuart(t - 1f) + 1f;

            float inQuint(float t) => t * t * t * t * t;

            float outQuint(float t) => inQuint(t - 1f) + 1f;

            float inOutQuint(float t) =>
                t < 0.5f
                    ? 16f * inQuint(t)
                    : 0.5f * inQuint(2f * t - 2f) + 1f;

            float inSine(float t) => Mathf.Sin((t - 1f) * (Mathf.PI * 0.5f)) + 1f;
            float outSine(float t) => Mathf.Sin(t * (Mathf.PI * 0.5f));
            float inOutSine(float t) => 0.5f * (1f - Mathf.Cos(t * Mathf.PI));
        }
    }
}