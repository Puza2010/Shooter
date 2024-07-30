using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Playniax.Ignition;

namespace Playniax.Sequencer
{
    public class SequencerBase : SpawnerBase
    {
        public int index;
        public int wave = 1;

        public UnityEvent onFinished;

#if UNITY_EDITOR
        //[Header("Specific sequence to start from")]
        //public SequenceBase sequence;
        [Header("Simulation keys (+ Left Shift)")]
        public KeyCode killWave = KeyCode.K;
#endif
        public override void Awake()
        {
            base.Awake();

            var sequences = gameObject.GetComponentsInChildren<SequenceBase>();

            for (int i = 0; i < sequences.Length; i++)
            {
                sequences[i].OnSequencerInit();
            }

            sequences = gameObject.GetComponentsInChildren<SequenceBase>();

            for (int i = 0; i < sequences.Length; i++)
            {
                sequences[i].OnSequencerAwake();

                if (_gameObjects.Contains(sequences[i].gameObject) == false) _gameObjects.Add(sequences[i].gameObject);
            }

            for (int i = 0; i < _gameObjects.Count; i++)
            {
                _gameObjects[i].SetActive(false);
            }

            for (int i = 0; i < sequences.Length; i++)
            {
                sequences[i].sequencer = this;
                sequences[i].state = 1;
            }

            /*
#if UNITY_EDITOR
            if (sequence != null) index = GetIndex();

            int GetIndex()
            {
                for (int i = 0; i < sequences.Length; i++)
                {
                    if (sequences[i] == sequence) return i;
                }

                return 0;
            }
#endif
            */
        }

        void Update()
        {
            _Update();

#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.LeftShift) && index < _gameObjects.Count - 1)
            {
                if (Input.GetKeyDown(killWave))
                {
                    var wave = FindObjectsOfType<ProgressCounter>();

                    for (int i = 0; i < wave.Length; i++)
                        Destroy(wave[i].gameObject);

                    _gameObjects[index].SetActive(false);

                    index += 1;
                }
            }
#endif
        }
        void _Update()
        {
            if (index >= _gameObjects.Count)
            {
                onFinished?.Invoke();

                enabled = false;

                return;
            }

            if (TimingHelper.Paused) return;
            if (EasyGameUI.instance && EasyGameUI.instance.isBusy) return;
            if (EasyGameUI.instance && EasyGameUI.instance.effects.messenger.isActiveAndEnabled) return;
            if (TextEffect.current) return;

            if (_gameObjects[index].activeInHierarchy == false) _gameObjects[index].SetActive(true);

            _CleanUp();

            var sequences = _gameObjects[index].GetComponentsInChildren<SequenceBase>();

            if (_Busy(sequences) == false) index += 1;
        }
        bool _Busy(SequenceBase[] sequences)
        {
            for (int i = 0; i < sequences.Length; i++)
            {
                if (sequences[i].enabled) return true;
            }
            return false;
        }
        void _CleanUp()
        {
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                var sequences = _gameObjects[i].GetComponentsInChildren<SequenceBase>(true);

                if (_Busy(sequences) == false)
                {
                    _gameObjects[i].SetActive(false);
                }
            }
        }

        List<GameObject> _gameObjects = new List<GameObject>();
    }
}

