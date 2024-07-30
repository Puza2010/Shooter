using UnityEngine;
using UnityEngine.Events;

namespace Playniax.Ignition
{
    public class PlayerLivesHelper : IgnitionBehaviour
    {
        public GameObject prefab;
        public int playerIndex;
        public float timeout = 1.5f;
#if UNITY_EDITOR
        public int testLives = 9;
#endif
        public bool resetCamera;
        public float cameraSteps = 10;
        public float cameraPosition = .01f;
        public UnityEvent<PlayerLivesHelper> onInit;
        public UnityEvent onGameOver;
        public override void OnInitialize()
        {
            if (enabled == false) return;

            onInit?.Invoke(this);

            if (prefab == null) return;

            if (PlayerData.Get(playerIndex).lives <= 0)
            {
                Destroy(prefab);

                return;
            }

#if UNITY_EDITOR
            if (EasyGameUI.instance == null) PlayerData.Get(playerIndex).lives = testLives;
#endif
            _parent = prefab.transform.parent;

            _prefab = Instantiate(prefab);
            _prefab.name = prefab.name;
#if UNITY_EDITOR
            _prefab.hideFlags = HideFlags.HideInHierarchy;
#endif
            _prefab.SetActive(false);

            _player = prefab;
        }
        void LateUpdate()
        {
            if (_prefab == null) return;
            if (EasyGameUI.instance && EasyGameUI.instance.isBusy) return;
            if (PlayerData.Get(playerIndex).lives <= 0) return;
            if (_player && _player.activeInHierarchy) return;

            if (_timer < timeout)
            {
                _timer += 1 * Time.deltaTime;

                return;
            }

            if (PlayerData.Get(playerIndex).lives > 1 && resetCamera && _ResetCamera() == false) return;

            if (PlayerData.Get(playerIndex).lives > 1) _NewPlayer();

            PlayerData.Get(playerIndex).lives -= 1;

            if (PlayerData.Get(playerIndex).lives <= 0) onGameOver?.Invoke();
        }

        void _NewPlayer()
        {
            _player = Instantiate(_prefab, _parent);

            _player.name = _prefab.name;

            _player.SetActive(true);

            _timer = 0;
        }

        bool _ResetCamera()
        {
            var camera = Camera.main.transform.position;

            camera.x += (_prefab.transform.position.x - camera.x) / cameraSteps;
            camera.y += (_prefab.transform.position.y - camera.y) / cameraSteps;

            Camera.main.transform.position = camera;

            camera.z = _prefab.transform.position.z;

            if (Vector3.Distance(_prefab.transform.position, camera) < cameraPosition) return true;

            return false;
        }

        Transform _parent;
        GameObject _prefab;
        GameObject _player;
        float _timer;
    }
}

/*
        public void ShopHelper(PlayerLivesHelper playerLivesHelper)
        {
            playerLivesHelper.prefab = arsenalSettings.prefabs[0];
        }
*/
