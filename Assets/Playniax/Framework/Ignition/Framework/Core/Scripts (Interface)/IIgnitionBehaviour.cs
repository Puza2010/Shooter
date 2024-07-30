namespace Playniax.Ignition
{
    public interface IIgnitionBehaviour
    {
        bool isInitialized
        {
            get;
            set;
        }

        void OnInitialize();
    }
}