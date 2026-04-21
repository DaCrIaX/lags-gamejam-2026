using System.Linq;
using System.Collections.Generic;

namespace UnityEngine.Audio
{
    using Rendering;
    using AddressableAssets;

    public class AudioEmitterID : AudioEmitterBehaviour
    {
        [SerializeField] private SerializedDictionary<string, AssetReferenceT<AudioClip>> _audioReference;
        [SerializeField] private bool _preloadAsset;

        private IList<string> _ids;
        private bool[] _hasLoaded;
        private string _id;

        private async void Start()
        {
            _ids = _audioReference.Keys.ToList();
            _hasLoaded = new bool[_ids.Count];

            if (!_preloadAsset) return;

            for (int i = 0; i < _ids.Count; i++)
            {
                await LoadAsset(_audioReference[_ids[i]]);
                _hasLoaded[i] = true;
            }
        }
        private void OnDestroy()
        {
            for (int i = 0; i < _ids.Count; i++)
            {
                if (!_hasLoaded[i]) continue;
                _manager.UnloadAudioAsset(_audioReference[_ids[i]]);
            }
        }

        [ContextMenu("Add")] private void AddAudioTrack()
        {
            _audioReference.Add(string.Empty, null);
        }
        [ContextMenu("Remove")] private void RemoveEmptyTrack()
        {
            _audioReference.Remove(string.Empty);
        }

        public void PlayOneShot(string id)
        {
            _id = id;
            PlayOneShot();
        }

        public override void Play()
        {
            
        }
        public override async void PlayOneShot()
        {
            if (string.IsNullOrEmpty(_id)) return;

            int index = _ids.IndexOf(_id);
            if (index < 0) return;

            print($"loading audio: {_id} at index: {index}");
            var audio = await LoadAsset(_audioReference[_id], _hasLoaded[index]);
            _manager.PlayOneShot(_channel, audio);
            _hasLoaded[index] = true;
            _id = null;
        }
    }
}