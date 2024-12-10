﻿using UnityEngine;
using UnityEngine.Audio;

namespace VehiclePhysics.Scripts.Vehicle.Sound
{
    /// <summary>
    /// Shifter sound played when changing gears.
    /// Supports multiple audio clips of which one is chosen at random each time this effect is played.
    /// </summary>
    [System.Serializable]
    public class GearChangeComponent : SoundComponent
    {
        private int previousGear;

        public override void Initialize(VehicleController vc, AudioMixerGroup amg)
        {
            this.vc = vc;
            this.audioMixerGroup = amg;

            if (Clips.Count != 0)
            {
                // Setup_VE_Dependency gear shift sound
                Source = vc.gameObject.AddComponent<AudioSource>();
                vc.sound.SetAudioSourceDefaults(Source, false, false, volume, RandomClip);
                RegisterSources();
            }
        }

        public override void Update()
        {
            if (Clips.Count != 0)
            {
                if (previousGear != vc.transmission.Gear && !Source.isPlaying)
                {
                    Source.clip = RandomClip;
                    SetVolume(volume + volume * Random.Range(-0.1f, 0.1f));
                    Source.pitch = pitch + pitch * Random.Range(-0.1f, 0.1f);
                    if(Source.enabled) Source.Play();
                }

                previousGear = vc.transmission.Gear;
            }
        }
    }
}
