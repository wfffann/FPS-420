using UnityEngine;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/FireArms Audio Data")]
    public class FireArmsAudioData : ScriptableObject
    {
        public AudioClip shootingAudioClip;
        public AudioClip reloadLeft;
        public AudioClip reloadOutOf;
    }
}
