namespace Playniax.Ignition
{
    [System.Serializable]
    // PlayerData can be used to temporarily store data of the game like lives, name or score of the player. 
    //
    // Examples of built-in data fields are: lives, name and scoreboard.
    //
    // Custom fields can be added to custom and supports floats, integers, booleans and strings.
    //
    // This information will not be saved, but it will persist and remain available throughout scenes unless manually reset or reset by the $EasyGameUI/Introduction$.
    //
    // Example(s):
    //
    // Set name of the player              : <code>PlayerData.Get(0).name = "Tony";</code>
    //
    // Increase the score of the player    : <code>PlayerData.Get(0).scoreboard += 100;</code>
    //
    // Custom variable                     : <code>PlayerData.Get(0).custom.SetBool("invincible", true);</code>
    //
    // <i>* The zero used with the Get(0) command represents the player's index (where player 1 is 0, player 2 is 1, and so forth).</i>
    public class PlayerData
    {
        public static int defaultLives = 3;

        public static PlayerData[] data;

        // Number of coins the player has collected
        public int coins;
        // Number of collectables the player has gathered
        public int collectables;
        // Remaining lives of the player
        public int lives = defaultLives;
        // Name of the player's character
        public string name = "";
        // Current score of the player
        public int scoreboard;

        // Custom data storage for additional configurations
        public Config custom = new Config();
        public static int CountLives()
        {
            if (data == null) return 0;

            int lives = 0;

            for (int i = 0; i < data.Length; i++)
                lives += data[i].lives;
            
            return lives;
        }

        // Returns the PlayerData for the player by index.
        public static PlayerData Get(int index = 0)
        {
            if (index < 0) return null;
            if (data == null) System.Array.Resize(ref data, index + 1);
            if (index >= data.Length) System.Array.Resize(ref data, index + 1);
            if (data[index] == null) data[index] = new PlayerData();

            return data[index];
        }

        // Returns the PlayerData for the player by name.
        public static PlayerData Get(string name)
        {
            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                    if (data[i].name == name) return Get(i);
            }

            return null;
        }

        // Reset to defaults
        public static void Reset(int lives)
        {
            if (data == null) return;

            for (int i = 0; i < data.Length; i++)
            {
                data[i].coins = 0;
                data[i].collectables = 0;
                data[i].lives = lives;
                data[i].scoreboard = 0;

                data[i].custom.Clear();
            }
        }

        // Set number of lives
        public static void SetLives(int lives)
        {
            if (data == null) return;

            for (int i = 0; i < data.Length; i++)
                data[i].lives = lives;
        }
    }
}