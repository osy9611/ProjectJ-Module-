namespace Module.Unity.DayNight
{
    using global::Unity.VisualScripting;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class DayNight : MonoBehaviour
    {
        [Range(0, 24)]
        public float TimeOfDay;

        public float OrbitSpeed = 1.0f;
        public Light Sun;
        public Gradient sunColor;
        public Light Moon;
        public Gradient moonColor;

        public Gradient skyColor;
        public Gradient middleColor;
        public Gradient groundColor;

        public AnimationCurve SunBrightness = new AnimationCurve(
             new Keyframe(0, 0.01f),
             new Keyframe(0.15f, 0.01f),
             new Keyframe(0.35f, 1),
             new Keyframe(0.65f, 1),
             new Keyframe(0.85f, 0.01f),
             new Keyframe(1, 0.01f)
         );

        public AnimationCurve MoonBrightness = new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(0.15f, 1),
            new Keyframe(0.35f, 0.01f),
            new Keyframe(0.65f, 0.01f),
            new Keyframe(0.85f, 1),
            new Keyframe(1, 1)
        );

        protected float alpha = 0;
        protected float sunRotation, moonRotation;
        protected float sunIntensity, moonIntensity;

        protected DesignEnum.TimeType timeType;
        protected DesignEnum.TimeType messageCheck;

        public UnityEvent OnMorning;
        public UnityEvent OnNoon;
        public UnityEvent OnNight;
        public UnityEvent OnMidnight;

        public virtual void Execute()
        {
            UpdateTime();
            UpdateRotation();
            UpdateColor();
            UpdateIntensity();
            UpdateTimeType();
        }

        protected virtual void UpdateTime()
        {
            TimeOfDay += Time.deltaTime * OrbitSpeed;
            if (TimeOfDay > 24)
                TimeOfDay = 0;

            alpha = TimeOfDay / 24.0f;
        }

        protected virtual void UpdateRotation()
        {
            sunRotation = Mathf.Lerp(-90, 270, alpha);
            moonRotation = sunRotation - 180;

            Sun.transform.rotation = Quaternion.Euler(sunRotation, 150, 0);
            Moon.transform.rotation = Quaternion.Euler(moonRotation, 150, 0);
        }

        protected virtual void UpdateIntensity()
        {
            sunIntensity = Vector3.Dot(Sun.transform.forward, Vector3.down);
            sunIntensity = Mathf.Clamp01(sunIntensity);
            moonIntensity = Vector3.Dot(Moon.transform.forward, Vector3.down);
            moonIntensity = Mathf.Clamp01(moonIntensity);
        }

        protected virtual void UpdateColor()
        {
            Sun.intensity = SunBrightness.Evaluate(alpha);
            Moon.intensity = MoonBrightness.Evaluate(alpha);

            Sun.color = sunColor.Evaluate(sunIntensity);
            Moon.color = moonColor.Evaluate(moonIntensity);

            RenderSettings.skybox.SetVector("_SunDir", -Sun.transform.forward);
            RenderSettings.skybox.SetVector("_MoonDir", -Moon.transform.forward);

            RenderSettings.skybox.SetColor("_SunColor", Sun.color);
            RenderSettings.skybox.SetColor("_MoonColor", Moon.color);

            Color skyColor1 = (Color)skyColor.Evaluate(alpha);
            Color skyColor2 = (Color)middleColor.Evaluate(alpha);
            Color skyColor3 = (Color)groundColor.Evaluate(alpha);

            RenderSettings.skybox.SetColor("_SkyColor1", skyColor1);
            RenderSettings.skybox.SetColor("_SkyColor2", skyColor2);
            RenderSettings.skybox.SetColor("_SkyColor3", skyColor3);
        }

        protected virtual void StartDay()
        {
            RenderSettings.sun = Sun;
            Sun.shadows = LightShadows.Soft;
            Moon.shadows = LightShadows.None;
        }

        protected virtual void StartNight()
        {
            RenderSettings.sun = Moon;
            Sun.shadows = LightShadows.None;
            Moon.shadows = LightShadows.Soft;
        }

        protected virtual void UpdateTimeType()
        {
            if (TimeOfDay >= 6.0f && TimeOfDay < 12)
            {
                timeType = DesignEnum.TimeType.Morning;
            }
            else if (TimeOfDay >= 12 && TimeOfDay < 18)
            {
                timeType = DesignEnum.TimeType.Noon;
            }
            else if (TimeOfDay >= 18 && TimeOfDay < 23)
            {
                timeType = DesignEnum.TimeType.Night;
            }
            else
            {
                timeType = DesignEnum.TimeType.Midnight;
            }

            if (messageCheck != timeType)
            {
                InvokeTimeEvent();
                messageCheck = timeType;
            }
        }

        public virtual UnityEvent GetEvent(DesignEnum.TimeType timeType,System.Action action)
        {
            switch (timeType)
            {
                case DesignEnum.TimeType.Morning:
                    OnMorning.AddListener(action.Invoke);
                    return OnMorning;
                case DesignEnum.TimeType.Noon:
                    OnNoon.AddListener(action.Invoke);
                    return OnNoon;
                case DesignEnum.TimeType.Night:
                    OnNight.AddListener(action.Invoke);
                    return OnNight;
                case DesignEnum.TimeType.Midnight:
                    OnMidnight.AddListener(action.Invoke);
                    return OnMidnight;
                default:
                    return null;
            }
        }

        private void InvokeTimeEvent()
        {
            switch (timeType)
            {
                case DesignEnum.TimeType.Morning:
                    StartDay();
                    if (OnMorning != null) OnMorning.Invoke();
                    break;
                case DesignEnum.TimeType.Noon:
                    if (OnNoon != null) OnNoon.Invoke();
                    break;
                case DesignEnum.TimeType.Night:
                    StartNight();
                    if (OnNight != null) OnNight.Invoke();
                    break;
                case DesignEnum.TimeType.Midnight:
                    if (OnMidnight != null) OnMidnight.Invoke();
                    break;
            }
        }

    }
}