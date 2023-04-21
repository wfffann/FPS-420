using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/FootStep Audio Data")]
public class FootStepAudioData : ScriptableObject
{
    public List<FootStepAudio> footStepAudios = new List<FootStepAudio>();

}

[System.Serializable]
public class FootStepAudio
{
    public string Tag;
    public List<AudioClip> AudioClips = new List<AudioClip>();

    public float sprintingDelay;
    public float Delay;
    public float crouchingDealy;
}
