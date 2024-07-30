using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class Matroesjka : MonoBehaviour
    {
        public int count = 3;
        public float scale = 1;

        void Awake()
        {
            var collisionState = GetComponent<CollisionState>();
            if (collisionState == null) return;

            gameObject.SetActive(false);

            var clone = Instantiate(gameObject);

            DestroyImmediate(clone.GetComponent<Matroesjka>());

            gameObject.SetActive(true);

            var clones = 2;
            var scale = this.scale;

            for (int i = 0; i < count; i++)
            {
                var copy = (GameObject[])collisionState.cargoSettings.prefab.Clone();

                collisionState.cargoSettings.prefab = new GameObject[1];
                collisionState.cargoSettings.prefab[0] = Instantiate(clone);
                collisionState.cargoSettings.prefab[0].transform.localScale *= scale;

                System.Array.Resize(ref collisionState.cargoSettings.prefab, clones);

                for (int j = 0; j < clones; j++)
                {
                    collisionState.cargoSettings.prefab[j] = collisionState.cargoSettings.prefab[0];
                }

                collisionState.cargoSettings.prefab = ArrayHelpers.Merge(collisionState.cargoSettings.prefab, copy);

                collisionState = collisionState.cargoSettings.prefab[0].GetComponent<CollisionState>();

                scale *= this.scale;

                clones += 1;
            }
        }
    }
}