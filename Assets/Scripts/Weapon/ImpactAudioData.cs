using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    [CreateAssetMenu(fileName = "FPS/Impact Audio Data")]
    public class ImpactAudioData : ScriptableObject
    {
        public List<ImpactTagsWithAudio> impactTagsWithAudios;
    }

    [System.Serializable]
    public class ImpactTagsWithAudio
    {
        public string Tag;
        public List<AudioClip> impactAudioClips;
    }
}


