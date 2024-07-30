namespace Playniax.Ignition
{
    [System.Serializable]
    public struct IntRange
    {
        public int min;
        public int max;

        public IntRange(int constant)
        {
            min = constant;
            max = constant;
        }
        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
