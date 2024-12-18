﻿using UnityEngine;
using UnityEngine.Audio;

namespace VehiclePhysics.Scripts.Vehicle.Sound
{
    /// <summary>
    /// IVirtualEntityProduct horn sound.
    /// </summary>
    [System.Serializable]
    public class HornComponent : SoundComponent
    {
        public override void Initialize(VehicleController vc, AudioMixerGroup amg)
        {
            this.vc = vc;
            this.audioMixerGroup = amg;

            if (Clips.Count != 0)
            {
                Source = vc.gameObject.AddComponent<AudioSource>();
                vc.sound.SetAudioSourceDefaults(Source, false, true, volume, RandomClip);
                RegisterSources();
            }
        }

        public override void Update()
        {
            if(Source != null && Clip != null)
            {
                if (vc.input.horn && !Source.isPlaying)
                {
                    Source.pitch = pitch;
                    Source.Play();
                }
                else if(!vc.input.horn && Source.isPlaying)
                {
                    Source.Stop();
                }
            }
        }
    }
}
