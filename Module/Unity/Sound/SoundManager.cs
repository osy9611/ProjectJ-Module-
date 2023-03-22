namespace Module.Unity.Sound
{
    using Module.Unity.Addressables;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public enum Sound
    {
        Bgm,
        FX,
        UI
    }

    public class SoundManager
    {
        AudioSource[] audioSources = new AudioSource[System.Enum.GetNames(typeof(Sound)).Length];
        Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

        ResourceManager resourceManager;

        private Transform root;

        public void Init(ResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;

            if(root == null)
            {
                root = new GameObject { name = "@Sound" }.transform;
                Object.DontDestroyOnLoad(root);

                string[] soundNames = System.Enum.GetNames(typeof(Sound));
                for (int i = 0, range = soundNames.Length; i < range; ++i)
                {
                    GameObject go = new GameObject { name = soundNames[i] };
                    audioSources[i] = go.AddComponent<AudioSource>();
                    go.transform.parent = root.transform;
                }

                audioSources[(int)Sound.Bgm].loop = true;
            }
        }

        public void Clear()
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.clip = null;
                audioSource.Stop();
            }

            audioClips.Clear();
        }

        public void Play(string path, Sound type = Sound.FX, float pitch = 1.0f)
        {
            AudioClip audioClip = GetOrAddAudioClip(path);
            Play(audioClip, type, pitch);
        }

        public void Play(AudioClip audioClip, Sound type = Sound.FX, float pitch = 1.0f)
        {
            if (audioClip == null)
                return;

            if(type == Sound.Bgm)
            {
                AudioSource audioSource = audioSources[(int)Sound.Bgm];
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.pitch = pitch;
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else
            {
                AudioSource audioSource = audioSources[(int)type];
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(audioClip);
            }
        }

        AudioClip GetOrAddAudioClip(string path, Sound type = Sound.FX)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            AudioClip audioClip = null;

            if (type == Sound.Bgm)
            {
                audioClip = resourceManager.LoadAndGet<AudioClip>(path);
            }
            else
            {
                if (audioClips.TryGetValue(path, out audioClip) == false)
                {
                    audioClip = resourceManager.LoadAndGet<AudioClip>(path);
                    audioClips.Add(path, audioClip);
                }
            }

            if (audioClip == null)
                Debug.LogError($"AudioClip Missing!! {path}");

            return audioClip;

        }
    }

}
