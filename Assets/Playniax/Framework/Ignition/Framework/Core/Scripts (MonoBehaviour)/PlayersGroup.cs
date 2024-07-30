using System.Collections.Generic;
using UnityEngine;

namespace Playniax.Ignition
{
    [AddComponentMenu("Playniax/Ignition/PlayerGroup")]
    // Whether the GameObject is 'marked' as a player or not.
    //
    // PlayerControls and some of the AI and bullet spawners depend on it.
    public class PlayerGroup : MonoBehaviour
    {
        // Player id.
        public string id = "Player 1";

        // Returns the active players .
        public static int Count()
        {
            int count = 0;

            List<PlayerGroup> list = GetList();

            for (int i = 0; i < list.Count; i++)
                if (list[i] && list[i].gameObject && list[i].gameObject.activeInHierarchy) count += 1;

            return count;
        }

        // Returns the player by id.
        public static GameObject Get(string id)
        {
            List<PlayerGroup> list = GetList();

            for (int i = 0; i < list.Count; i++)
                if (list[i] && list[i].gameObject && list[i].gameObject.activeInHierarchy && list[i].id == id) return list[i].gameObject;

            return null;
        }

        // Returns the first player.
        public static GameObject GetFirstAvailable(GameObject locked = null)
        {
            if (locked != null && locked.activeInHierarchy == true) return locked;

            List<PlayerGroup> list = GetList();

            for (int i = 0; i < list.Count; i++)
                if (list[i] && list[i].gameObject && list[i].gameObject.activeInHierarchy == true) return list[i].gameObject;

            return null;
        }

        public static GameObject GetFirstAvailableAndLock()
        {
            return GetFirstAvailable(_firstAvailableLocked);
        }

        // Returns all players.
        public static List<PlayerGroup> GetList()
        {
            if (_update == false) return _list;

            _update = false;

            PlayerGroup[] list = FindObjectsOfType<PlayerGroup>();

            _list.Clear();

            for (int i = 0; i < list.Length; i++)
                _list.Add(list[i]);

            return _list;
        }

        // Returns a random player.
        public static GameObject GetRandom(GameObject locked = null)
        {
            if (locked && locked.activeInHierarchy) return locked;

            List<PlayerGroup> list = GetList();

            if (list == null) return null;
            if (list.Count == 0) return null;

            int index = Random.Range(0, list.Count);

            if (list[index] == null) return null;

            return list[index].gameObject;
        }

        public static GameObject GetRandomAndLock()
        {
            return GetRandom(_randomLocked);
        }

        // Returns whether player is a member.
        public static bool IsMember(GameObject gameObject)
        {
            if (gameObject == null) return false;

            PlayerGroup group = gameObject.GetComponent<PlayerGroup>();

            if (group == null) return false;

            List<PlayerGroup> list = GetList();

            for (int i = 0; i < list.Count; i++)
                if (list[i] != null && list[i].gameObject && list[i] == group) return true;

            return false;
        }

        void OnEnable()
        {
            _update = true;
        }

        void OnDisable()
        {
            _update = true;
        }

        static List<PlayerGroup> _list = new List<PlayerGroup>();
        static GameObject _firstAvailableLocked;
        static GameObject _randomLocked;
        static bool _update = true;
    }
}