using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVoiceAudio : MonoBehaviour
{
    [Serializable]
    private class VoiceClipEntry
    {
        public string voiceId;
        public AudioClip clip;
    }

    [Header("Audio")]
    [SerializeField] private AudioSource voiceAudioSource;

    [Header("Voice Clips")]
    [SerializeField] private VoiceClipEntry[] voiceClips;

    private Dictionary<string, AudioClip> voiceClipLookup;

    private void Awake()
    {
        voiceClipLookup = new Dictionary<string, AudioClip>(StringComparer.OrdinalIgnoreCase);

        foreach (VoiceClipEntry entry in voiceClips)
        {
            if (string.IsNullOrWhiteSpace(entry.voiceId))
            {
                continue;
            }

            if (entry.clip == null)
            {
                Debug.LogWarning($"No AudioClip assigned for voice ID'{entry.voiceId}'.");

                continue;
            }

            voiceClipLookup[entry.voiceId] = entry.clip;
        }
        if (voiceAudioSource == null)
        {
            voiceAudioSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.dialogueEvents.onVoiceLineRequested += PlayVoiceLine;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.dialogueEvents.onVoiceLineRequested -= PlayVoiceLine;
    }

    private void PlayVoiceLine(string voiceId)
    {
        voiceAudioSource.Stop();
        voiceAudioSource.clip = null;

        if (string.IsNullOrWhiteSpace(voiceId))
        {
            return;
        }

        if (!voiceClipLookup.TryGetValue(voiceId, out AudioClip voiceClip))
        {
            Debug.LogWarning($"No voice clip found for voice ID '{voiceId}'.", this);
            return;
        }

        voiceAudioSource.clip = voiceClip;
        voiceAudioSource.Play();
    }
}
