using UnityEditor;
using UnityEngine;
using Playniax.Framework.Editor;

namespace Playniax.Prototyping.Editor
{
    public class Helpers : EditorWindowHelpers
    {
        const string root = "Assets/Playniax/Framework/Pyro/Prototyping";

        [MenuItem("Tools/Playniax/Pyro Prototyping/UI/Player Dash 1", false, 61)]
        public static void Add_PlayerDash1()
        {
            GetAssetAtPath(root + "/Prefabs/UI/Player/Player Dash 1.prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/UI/Player Dash 2", false, 61)]
        public static void Add_PlayerDash2()
        {
            GetAssetAtPath(root + "/Prefabs/UI/Player/Player Dash 2.prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Players/Player (Spaceship)", false, 61)]
        public static void Add_Player()
        {
            GetAssetAtPath(root + "/Prefabs/Players/Player (Spaceship).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Players/Player (Spaceship Weaponized)", false, 61)]
        public static GameObject Add_Player_Weaponized()
        {
            return GetAssetAtPath(root + "/Prefabs/Players/Player (Spaceship Weaponized).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Enemies/Enemy", false, 61)]
        public static void Enemey_01()
        {
            GetAssetAtPath(root + "/Prefabs/Enemies/Enemy.prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Enemies/Rocket", false, 61)]
        public static void Add_Rocket()
        {
            GetAssetAtPath(root + "/Prefabs/Enemies/Rocket.prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (3 Way Shooter)", false, 61)]
        public static void Add_Pickup_3_Way_Shooter()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (3 Way Shooter).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Cannons)", false, 61)]
        public static void Add_Pickup_Cannons()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Cannons).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Coin)", false, 61)]
        public static void Add_Pickup_Coin()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Coin).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Drone)", false, 61)]
        public static void Add_Pickup_Drone()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Drone).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Energy Beam)", false, 61)]
        public static void Add_Pickup_Energy_Beam()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Energy Beam).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Extra Life)", false, 61)]
        public static void Add_Pickup_Extra_Life()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Extra Life).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Health)", false, 61)]
        public static void Add_Pickup_Health()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Health).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Laser)", false, 61)]
        public static void Add_Pickup_Laser()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Laser).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Main Gun)", false, 61)]
        public static void Add_Pickup_Main_Gun()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Main Gun).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Missiles)", false, 61)]
        public static void Add_Pickup_Missiles()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Missiles).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Nuke)", false, 61)]
        public static void Add_Pickup_Nuke()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Nuke).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Phaser)", false, 61)]
        public static void Add_Pickup_Phaser()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Phaser).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Shield)", false, 61)]
        public static void Add_Pickup_Shield()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Shield).prefab");
        }

        [MenuItem("Tools/Playniax/Pyro Prototyping/Sprites/Pickups/Pickup (Wrecking Ball)", false, 61)]
        public static void Add_Pickup_Wrecking_Ball()
        {
            GetAssetAtPath(root + "/Prefabs/Players (Pickups)/Pickup (Wrecking Ball).prefab");
        }
    }
}


