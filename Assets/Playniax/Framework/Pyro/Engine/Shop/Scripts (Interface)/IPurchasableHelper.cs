namespace Playniax.Pyro
{
    public interface IPurchasableHelper
    {
        int purchasableCounter
        {
            get;
            set;
        }

        string purchasableId
        {
            get;
            set;
        }

        int purchasableMax
        {
            get;
            set;
        }

        int playerIndex
        {
            get;
            set;
        }

        void OnBuy();
    }
}