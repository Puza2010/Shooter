using UnityEngine;

namespace Playniax.Pyro
{
    public interface IScoreBase
    {
        bool enabled
        {
            get;
            set;
        }

        GameObject friend
        {
            get;
            set;
        }

        GameObject gameObject { get; }

        bool isActiveAndEnabled { get; }

        bool indestructible
        {
            get;
            set;
        }

        GameObject isTargeted
        {
            get;
            set;
        }

        bool isVisible
        {
            get;
        }

        public string material
        {
            get;
            set;
        }

        public int points
        {
            get;
        }

        float structuralIntegrity
        {
            get;
            set;
        }

        void DoDamage(float damage);

        void Ghost();

        void Kill();
    }
}
