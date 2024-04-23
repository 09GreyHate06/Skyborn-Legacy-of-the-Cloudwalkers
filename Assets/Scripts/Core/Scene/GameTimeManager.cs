using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace SLOTC.Core.Scene
{
    public class GameTimeManager : MonoBehaviour
    {
        [SerializeField] Light _globalLight;
        [SerializeField] Gradient _gradientNightToSunrise;
        [SerializeField] Gradient _gradientSunriseToDay;
        [SerializeField] Gradient _gradientDayToSunset;
        [SerializeField] Gradient _gradientSunsetToNight;


        [SerializeField] Texture2D _skyboxSunrise;
        [SerializeField] Texture2D _skyboxDay;
        [SerializeField] Texture2D _skyboxSunset;
        [SerializeField] Texture2D _skyboxNight;


        [SerializeField] float _blendTime = 10;

        [SerializeField] int _secondsInAMinute = 60;
        [SerializeField] int _minutesInAnHour = 60;
        private int _seconds;
        private int _minutes;
        private int _hours = 8;
        private int _days;


        private int _hoursInADay = 24;
        private float _tempSeconds;

        public int Seconds
        {
            get
            {
                return _seconds;
            }
            set
            {
                _seconds = value;
                OnSecondsChanged();
            }
        }

        public int Minutes
        {
            get
            {
                return _minutes;
            }
            set
            {
                _minutes = value;
                OnMinutesChanged();
            }
        }

        public int Hours
        {
            get 
            { 
                return _hours;
            }
            set
            {
                _hours = value;
                OnHoursChanged();
            }
        }

        public int Days
        {
            get
            {
                return _days;
            }
            set
            {
                _days = value;
                OnDaysChanged();
            }
        }

        private void Start()
        {
            RenderSettings.skybox.SetTexture("_Texture1", _skyboxDay);
            RenderSettings.skybox.SetTexture("_Texture2", _skyboxDay);
            RenderSettings.skybox.SetFloat("_Blend", 0.0f);
            _globalLight.color = _gradientSunriseToDay.Evaluate(1.0f);
            RenderSettings.fogColor = _globalLight.color;
            RenderSettings.ambientSkyColor = _globalLight.color * 0.3f;
        }

        private void Update()
        {
            _globalLight.transform.Rotate(Vector3.up, 1.0f / (_secondsInAMinute * _minutesInAnHour * _hoursInADay) * 360.0f * Time.deltaTime, Space.World);

            _tempSeconds += Time.deltaTime;
            if(_tempSeconds >= 1.0f)
            {
                ++Seconds;
                _tempSeconds = 0.0f;
            }
        }

        private void OnSecondsChanged()
        {
            
            if(_seconds >= _secondsInAMinute)
            {
                ++Minutes;
                _seconds = 0;
            }
        }

        private void OnMinutesChanged()
        {
            if (_minutes >= _minutesInAnHour)
            {
                ++Hours;
                _minutes = 0;
            }
        }

        private void OnHoursChanged()
        {
            if(_hours >= _hoursInADay)
            {
                ++Days;
                _hours = 0;
            }

            if (_hours == 6)
            {
                StartCoroutine(LerpSkybox(_skyboxNight, _skyboxSunrise, _blendTime));
                StartCoroutine(LerpLight(_gradientNightToSunrise, _blendTime));
            }
            else if (_hours == 8)
            {
                StartCoroutine(LerpSkybox(_skyboxSunrise, _skyboxDay, _blendTime));
                StartCoroutine(LerpLight(_gradientSunriseToDay, _blendTime));
            }
            else if (_hours == 18)
            {
                StartCoroutine(LerpSkybox(_skyboxDay, _skyboxSunset, _blendTime));
                StartCoroutine(LerpLight(_gradientDayToSunset, _blendTime));
            }
            else if (_hours == 20)
            {
                StartCoroutine(LerpSkybox(_skyboxSunset, _skyboxNight, _blendTime));
                StartCoroutine(LerpLight(_gradientSunsetToNight, _blendTime));
            }
        }

        private void OnDaysChanged()
        {

        }

        private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
        {
            RenderSettings.skybox.SetTexture("_Texture1", a);
            RenderSettings.skybox.SetTexture("_Texture2", b);
            RenderSettings.skybox.SetFloat("_Blend", 0);
            for(float i = 0.0f; i < time; i += Time.deltaTime)
            {
                RenderSettings.skybox.SetFloat("_Blend", i / time);
                yield return null;
            }
            RenderSettings.skybox.SetTexture("_Texture1", b);
        }

        private IEnumerator LerpLight(Gradient lightGradient, float time)
        {
            for (float i = 0.0f; i < time; i += Time.deltaTime)
            {
                _globalLight.color = lightGradient.Evaluate(i / time);
                RenderSettings.fogColor = _globalLight.color;
                RenderSettings.ambientSkyColor = _globalLight.color * 0.3f;
                yield return null;
            }
        }
    }
}