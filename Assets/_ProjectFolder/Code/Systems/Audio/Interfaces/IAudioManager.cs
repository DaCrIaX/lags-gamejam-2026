using UnityEngine.AddressableAssets;

namespace UnityEngine.Audio
{
    public interface IAudioManager
    {
        Awaitable<IAudioGenerator> LoadAudioAsset(AssetReferenceT<AudioClip> reference, bool hasLoaded = false);
        void UnloadAudioAsset(AssetReferenceT<AudioClip> reference);

        void Play(Channel channel, IAudioGenerator key);
        void PlayAtPoint(Channel channel, IAudioGenerator audio, Vector3 position);
        void PlayOneShot(Channel channel, IAudioGenerator key, float pitch = 1f);
        void PlayOneShotAtPoint(Channel channel, IAudioGenerator key, Vector3 position, float pitch = 1f);
        void ResetAudio(Channel channel);
        void SaveCurrentAudio(Channel channel);
    }
}