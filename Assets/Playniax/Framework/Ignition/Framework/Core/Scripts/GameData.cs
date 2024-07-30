// https://answers.unity.com/questions/45676/making-a-timer-0000-minutes-and-seconds.html

namespace Playniax.Ignition
{
    // GameData is typically used to store statistics of the game temporarily per scene. It has has a number of built-in variables but custom fields can be added. The information will not be saved and is only avaiable at runtime.
    public class GameData
    {
        // Used by the spawners. It tells you the number of spawned objects that are destroyed.
        public static int bodyCount;
        public static int completed;
        // Used by the spawners. It tells you the game progress.
        public static int progress;
        public static int progressScale;
        public static int spawned;

        // Custom data storage. Supports floats, integers, booleans and strings. Have a look at the $Config$ class for more information.
        public Config custom = new Config();
    }
}