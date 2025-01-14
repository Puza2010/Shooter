#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Playniax.Pyro;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Playniax.Ignition
{
    // The EasyGameUI will manage the different screens and can hold game data that has to be maintained throughout the game session like number of lives, current level, purchases, etc.
    public class EasyGameUI : MonoBehaviour
    {
        public GameObject skillSelectionPanel; // Assign in Inspector
        public GameObject skillButtonPrefab;   // Assign in Inspector (Prefab for the skill button)
        public Transform skillButtonContainer; // Assign in Inspector (Parent transform to hold skill buttons)
        public GameObject superSkillButtonPrefab; // Assign in Inspector - Special prefab for super skill buttons

        public GameObject redLaserPrefab;  // Assign in Inspector
        public GameObject blueLaserPrefab; // Assign in Inspector
        public GameObject greenLaserPrefab;  // Assign in Inspector
        public GameObject purpleLaserPrefab; // Assign in Inspector
        public GameObject wreckingBallPrefab;
        public GameObject dronePrefab; // Assign in Inspector
        
        public Sprite emptySkillSlotImage; // Assign in Inspector
        
        public Sprite mainGunImage; // Assign in Inspector
        public Sprite angledShotsImage; // Assign in Inspector
        public Sprite cannonImage; // Assign in Inspector
        public Sprite threeWayShooterImage; // Assign in Inspector
        public Sprite speedUpImage; // Assign in Inspector
        public Sprite increaseHealthImage ; // Assign in Inspector
        public Sprite missileImage;       // Assign in Inspector (Image for the Missile skill)
        public Sprite phaserImage;       // Assign the Phaser skill image in the Inspector
        public Sprite wreckingBallImage;       // Assign the Phaser skill image in the Inspector
        public Sprite shieldImage;       // Assign the Phaser skill image in the Inspector
        public Sprite redLaserImage;  // Assign in Inspector (Image for Red Laser)
        public Sprite blueLaserImage; // Assign in Inspector (Image for Blue Laser)
        public Sprite greenLaserImage;  // Assign in Inspector (Image for Green Laser)
        public Sprite purpleLaserImage; // Assign in Inspector (Image for Purple Laser)
        public Sprite slowEnemiesImage; // Assign in Inspector
        public Sprite slowEnemyBulletsImage; // Assign this sprite in the Unity Inspector
        public Sprite weaponSpeedImage; // Assign in Inspector (Icon for the Weapon Speed skill)
        public Sprite extraScoreImage; // Assign in Inspector
        public Sprite droneSkillImage; // Assign in Inspector
        public Sprite bouncingShotsImage; // Assign in Inspector
        public Sprite engineFireImage; // Assign in Inspector (Image for Engine Fire skill)
        public Sprite gunsBlazingIcon; // Assign in Inspector
        
        private int weaponSpeedLevel = 0; // Tracks the current level of the skill
        
        public GameObject skillIconsPanel; // Assign the SkillIconsPanel in the Inspector
        public GameObject skillIconPrefab; // Create a prefab for the skill icon UI element
        public Canvas skillIconsCanvas; // Assign in Inspector

        // Change from private to public
        public Dictionary<string, Skill> skills;
        public List<string> acquiredSkills = new List<string>(); // Tracks acquired skills in order
        private Skill selectedSkill;
        private List<GameObject> skillButtons = new List<GameObject>(); // To keep track of instantiated buttons
        
        private int selectedSkillIndex = 0; // Keeps track of which skill is currently selected
        private bool isSkillSelectionActive = false; // Tracks if the skill selection is active
        
        [System.Serializable]
        // Add the scenes for the advertisements here.
        public class AdvertismentSettings
        {
            // Whether to allow.
            public bool allow = true;
            // Scenes to load.
            public string[] sceneName;
        }

        [System.Serializable]
        // Add the scenes for the game levels here.
        public class LevelSettings
        {
            // List of levels.
            public string[] sceneName;
            // Whether to start the game paused when the game is used as background.
            public bool startPausedWhenGameIsUsedAsBackground;
            // Whether to ignore the home page when the game is used as background.
            public bool DisableHomePageWhenGameIsUsedAsBackground;
        }

        [System.Serializable]
        // Set the total of lives for the player here.
        public class PlayerSettings
        {
            // Number of lives for the player.
            public int lives = 3;
            // Whether to reset the game.
            public bool newGameEverytime = true;
        }

        [System.Serializable]
        // Add the scenes to be loaded before each level here.
        public class PreLevelSettings
        {
            // Whether to allow.
            public bool allow = true;
            // Scenes to load.
            public string[] sceneName;
        }

        [System.Serializable]
        // Add the scenes to be loaded after each level here.
        public class PostLevelSettings
        {
            // Whether to allow.
            public bool allow = true;
            // Scenes to load.
            public string[] sceneName;
        }

        const string NO_ADS_PRODUCT_ID = "noads";

        public enum Mode { UseBackgroundScene, UseGameAsBackground };

        [Header("Main Settings")]
        [Space(10)]

        // Operation Mode.
        public Mode mode;
        [Space(10)]
        // Background scene to load.
        public string backgroundSceneName = "Background Scene";
        // The game scenes or levels to load.
        [Space(10)]
        public LevelSettings levelSettings;
        // The number of lives, etc.
        public PlayerSettings playerSettings;

        [Space(10)]
        [Header("Advanced Settings")]
        [Space(10)]
        // The scene(s) containing the intro(s).
        public PreLevelSettings preLevelSettings;
        // The transition scene will be shown after each level is completed.
        public PostLevelSettings postLevelSettings;
        // The scene(s) containing the ad(s).
        public AdvertismentSettings advertisementSettings;
        [Space(10)]
        // Whether the loading progress screen is showed during loading.
        public bool showLoadingProgress = true;
        [Space(10)]
        // Fallback font.
        public Font font;

#if UNITY_EDITOR
        [Space(10)]
        [Header("Editor Simulation keys (Left Shift + Key)")]
        [Space(10)]
        public KeyCode gameOverKey = KeyCode.G;
        public KeyCode nextLevelKey = KeyCode.N;
        public KeyCode resetGameKey = KeyCode.R;
        public KeyCode reloadLevelKey = KeyCode.L;

        [Space(10)]
        [Header("Build Settings Helper")]
        [Space(10)]
        public string[] searchInFolders;
        public bool ignoreSearchInFoldersAndIsolate;
#endif

        // Returns the fallback font.
        public static Font GetFont(Font font)
        {
            if (instance && instance.font) return instance.font;

            return font;
        }

        // Returns the instance of the EasyGameUI.
        public static EasyGameUI instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<EasyGameUI>();

                return _instance;
            }
        }

        public About about
        {
            get
            {
                if (_about) return _about;

                _about = FindObjectOfType<About>(true);

                return _about;
            }
        }

        public Effects effects
        {
            get
            {
                if (_effects) return _effects;

                _effects = FindObjectOfType<Effects>(true);

                return _effects;
            }
        }

        public GameOver gameOver
        {
            get
            {
                if (_gameOver) return _gameOver;

                _gameOver = FindObjectOfType<GameOver>(true);

                return _gameOver;
            }
        }

        public Home home
        {
            get
            {
                if (_home) return _home;

                _home = FindObjectOfType<Home>(true);

                return _home;
            }
        }

        public InGame inGame
        {
            get
            {
                if (_inGame) return _inGame;

                _inGame = FindObjectOfType<InGame>(true);

                return _inGame;
            }
        }

        // Whether the EasyGameUI is showing UI activity.
        public bool isBusy
        {
            get
            {
                if (
                    _loading == true ||
                    _onTimerDone != null ||
                    _intermission == true ||
                    //(_effects && _effects.isActiveAndEnabled == true) ||
                    (home && home.isActiveAndEnabled == true) ||
                    (about && about.isActiveAndEnabled == true) ||
                    (gameOver && gameOver.isActiveAndEnabled == true) ||
                    (pause && pause.isActiveAndEnabled == true) ||
                    (settings && settings.isActiveAndEnabled == true) ||
                    (shop && shop.isActiveAndEnabled == true) ||
                    (loader && loader.isActiveAndEnabled == true) ||
                    (reset && reset.isActiveAndEnabled == true)
                    )
                    return true;

                return false;
            }
        }

        // Whether the loaded level is the last one in the list.
        public bool isLastLevel
        {
            get
            {
                int levelIndex = PlayerPrefs.GetInt("levelIndex");
                if (levelIndex == levelCount - 1) return true;
                return false;
            }
        }

        // Whether the level is completed.
        public bool isLevelCompleted
        {
            get
            {
                if (
                    TimingHelper.Paused == false &&
                    _asyncOperation != null && _asyncOperation.isDone == true &&
                    SpawnerBase.count == 0 &&
                    ObjectCounter.count == 0 &&
                    GameData.progress == 0 &&
                    ((inGame && inGame.isActiveAndEnabled == true) || inGame == null) &&
                    ((home && home.isActiveAndEnabled == false) || home == null) &&
                    ((about && about.isActiveAndEnabled == false) || about == null) &&
                    ((pause && pause.isActiveAndEnabled == false) || pause == null) &&
                    ((settings && settings.isActiveAndEnabled == false) || settings == null) &&
                    ((shop && shop.isActiveAndEnabled == false) || shop == null) &&
                    ((gameOver && gameOver.isActiveAndEnabled == false) || gameOver == null) &&
                    ((loader && loader.isActiveAndEnabled == false) || loader == null) &&
                    ((reset && reset.isActiveAndEnabled == false) || reset == null) &&
                    effects.flash.activeInHierarchy == false &&
                    effects.pauseFader.activeInHierarchy == false &&
                    effects.gameFader.activeInHierarchy == false &&
                    effects.screenFader.activeInHierarchy == false &&
                    effects.messenger.isActiveAndEnabled == false &&
                    _intermission == false &&
                    _loading == false
                    )
                    return true;

                return false;
            }
        }

        // Whether the EasyGameUI is loading.
        public bool isLoading
        {
            get
            {
                return _loading;
            }
        }

        // Whether the messenger is active.
        public bool isMessengerBusy
        {
            get
            {
                return effects.messenger.isActiveAndEnabled;
            }
        }

        // Whether the mouse is hovering the pausebutton.
        public bool isMouseOverPauseButton
        {
            get
            {
                if ((inGame && inGame.isActiveAndEnabled == false) || inGame == null) return false;
                if (_pauseRect == null) _pauseRect = inGame.pauseButton.gameObject.GetComponent<RectTransform>();
                Vector3 mousePosition = _pauseRect.InverseTransformPoint(Input.mousePosition);
                return _pauseRect.rect.Contains(mousePosition);
            }
        }

        // Returns the total number of levels.
        public int levelCount
        {
            get
            {
                return levelSettings.sceneName.Length;
            }
        }

        // Retrurns the current level.
        public int levelIndex
        {
            get
            {
                return PlayerPrefs.GetInt("levelIndex", 0);
            }
        }

        public Loader loader
        {
            get
            {
                if (_loader) return _loader;

                _loader = FindObjectOfType<Loader>(true);

                return _loader;
            }
        }

        public Pause pause
        {
            get
            {
                if (_pause) return _pause;

                _pause = FindObjectOfType<Pause>(true);

                return _pause;
            }
        }

        public Reset reset
        {
            get
            {
                if (_reset) return _reset;

                _reset = FindObjectOfType<Reset>(true);

                return _reset;
            }
        }

        public Settings settings
        {
            get
            {
                if (_settings) return _settings;

                _settings = FindObjectOfType<Settings>(true);

                return _settings;
            }
        }

        public Shop shop
        {
            get
            {
                if (_shop) return _shop;

                _shop = FindObjectOfType<Shop>(true);

                return _shop;
            }
        }

        public void Delay(Action action, float time)
        {
            if (_onTimerDone != null) return;

            _onTimerDone = action;
            _timer = time;
        }

        // Exit Intermission.
        //
        // An intermission can be an advertisement scene, pre level scene or post level scene.
        public void ExitIntermission()
        {
            _intermission = false;
        }

        // Is called when player selects the about button.
        public void AboutButton()
        {
            if (about == null) return;

            if (home && home.gameObject.activeInHierarchy) _back = home.gameObject;
            if (pause && pause.gameObject.activeInHierarchy) _back = pause.gameObject;
            if (gameOver && gameOver.gameObject.activeInHierarchy) _back = gameOver.gameObject;

            _back.SetActive(false);

            _current = about.gameObject;

            _current.SetActive(true);
        }

        // Is called when player selects the back button.
        public void BackButton()
        {
            if (_back == null) return;
            if (_current == null) return;

            _current.SetActive(false);

            _back.SetActive(true);

            _back = null;
            _current = null;
        }

        // Is called when player selects the exit button.
        public void ExitButton()
        {
            _Load();
        }

        // Will show the Game Over page. Usually this is managed by the EasyGameUI or other helpers but there might be circumstances where you want to manage it yourself.
        public void GameOver()
        {
            if (gameOver == null) return;

            StartCoroutine(_GameOver());

        }

        // Will show the Game Over page but after some delay. Usually this is managed by the EasyGameUI but there might be circumstances where you want to manage it yourself.
        public void GameOver(float delay)
        {
            Delay(GameOver, delay);
        }

        // Is called when player selects the reset button.
        public void ResetButton()
        {
            if (reset == null) return;

            reset.gameObject.SetActive(true);

            TimingHelper.Paused = true;
        }

        // Resets the game.
        public virtual string ResetGame()
        {
            PlayerData.Reset(playerSettings.lives);

            PlayerPrefs.SetInt("levelIndex", 0);
            PlayerPrefs.Save();
            
            weaponSpeedLevel = 0;
            BulletSpawnerBase.weaponSpeedMultiplier = 1.0f; // Reset the multiplier
            // Reset enemy speed multiplier
            EnemyAI.globalSpeedMultiplier = 1.0f;
            // Reset enemy bullet speed multiplier
            BulletBase.enemyBulletSpeedMultiplier = 1.0f;

            return levelSettings.sceneName[0];
        }

        // Is called when player selects the yes button.
        public void ResetYes()
        {
            if (reset == null) return;

            reset.gameObject.SetActive(false);

            StartCoroutine(Load(ResetGame, inGame.gameObject, false, true, advertisementSettings.sceneName, preLevelSettings.sceneName, null, null));
        }

        // Is called when player selects the no button.
        public void ResetNo()
        {
            if (reset == null) return;

            reset.gameObject.SetActive(false);

            if (
                ((home && home.isActiveAndEnabled == false) || home == null) &&
                ((inGame && inGame.isActiveAndEnabled == true) || inGame == null)
                )
                TimingHelper.Paused = false;
        }

        // Is called when player selects the settings button.
        public void SettingsButton()
        {
            if (settings == null) return;

            if (home && home.gameObject.activeInHierarchy) _back = home.gameObject;
            if (pause && pause.gameObject.activeInHierarchy) _back = pause.gameObject;
            if (gameOver && gameOver.gameObject.activeInHierarchy) _back = gameOver.gameObject;

            if (_back == null) return;

            _back.SetActive(false);

            _current = settings.gameObject;

            _current.SetActive(true);
        }

        // Is called when player selects the shop button.
        public void ShopButton()
        {
            if (shop == null) return;

            if (home && home.gameObject.activeInHierarchy) _back = home.gameObject;
            if (pause && pause.gameObject.activeInHierarchy) _back = pause.gameObject;
            if (gameOver && gameOver.gameObject.activeInHierarchy) _back = gameOver.gameObject;

            if (_back == null) return;

            _back.SetActive(false);

            _current = shop.gameObject;

            _current.SetActive(true);
        }

        public IEnumerator Load(Func<string> sceneName, GameObject showUI, bool paused, bool fade, string[] advertisements, string[] preLevels, string[] postLevels, Action onLoaded)
        {
            _loading = true;

            if (fade)
            {
                effects.SetScreenFader(1);

                while (effects.screenFaderImage.color.a != effects.screenFaderTarget) yield return null;
            }

            effects.messenger.gameObject.SetActive(false);

            if (home) home.gameObject.SetActive(false);
            if (inGame) inGame.gameObject.SetActive(false);
            if (about) about.gameObject.SetActive(false);
            if (gameOver) gameOver.gameObject.SetActive(false);
            if (pause) pause.gameObject.SetActive(false);
            if (settings) settings.gameObject.SetActive(false);
            if (shop) shop.gameObject.SetActive(false);
            if (reset) reset.gameObject.SetActive(false);

            effects.pauseFader.gameObject.SetActive(false);

            if (postLevelSettings.allow && postLevels != null && _intermission == false)
            {
                while (postLevels.Length > 0)
                {
                    _intermission = true;

                    SceneManager.LoadScene(postLevels[0]);

                    bool isPaused = TimingHelper.Paused;
                    TimingHelper.Paused = false;

                    if (fade)
                    {
                        effects.SetScreenFader(-1);

                        while (effects.screenFaderImage.color.a != effects.screenFaderTarget) yield return null;
                    }

                    postLevels = ArrayHelpers.Skim(postLevels);

                    while (_intermission == true) yield return null;

                    if (fade)
                    {
                        effects.SetScreenFader(1);

                        while (effects.screenFaderImage.color.a != effects.screenFaderTarget) yield return null;
                    }

                    TimingHelper.Paused = isPaused;
                }
            }

            if (advertisementSettings.allow && advertisements != null && _intermission == false && PlayerPrefs.GetInt(NO_ADS_PRODUCT_ID) == 0)
            {
                while (advertisements.Length > 0)
                {
                    _intermission = true;

                    SceneManager.LoadScene(advertisements[0]);

                    bool isPaused = TimingHelper.Paused;
                    TimingHelper.Paused = false;

                    if (fade)
                    {
                        effects.SetScreenFader(-1);

                        while (effects.screenFaderImage.color.a != effects.screenFaderTarget) yield return null;
                    }

                    advertisements = ArrayHelpers.Skim(advertisements);

                    while (_intermission == true) yield return null;

                    if (fade)
                    {
                        effects.SetScreenFader(1);

                        while (effects.screenFaderImage.color.a != effects.screenFaderTarget) yield return null;
                    }

                    TimingHelper.Paused = isPaused;
                }
            }

            if (preLevelSettings.allow && preLevels != null && _intermission == false)
            {
                while (preLevels.Length > 0)
                {
                    _intermission = true;

                    SceneManager.LoadScene(preLevels[0]);

                    bool isPaused = TimingHelper.Paused;
                    TimingHelper.Paused = false;

                    if (fade)
                    {
                        effects.SetScreenFader(-1);

                        while (effects.screenFaderImage.color.a != effects.screenFaderTarget) yield return null;
                    }

                    preLevels = ArrayHelpers.Skim(preLevels);

                    while (_intermission == true) yield return null;

                    if (fade)
                    {
                        effects.SetScreenFader(1);

                        while (effects.screenFaderImage.color.a != effects.screenFaderTarget) yield return null;
                    }

                    TimingHelper.Paused = isPaused;
                }
            }

            if (loader && showLoadingProgress) loader.gameObject.SetActive(true);

            _asyncOperation = SceneManager.LoadSceneAsync(sceneName.Invoke());

            _asyncOperation.allowSceneActivation = false;

            yield return null;

            while (_asyncOperation.isDone == false)
            {
                if (loader)
                {
                    if (loader.progressText) loader.progressText.text = (int)(_asyncOperation.progress * (100 / .9f)) + "%";
                    if (loader.progressBar) loader.progressBar.value = (int)(_asyncOperation.progress * (100 / .9f));
                }

                if (_asyncOperation.progress >= 0.9f)
                {
                    yield return null;

                    _asyncOperation.allowSceneActivation = true;

                    yield return null;
                }

                yield return null;
            }

            if (loader) loader.gameObject.SetActive(false);

            TimingHelper.Paused = paused;

            if (paused) effects.SetPauseFader(1);

            if (showUI) showUI.gameObject.SetActive(true);

            if (fade)
            {
                effects.SetScreenFader(-1);

                while (effects.screenFaderImage.color.a != effects.screenFaderTarget) yield return null;
            }

            _loading = false;

            yield return new WaitForEndOfFrame();
            
            // Activate or deactivate skillIconsCanvas based on the new scene
            if (inGame != null && inGame.gameObject.activeInHierarchy)
            {
                if (skillIconsCanvas != null && !skillIconsCanvas.gameObject.activeInHierarchy)
                {
                    skillIconsCanvas.gameObject.SetActive(true);
                    InitializeAcquiredSkills();
                }
            }
            else
            {
                if (skillIconsCanvas != null && skillIconsCanvas.gameObject.activeInHierarchy)
                {
                    skillIconsCanvas.gameObject.SetActive(false);
                }
            }

            if (onLoaded != null) onLoaded();
        }

        // Will stop current level and load the next level.
        public void LevelUp()
        {
            StartCoroutine(Load(_GetNextLevel, inGame.gameObject, false, true, advertisementSettings.sceneName, preLevelSettings.sceneName, postLevelSettings.sceneName, null));
        }

        // Will stop current level and reload it.
        public void Reload()
        {
            StartCoroutine(Load(SceneManager.GetActiveScene().name.ToString, inGame.gameObject, false, true, advertisementSettings.sceneName, preLevelSettings.sceneName, postLevelSettings.sceneName, null));
        }

        // Is called when player selects the pause button.
        public void PauseButton()
        {
            //if (isBusy || _effects.messenger.isActiveAndEnabled == true) return;
            if (isBusy) return;

            if (pause == null || inGame == null) return;

            if (pause.gameObject.activeInHierarchy == true) return;

            StartCoroutine(_PauseButton());
        }

        // Is called when player selects the play button.
        public void PlayButton()
        {
            if (home && home.gameObject.activeInHierarchy == true && inGame)
            {
                if (SceneManager.GetActiveScene().name == _GetLevelIndex())
                {
                    home.gameObject.SetActive(false);

                    inGame.gameObject.SetActive(true);

                    effects.SetPauseFader(-1);

                    TimingHelper.Paused = false;

                    FirebaseScoreUpdater.Instance.StartGame(); // Start the score updating counter
                }
                else
                {
                    // Consider skipping the preLevelSettings the first time here?
                    StartCoroutine(Load(_StartGame, inGame.gameObject, false, true, null, preLevelSettings.sceneName, null, null));

                    FirebaseScoreUpdater.Instance.StartGame(); // Start the score updating counter
                }
            }
            else if (pause && pause.gameObject.activeInHierarchy == true && inGame && inGame.gameObject.activeInHierarchy == false)
            {
                pause.gameObject.SetActive(false);

                inGame.gameObject.SetActive(true);

                effects.SetPauseFader(-1);

                TimingHelper.Paused = false;

                FirebaseScoreUpdater.Instance.StartGame(); // Start the score updating counter
                
                // Activate the Skill Icons Canvas
                if (skillIconsCanvas != null)
                {
                    skillIconsCanvas.gameObject.SetActive(true);
                    InitializeAcquiredSkills();
                }
            }
        }

        // Is called when player selects the replay button.
        public void ReplayButton()
        {
            StartCoroutine(Load(_StartGame, inGame.gameObject, false, true, advertisementSettings.sceneName, preLevelSettings.sceneName, null, () => {
                if (skillIconsCanvas != null)
                {
                    skillIconsCanvas.gameObject.SetActive(true);
                    InitializeAcquiredSkills();
                }
            }));
        }

        void Awake()
        {

#if UNITY_EDITOR

            var scenes = ScenesInBuildDialog();

            if (scenes != null)
            {
                EditorApplication.isPlaying = false;

                var message = "";

                for (int i = 0; i < scenes.Length; i++)
                    message += scenes[i].path + "\n";

                if (IsBuildSettingsOpen() == false)
                {
                    message += "\nDo you want to open the Build Settings window to verify?";

                    bool result = EditorUtility.DisplayDialog("Scenes added to Scenes In Build", message, "Yes, open the Build Settings window for me to verify", "No, I trust you and I will run the EasyGameUI again.");

                    if (result == true) EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                }

                return;
            }

            bool IsBuildSettingsOpen()
            {
                Type buildPlayerWindowType = System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor");

                EditorWindow[] windows = Resources.FindObjectsOfTypeAll(buildPlayerWindowType) as EditorWindow[];

                if (windows != null && windows.Length > 0) return true;

                return false;
            }
#endif

            DontDestroyOnLoad(gameObject);

            _simpleShare = GetComponent<SimpleShare>();

            SceneManager.sceneLoaded += OnSceneLoaded;

            PlayerData.defaultLives = playerSettings.lives;

            if (home) home.gameObject.SetActive(false);
            if (inGame) inGame.gameObject.SetActive(false);
            if (about) about.gameObject.SetActive(false);
            if (gameOver) gameOver.gameObject.SetActive(false);
            if (pause) pause.gameObject.SetActive(false);
            if (settings) settings.gameObject.SetActive(false);
            if (shop) shop.gameObject.SetActive(false);
            if (loader) loader.gameObject.SetActive(false);
            if (reset) reset.gameObject.SetActive(false);

            effects.screenFaderImage.color = new Color(effects.screenFaderImage.color.r, effects.screenFaderImage.color.g, effects.screenFaderImage.color.b, 1);

            effects.gameObject.SetActive(true);

            _Load();
        }

#if UNITY_ANDROID

        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && !isBusy)
            {
                PauseButton();
            }

            if (_intermission == false) AudioListener.pause = !hasFocus;
        }

#endif

#if UNITY_EDITOR || UNITY_IOS

        void OnApplicationPause(bool pause)
        {
            if (!pause && !isBusy)
            {
                PauseButton();
            }

            if (_intermission == false) AudioListener.pause = pause;
        }

#endif

        void OnDestroy()
        {
            _instance = null;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
        }
        
        void Start()
        {
            // Initialize the skill selection panel
            if (skillSelectionPanel)
            {
                skillSelectionPanel.SetActive(false);
            }

            // Initialize the skill dictionary
            skills = new Dictionary<string, Skill>();
            
            skills.Add("Main Gun Level 1", new Skill(null, 0f, 0, "", "Main Gun Level 1", mainGunImage, 1, 5));
            skills.Add("Main Gun Level 2", new Skill(null, 0f, 0, "", "Main Gun Level 2", mainGunImage, 2, 5));
            skills.Add("Main Gun Level 3", new Skill(null, 0f, 0, "", "Main Gun Level 3", mainGunImage, 3, 5));
            skills.Add("Main Gun Level 4", new Skill(null, 0f, 0, "", "Main Gun Level 4", mainGunImage, 4, 5));
            skills.Add("Main Gun Level 5", new Skill(null, 0f, 0, "", "Main Gun Level 5", mainGunImage, 5, 5));
                        
            // skills.Add("Red Laser Level 1", new Skill(redLaserPrefab, 1f, 10, "genericBulletRed", "Red Laser Level 1", redLaserImage, 1, 4));
            // skills.Add("Red Laser Level 2", new Skill(redLaserPrefab, 0.7f, 30, "genericBulletRed", "Red Laser Level 2", redLaserImage, 2, 4));
            // skills.Add("Red Laser Level 3", new Skill(redLaserPrefab, 0.4f, 50, "genericBulletRed", "Red Laser Level 3", redLaserImage, 3, 4));
            // skills.Add("Red Laser Level 4", new Skill(redLaserPrefab, 0.2f, 100, "genericBulletRed", "Red Laser Level 4", redLaserImage, 4, 4));
            // skills.Add("Blue Laser Level 1", new Skill(blueLaserPrefab, 1.0f, 10, "bulletBlue", "Blue Laser Level 1", blueLaserImage, 1, 4));
            // skills.Add("Blue Laser Level 2", new Skill(blueLaserPrefab, 0.7f, 30, "bulletBlue", "Blue Laser Level 2", blueLaserImage, 2, 4));
            // skills.Add("Blue Laser Level 3", new Skill(blueLaserPrefab, 0.4f, 50, "bulletBlue", "Blue Laser Level 3", blueLaserImage, 3, 4));
            // skills.Add("Blue Laser Level 4", new Skill(blueLaserPrefab, 0.2f, 100, "bulletBlue", "Blue Laser Level 4", blueLaserImage, 4, 4));
            // skills.Add("Green Laser Level 1", new Skill(greenLaserPrefab, 1.0f, 10, "genericBulletGreen", "Green Laser Level 1", greenLaserImage, 1, 4));
            // skills.Add("Green Laser Level 2", new Skill(greenLaserPrefab, 0.7f, 30, "genericBulletGreen", "Green Laser Level 2", greenLaserImage, 2, 4));
            // skills.Add("Green Laser Level 3", new Skill(greenLaserPrefab, 0.4f, 50, "genericBulletGreen", "Green Laser Level 3", greenLaserImage, 3, 4));
            // skills.Add("Green Laser Level 4", new Skill(greenLaserPrefab, 0.2f, 100, "genericBulletGreen", "Green Laser Level 4", greenLaserImage, 4, 4));
            // skills.Add("Purple Laser Level 1", new Skill(purpleLaserPrefab, 1.0f, 10, "bulletPurple", "Purple Laser Level 1", purpleLaserImage, 1, 4));
            // skills.Add("Purple Laser Level 2", new Skill(purpleLaserPrefab, 0.7f, 30, "bulletPurple", "Purple Laser Level 2", purpleLaserImage, 2, 4));
            // skills.Add("Purple Laser Level 3", new Skill(purpleLaserPrefab, 0.4f, 50, "bulletPurple", "Purple Laser Level 3", purpleLaserImage, 3, 4));
            // skills.Add("Purple Laser Level 4", new Skill(purpleLaserPrefab, 0.2f, 100, "bulletPurple", "Purple Laser Level 4", purpleLaserImage, 4, 4));
            
            skills.Add("Angled Shots Level 1", new Skill(null, 0f, 0, "", "Angled Shots Level 1", angledShotsImage, 1, 5));
            skills.Add("Angled Shots Level 2", new Skill(null, 0f, 0, "", "Angled Shots Level 2", angledShotsImage, 2, 5));
            skills.Add("Angled Shots Level 3", new Skill(null, 0f, 0, "", "Angled Shots Level 3", angledShotsImage, 3, 5));
            skills.Add("Angled Shots Level 4", new Skill(null, 0f, 0, "", "Angled Shots Level 4", angledShotsImage, 4, 5));
            skills.Add("Angled Shots Level 5", new Skill(null, 0f, 0, "", "Angled Shots Level 5", angledShotsImage, 5, 5));
            
            skills.Add("Cannons Level 1", new Skill(null, 0f, 0, "", "Cannons Level 1", cannonImage, 1, 5));
            skills.Add("Cannons Level 2", new Skill(null, 0f, 0, "", "Cannons Level 2", cannonImage, 2, 5));
            skills.Add("Cannons Level 3", new Skill(null, 0f, 0, "", "Cannons Level 3", cannonImage, 3, 5));
            skills.Add("Cannons Level 4", new Skill(null, 0f, 0, "", "Cannons Level 4", cannonImage, 4, 5));
            skills.Add("Cannons Level 5", new Skill(null, 0f, 0, "", "Cannons Level 5", cannonImage, 5, 5));
            
            skills.Add("3 Way Shooter Level 1", new Skill(null, 0f, 0, "", "3 Way Shooter Level 1", threeWayShooterImage, 1, 5));
            skills.Add("3 Way Shooter Level 2", new Skill(null, 0f, 0, "", "3 Way Shooter Level 2", threeWayShooterImage, 2, 5));
            skills.Add("3 Way Shooter Level 3", new Skill(null, 0f, 0, "", "3 Way Shooter Level 3", threeWayShooterImage, 3, 5));
            skills.Add("3 Way Shooter Level 4", new Skill(null, 0f, 0, "", "3 Way Shooter Level 4", threeWayShooterImage, 4, 5));
            skills.Add("3 Way Shooter Level 5", new Skill(null, 0f, 0, "", "3 Way Shooter Level 5", threeWayShooterImage, 5, 5));
            
            // skills.Add("Speed Up Level 1", new Skill(null, 0f, 0, "", "Speed Up Level 1", speedUpImage, 1, 5));
            // skills.Add("Speed Up Level 2", new Skill(null, 0f, 0, "", "Speed Up Level 2", speedUpImage, 2, 5));
            // skills.Add("Speed Up Level 3", new Skill(null, 0f, 0, "", "Speed Up Level 3", speedUpImage, 3, 5));
            // skills.Add("Speed Up Level 4", new Skill(null, 0f, 0, "", "Speed Up Level 4", speedUpImage, 4, 5));
            // skills.Add("Speed Up Level 5", new Skill(null, 0f, 0, "", "Speed Up Level 5", speedUpImage, 5, 5));
            
            // skills.Add("Health Upgrade Level 1", new Skill(null, 0f, 0, "", "Health Upgrade Level 1", increaseHealthImage, 1, 5));
            // skills.Add("Health Upgrade Level 2", new Skill(null, 0f, 0, "", "Health Upgrade Level 2", increaseHealthImage, 2, 5));
            // skills.Add("Health Upgrade Level 3", new Skill(null, 0f, 0, "", "Health Upgrade Level 3", increaseHealthImage, 3, 5));
            // skills.Add("Health Upgrade Level 4", new Skill(null, 0f, 0, "", "Health Upgrade Level 4", increaseHealthImage, 4, 5));
            // skills.Add("Health Upgrade Level 5", new Skill(null, 0f, 0, "", "Health Upgrade Level 5", increaseHealthImage, 5, 5));
            
            // skills.Add("Homing Missile Level 1", new Skill(null, 2.0f, 0, "", "Homing Missile Level 1", missileImage, 1, 5));
            // skills.Add("Homing Missile Level 2", new Skill(null, 1.5f, 0, "", "Homing Missile Level 2", missileImage, 2, 5));
            // skills.Add("Homing Missile Level 3", new Skill(null, 1.0f, 0, "", "Homing Missile Level 3", missileImage, 3, 5));
            // skills.Add("Homing Missile Level 4", new Skill(null, 0.75f, 0, "", "Homing Missile Level 4", missileImage, 4, 5));
            // skills.Add("Homing Missile Level 5", new Skill(null, 0.5f, 0, "", "Homing Missile Level 5", missileImage, 5, 5));
            
            skills.Add("Homing Gun Level 1", new Skill(null, 0f, 0, "", "Homing Gun Level 1", phaserImage, 1, 5));
            skills.Add("Homing Gun Level 2", new Skill(null, 0f, 0, "", "Homing Gun Level 2", phaserImage, 2, 5));
            skills.Add("Homing Gun Level 3", new Skill(null, 0f, 0, "", "Homing Gun Level 3", phaserImage, 3, 5));
            skills.Add("Homing Gun Level 4", new Skill(null, 0f, 0, "", "Homing Gun Level 4", phaserImage, 4, 5));
            skills.Add("Homing Gun Level 5", new Skill(null, 0f, 0, "", "Homing Gun Level 5", phaserImage, 5, 5));
            
            // skills.Add("Wrecking Ball Level 1", new Skill(wreckingBallPrefab, 0f, 0, "", "Wrecking Ball Level 1", wreckingBallImage, 1, 5));
            // skills.Add("Wrecking Ball Level 2", new Skill(wreckingBallPrefab, 0f, 0, "", "Wrecking Ball Level 2", wreckingBallImage, 2, 5));
            // skills.Add("Wrecking Ball Level 3", new Skill(wreckingBallPrefab, 0f, 0, "", "Wrecking Ball Level 3", wreckingBallImage, 3, 5));
            // skills.Add("Wrecking Ball Level 4", new Skill(wreckingBallPrefab, 0f, 0, "", "Wrecking Ball Level 4", wreckingBallImage, 4, 5));
            // skills.Add("Wrecking Ball Level 5", new Skill(wreckingBallPrefab, 0f, 0, "", "Wrecking Ball Level 5", wreckingBallImage, 5, 5));
            
            // skills.Add("Shield Level 1", new Skill(null, 0f, 0, "", "Shield Level 1", shieldImage, 1, 5));
            // skills.Add("Shield Level 2", new Skill(null, 0f, 0, "", "Shield Level 2", shieldImage, 2, 5));
            // skills.Add("Shield Level 3", new Skill(null, 0f, 0, "", "Shield Level 3", shieldImage, 3, 5));
            // skills.Add("Shield Level 4", new Skill(null, 0f, 0, "", "Shield Level 4", shieldImage, 4, 5));
            // skills.Add("Shield Level 5", new Skill(null, 0f, 0, "", "Shield Level 5", shieldImage, 5, 5));
            
            // skills.Add("Slow Enemies Level 1", new Skill(null, 0f, 0, "", "Slow Enemies Level 1", slowEnemiesImage, 1, 5));
            // skills.Add("Slow Enemies Level 2", new Skill(null, 0f, 0, "", "Slow Enemies Level 2", slowEnemiesImage, 2, 5));
            // skills.Add("Slow Enemies Level 3", new Skill(null, 0f, 0, "", "Slow Enemies Level 3", slowEnemiesImage, 3, 5));
            // skills.Add("Slow Enemies Level 4", new Skill(null, 0f, 0, "", "Slow Enemies Level 4", slowEnemiesImage, 4, 5));
            // skills.Add("Slow Enemies Level 5", new Skill(null, 0f, 0, "", "Slow Enemies Level 5", slowEnemiesImage, 5, 5));
            
            // skills.Add("Slow Enemy Bullets Level 1", new Skill(null, 0f, 0, "", "Slow Enemy Bullets Level 1", slowEnemyBulletsImage, 1, 5));
            // skills.Add("Slow Enemy Bullets Level 2", new Skill(null, 0f, 0, "", "Slow Enemy Bullets Level 2", slowEnemyBulletsImage, 2, 5));
            // skills.Add("Slow Enemy Bullets Level 3", new Skill(null, 0f, 0, "", "Slow Enemy Bullets Level 3", slowEnemyBulletsImage, 3, 5));
            // skills.Add("Slow Enemy Bullets Level 4", new Skill(null, 0f, 0, "", "Slow Enemy Bullets Level 4", slowEnemyBulletsImage, 4, 5));
            // skills.Add("Slow Enemy Bullets Level 5", new Skill(null, 0f, 0, "", "Slow Enemy Bullets Level 5", slowEnemyBulletsImage, 5, 5));
            
            // skills.Add("Weapon Speed Level 1", new Skill(null, 0f, 0, "", "Weapon Speed Level 1", weaponSpeedImage, 1, 5));
            // skills.Add("Weapon Speed Level 2", new Skill(null, 0f, 0, "", "Weapon Speed Level 2", weaponSpeedImage, 2, 5));
            // skills.Add("Weapon Speed Level 3", new Skill(null, 0f, 0, "", "Weapon Speed Level 3", weaponSpeedImage, 3, 5));
            // skills.Add("Weapon Speed Level 4", new Skill(null, 0f, 0, "", "Weapon Speed Level 4", weaponSpeedImage, 4, 5));
            // skills.Add("Weapon Speed Level 5", new Skill(null, 0f, 0, "", "Weapon Speed Level 5", weaponSpeedImage, 5, 5));
            
            skills.Add("Drone Level 1", new Skill(null, 0f, 0, "", "Drone Level 1", droneSkillImage, 1, 5));
            skills.Add("Drone Level 2", new Skill(null, 0f, 0, "", "Drone Level 2", droneSkillImage, 2, 5));
            skills.Add("Drone Level 3", new Skill(null, 0f, 0, "", "Drone Level 3", droneSkillImage, 3, 5));
            skills.Add("Drone Level 4", new Skill(null, 0f, 0, "", "Drone Level 4", droneSkillImage, 4, 5));
            skills.Add("Drone Level 5", new Skill(null, 0f, 0, "", "Drone Level 5", droneSkillImage, 5, 5));
            
            skills.Add("Bouncing Shot Level 1", new Skill(null, 0f, 0, "", "Bouncing Shot Level 1", bouncingShotsImage, 1, 5));
            skills.Add("Bouncing Shot Level 2", new Skill(null, 0f, 0, "", "Bouncing Shot Level 2", bouncingShotsImage, 2, 5));
            skills.Add("Bouncing Shot Level 3", new Skill(null, 0f, 0, "", "Bouncing Shot Level 3", bouncingShotsImage, 3, 5));
            skills.Add("Bouncing Shot Level 4", new Skill(null, 0f, 0, "", "Bouncing Shot Level 4", bouncingShotsImage, 4, 5));
            skills.Add("Bouncing Shot Level 5", new Skill(null, 0f, 0, "", "Bouncing Shot Level 5", bouncingShotsImage, 5, 5));

            // skills.Add("Engine Fire Level 1", new Skill(null, 0f, 0, "", "Engine Fire Level 1", engineFireImage, 1, 5));
            // skills.Add("Engine Fire Level 2", new Skill(null, 0f, 0, "", "Engine Fire Level 2", engineFireImage, 2, 5));
            // skills.Add("Engine Fire Level 3", new Skill(null, 0f, 0, "", "Engine Fire Level 3", engineFireImage, 3, 5));
            // skills.Add("Engine Fire Level 4", new Skill(null, 0f, 0, "", "Engine Fire Level 4", engineFireImage, 4, 5));
            // skills.Add("Engine Fire Level 5", new Skill(null, 0f, 0, "", "Engine Fire Level 5", engineFireImage, 5, 5));
            
            UpdateSkillIconsDisplay(); // Update the skill icons display
        }

        void Update()
        {
            if (_onTimerDone != null)
            {
                if (isMessengerBusy == false) _timer -= 1 * Time.deltaTime;

                if (_timer <= 0)
                {
                    _onTimerDone.Invoke();

                    _onTimerDone = null;
                }
            }

            if (isSkillSelectionActive)
            {
                HandleSkillSelectionInput();
            }
        }


        IEnumerator _GameOver()
        {
            yield return new WaitForEndOfFrame();

            _Screenshot();

            effects.messenger.gameObject.SetActive(false);

            if (home) home.gameObject.SetActive(false);
            if (inGame) inGame.gameObject.SetActive(false);
            if (pause) pause.gameObject.SetActive(false);

            gameOver.gameObject.SetActive(true);

            effects.SetPauseFader(1);
            
            // Deactivate the Skill Icons Canvas
            if (skillIconsCanvas != null && skillIconsCanvas.gameObject.activeInHierarchy)
            {
                skillIconsCanvas.gameObject.SetActive(false);
            }

            TimingHelper.Paused = true;
            
            // Get the player's score
            int playerScore = PlayerData.Get(0).scoreboard;

            // Call GameOverUI.ShowGameOver
            GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
            if (gameOverUI != null)
            {
                gameOverUI.ShowGameOver(playerScore);
            }
            
            // Save player progress
            PlayerProgression.Instance.SavePlayerProgress();
        }

        string _GetBackgroundScene()
        {
            return backgroundSceneName;
        }

        string _GetLevelIndex()
        {
            int index = PlayerPrefs.GetInt("levelIndex", 0);
            if (levelSettings.sceneName.Length == 0 || index >= levelSettings.sceneName.Length) return "";
            return levelSettings.sceneName[index];
        }

        string _GetNextLevel()
        {
            int levelIndex = PlayerPrefs.GetInt("levelIndex");
            if (levelIndex < 0 || levelIndex >= levelCount) levelIndex = 0;

            levelIndex += 1;
            if (levelIndex < 0 || levelIndex >= levelCount) levelIndex = 0;

            PlayerPrefs.SetInt("levelIndex", levelIndex);
            PlayerPrefs.Save();

            return levelSettings.sceneName[levelIndex];
        }

        void _Load()
        {
            switch (home)
            {
                case null:

                    StartCoroutine(Load(_StartGame, pause.gameObject, levelSettings.startPausedWhenGameIsUsedAsBackground, true, null, null, null, null));

                    break;

                default:

                    GameObject UI = levelSettings.DisableHomePageWhenGameIsUsedAsBackground ? pause.gameObject : home.gameObject;

                    switch (mode)
                    {
                        case Mode.UseBackgroundScene:

                            StartCoroutine(Load(_GetBackgroundScene, home.gameObject, false, true, null, null, null, _Screenshot));

                            break;

                        case Mode.UseGameAsBackground:

                            StartCoroutine(Load(_StartGame, UI, levelSettings.startPausedWhenGameIsUsedAsBackground, true, null, null, null, _Screenshot));

                            break;
                    }

                    break;
            }
        }

        IEnumerator _PauseButton()
        {
            yield return new WaitForEndOfFrame();

            _Screenshot();

            inGame.gameObject.SetActive(false);

            pause.gameObject.SetActive(true);

            effects.SetPauseFader(1);

            //TimingHelper.Paused = startPaused;
            TimingHelper.Paused = true;
            
            // Deactivate the Skill Icons Canvas
            if (skillIconsCanvas != null && skillIconsCanvas.gameObject.activeInHierarchy)
            {
                skillIconsCanvas.gameObject.SetActive(false);
            }
        }

        void _Screenshot()
        {
            if (_simpleShare) _simpleShare.OnScreenshot();
        }

        string _StartGame()
        {
            if (playerSettings.newGameEverytime) return ResetGame();

            PlayerData.Reset(playerSettings.lives);

            int levelIndex = PlayerPrefs.GetInt("levelIndex");

            if (levelIndex >= levelCount)
            {
                PlayerPrefs.SetInt("levelIndex", levelIndex);
                PlayerPrefs.Save();
            }

            return levelSettings.sceneName[levelIndex];
        }

#if UNITY_EDITOR
        public EditorBuildSettingsScene[] ScenesInBuildDialog()
        {
            List<EditorBuildSettingsScene> scenesInBuild = GetScenesInBuild();

            List<EditorBuildSettingsScene> missingScenes = GetAllMissingScenesInBuild();

            if (missingScenes.Count > 0)
            {
                bool result = EditorUtility.DisplayDialog("Problem with loading scenes.", "This could be because the scenes used are not added to Scenes In Build. \n\nDo you want the EasyGameUI to try and fix it? \n\nIf you answer with yes the EasyGameUI will try to add the scenes to the Unity 'Scenes In Build' and open the 'Build Settings' and display the results. \n\nYou must add the scenes manually if this doesn't solve your problem. \n\nThe EasyGameUI will quit in any case and you will need to run your program again.", "Yes, fix it", "No, will do this manually");

                if (result == true)
                {
                    scenesInBuild.AddRange(missingScenes);

                    EditorBuildSettings.scenes = scenesInBuild.ToArray();

                    return EditorBuildSettings.scenes;
                }
            }

            return null;
        }
        public List<EditorBuildSettingsScene> ScenesInBuild()
        {
            List<EditorBuildSettingsScene> scenesInBuild = GetScenesInBuild();

            List<EditorBuildSettingsScene> missingScenes = GetAllMissingScenesInBuild();

            scenesInBuild.AddRange(missingScenes);

            EditorBuildSettings.scenes = scenesInBuild.ToArray();

            return missingScenes;
        }

        public List<EditorBuildSettingsScene> GetScenesInBuild()
        {
            List<EditorBuildSettingsScene> scenesInBuild = new List<EditorBuildSettingsScene>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                scenesInBuild.Add(scene);

            return scenesInBuild;
        }

        public List<EditorBuildSettingsScene> GetAllMissingScenesInBuild()
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", searchInFolders);

            List<EditorBuildSettingsScene> allMissingScenes = GetMissingScenesInBuild(new string[] { Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path) });

            allMissingScenes.AddRange(GetMissingScenesInBuild(new string[] { backgroundSceneName }));

            if (advertisementSettings.allow && advertisementSettings.sceneName.Length > 0) allMissingScenes.AddRange(GetMissingScenesInBuild(advertisementSettings.sceneName));
            if (levelSettings.sceneName.Length > 0) allMissingScenes.AddRange(GetMissingScenesInBuild(levelSettings.sceneName));
            if (postLevelSettings.allow) allMissingScenes.AddRange(GetMissingScenesInBuild(postLevelSettings.sceneName));
            if (preLevelSettings.allow && preLevelSettings.sceneName.Length > 0) allMissingScenes.AddRange(GetMissingScenesInBuild(preLevelSettings.sceneName));

            return allMissingScenes;

            List<EditorBuildSettingsScene> GetMissingScenesInBuild(string[] scenes)
            {
                List<EditorBuildSettingsScene> missingScenes = new List<EditorBuildSettingsScene>();

                if (scenes == null) return missingScenes;

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    if (Wanted() && Contains() == false)
                    {
                        EditorBuildSettingsScene scene = new EditorBuildSettingsScene(path, true);
                        missingScenes.Add(scene);
                    }

                    bool Wanted()
                    {
                        foreach (string name in scenes)
                        {
                            string file = Path.GetFileNameWithoutExtension(path);
                            if (name != "" && name == file) return true;
                        }
                        return false;
                    }

                    bool Contains()
                    {
                        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                            if (scene.path == path) return true;

                        return false;
                    }
                }

                return missingScenes;
            }
        }
        void LateUpdate()
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(gameOverKey) && !isBusy) GameOver();
                if (Input.GetKeyDown(nextLevelKey) && !isBusy) LevelUp();
                if (Input.GetKeyDown(reloadLevelKey) && !isBusy) Reload();
                if (Input.GetKeyDown(resetGameKey)) ResetButton();
            }
        }
#endif
        static EasyGameUI _instance;

        AsyncOperation _asyncOperation;
        GameObject _back;
        GameObject _current;
        bool _intermission;
        About _about;
        Effects _effects;
        Home _home;
        GameOver _gameOver;
        InGame _inGame;
        Loader _loader;
        bool _loading;
        Action _onTimerDone;
        Pause _pause;
        RectTransform _pauseRect;
        Reset _reset;
        Settings _settings;
        SimpleShare _simpleShare;
        Shop _shop;
        float _timer;
        
        public void ShowSkillSelectionPanel()
        {
            if (skillSelectionPanel == null || skillButtonContainer == null) return;

            // Clear existing buttons
            foreach (var button in skillButtons)
            {
                Destroy(button);
            }
            skillButtons.Clear();

            List<string> availableSkills = new List<string>();

            // Check for super skills first
            var newSuperSkills = PlayerProgression.Instance.CheckNewlyUnlockedSuperSkills();
            foreach (var superSkillName in newSuperSkills)
            {
                AddSuperSkillOption(superSkillName);
                availableSkills.Add(superSkillName);
            }

            // Get all available regular skills and randomize them
            var availableRegularSkills = skills
                .Where(s => IsSkillAvailable(s.Value))
                .OrderBy(x => UnityEngine.Random.value)
                .Take(3 - availableSkills.Count)
                .ToList();

            // If no regular skills are available and no super skills, show single Extra Score option
            if (availableRegularSkills.Count == 0 && availableSkills.Count == 0)
            {
                // Create single Extra Score option
                GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);
                Image skillImage = skillButton.transform.Find("SkillImage").GetComponent<Image>();
                TMP_Text skillTitle = skillButton.transform.Find("SkillTextBackground/SkillTitle").GetComponent<TMP_Text>();

                if (skillImage != null)
                {
                    skillImage.sprite = extraScoreImage;
                    skillImage.color = Color.white;
                }

                if (skillTitle != null)
                {
                    skillTitle.text = "Extra Score";
                }

                Button buttonComponent = skillButton.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => OnSkillSelected("Extra Score"));
                }

                skillButtons.Add(skillButton);
            }
            else
            {
                // Add the randomly selected regular skills
                foreach (var skill in availableRegularSkills)
                {
                    // Create regular skill button
                    GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);
                    Image skillImage = skillButton.transform.Find("SkillImage").GetComponent<Image>();
                    TMP_Text skillTitle = skillButton.transform.Find("SkillTextBackground/SkillTitle").GetComponent<TMP_Text>();

                    if (skillImage != null)
                    {
                        skillImage.sprite = skill.Value.skillImage;
                        skillImage.color = Color.white;
                    }

                    if (skillTitle != null)
                    {
                        skillTitle.text = skill.Key;
                    }

                    Button buttonComponent = skillButton.GetComponent<Button>();
                    if (buttonComponent != null)
                    {
                        buttonComponent.onClick.AddListener(() => OnSkillSelected(skill.Key));
                    }

                    skillButtons.Add(skillButton);
                    availableSkills.Add(skill.Key);
                }
            }

            // Show the panel and pause the game
            skillSelectionPanel.SetActive(true);
            isSkillSelectionActive = true;
            Time.timeScale = 0;
            
            selectedSkillIndex = 0;
            HighlightSelectedSkill();
        }
        
        private void AddSuperSkillOption(string superSkillName)
        {
            if (superSkillButtonPrefab == null || skillButtonContainer == null) return;

            // Get the super skill data
            SuperSkill superSkill = PlayerProgression.Instance.availableSuperSkills[superSkillName];

            // Instantiate the super skill button
            GameObject skillButton = Instantiate(superSkillButtonPrefab, skillButtonContainer);
            
            // Get components
            Image skillImage = skillButton.GetComponentInChildren<Image>();
            TextMeshProUGUI skillTitle = skillButton.GetComponentInChildren<TextMeshProUGUI>();

            // Set the skill image and name
            if (skillImage != null)
            {
                skillImage.sprite = superSkill.icon;
                skillImage.color = Color.white;
            }

            if (skillTitle != null)
            {
                skillTitle.text = superSkill.name;
                skillTitle.color = Color.yellow; // Make super skills stand out
            }

            // Set the button's onClick event
            Button buttonComponent = skillButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => {
                    PlayerProgression.Instance.ActivateSuperSkill(superSkillName);
                    skillSelectionPanel.SetActive(false);
                    isSkillSelectionActive = false;
                    Time.timeScale = 1;
                });
            }

            skillButtons.Add(skillButton); // Add to the list of buttons
        }
        
        public void OnSkillSelected(string skillName)
        {
            
            Skill selectedSkill = null;
            
            if (skillName == "Extra Score")
            {
                selectedSkill = new Skill(null, 0f, 0, "", "Extra Score", extraScoreImage);
            }
            else
            {
                selectedSkill = skills[skillName];
                
                // Only add the full skill name (with Level X) to PlayerProgression
                if (!PlayerProgression.Instance.unlockedSkills.Contains(skillName))
                {
                    PlayerProgression.Instance.unlockedSkills.Add(skillName);
                    PlayerProgression.Instance.SavePlayerProgress();
                }
            }

            if (skillName == "Extra Score")
            {
                // Do not add to acquiredSkills since it's a one-time bonus
                // Close the skill selection panel
                skillSelectionPanel.SetActive(false);
                isSkillSelectionActive = false;
                
                // Apply the skill (add points)
                ApplySkill(selectedSkill);
                
                // Resume the game
                Time.timeScale = 1;
            }
            else
            {
                string baseSkillName = GetBaseSkillName(skillName);
                int index = acquiredSkills.FindIndex(s => GetBaseSkillName(s) == baseSkillName);

                if (index != -1)
                {
                    // Skill is already acquired; replace it with the new level
                    acquiredSkills[index] = skillName;
                }
                else
                {
                    // Add the new skill
                    acquiredSkills.Add(skillName);
                }

                // Update the skill icons display
                UpdateSkillIconsDisplay();

                // Close the skill selection panel
                skillSelectionPanel.SetActive(false);
                isSkillSelectionActive = false;

                // Apply the skill
                ApplySkill(selectedSkill);
                
                // Resume the game
                Time.timeScale = 1;
            }
        }
        
        void ApplySkill(Skill skill)
        {
            Time.timeScale = 1; // Unpause the game

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                if (skill.skillName.StartsWith("Main Gun Level"))
                {
                    // Existing code for main gun level
                    var bulletSpawner = player.GetComponent<BulletSpawner>();
                    if (bulletSpawner != null)
                    {
                        bulletSpawner.mainGunLevel = skill.level;
                    }
                }
                else if (skill.skillName.StartsWith("Angled Shots Level"))
                {
                    // Existing code for angled shots level
                    var bulletSpawner = player.GetComponent<BulletSpawner>();
                    if (bulletSpawner != null)
                    {
                        bulletSpawner.angledShotsLevel = skill.level;
                    }
                }
                else if (skill.skillName.StartsWith("Cannons Level"))
                {
                    // Implemented Cannon skill
                    var bulletSpawnersList = player.GetComponents<BulletSpawners>();
                    foreach (var spawner in bulletSpawnersList)
                    {
                        if (spawner.id == "Cannon")
                        {
                            spawner.cannonLevel = skill.level; // Use the level from the skill
                            spawner.timer.counter = -1; // Set counter to infinite firing
                            break; // Found the spawner, exit loop
                        }
                    }
                }
                else if (skill.skillName.StartsWith("3 Way Shooter Level"))
                {
                    // Increase 3 Way Shooter level
                    var bulletSpawnersList = player.GetComponents<BulletSpawners>();
                    foreach (var spawner in bulletSpawnersList)
                    {
                        if (spawner.id == "3 Way Shooter")
                        {
                            spawner.threeWayShooterLevel = skill.level; // Use the level from the skill
                            spawner.timer.counter = -1; // Set counter to infinite firing
                            break; // Found the spawner, exit loop
                        }
                    }
                }
                else if (skill.skillName.StartsWith("Speed Up Level"))
                {
                    // Use the 'player' variable declared earlier
                    var playerControls = player.GetComponent<PlayerControls>();
                    if (playerControls != null)
                    {
                        // Increase the speed
                        playerControls.IncreaseSpeed(skill.level);
                    }
                }
                else if (skill.skillName.StartsWith("Health Upgrade Level"))
                {
                    var collisionState = player.GetComponent<CollisionState>();
                    if (collisionState != null)
                    {
                        collisionState.IncreaseHealth(skill.level);
                    }
                }
                else if (skill.skillName.StartsWith("Homing Missile Level"))
                {
                    var missileSpawner = player.GetComponent<SimpleBulletSpawner>();
                    if (missileSpawner == null)
                    {
                        missileSpawner = player.AddComponent<SimpleBulletSpawner>();
                        missileSpawner.parent = null; // Set parent if needed
                        missileSpawner.position = Vector3.right * 1f; // Adjust spawn position
                        missileSpawner.scale = 1f; // Adjust scale if needed

                        // Initialize the timer
                        missileSpawner.timer = new Timer();
                    }

                    // Ensure the timer is initialized
                    if (missileSpawner.timer == null)
                    {
                        missileSpawner.timer = new Timer();
                    }

                    // Set the timer interval based on the skill's interval
                    missileSpawner.timer.interval = skill.interval;

                    // Set counter to infinite firing
                    missileSpawner.timer.counter = -1;

                    // Adjust missile properties based on the level
                    var missileScript = missileSpawner.prefab.GetComponent<Missile>();
                    if (missileScript != null)
                    {
                        missileScript.speed = skill.level;          // Increase speed per level
                        missileScript.rotationSpeed = 250f + (skill.level * 50f); // Increase rotation speed per level
                    }
                }
                else if (skill.skillName.StartsWith("Homing Gun Level"))
                {
                    var bulletSpawnersList = player.GetComponents<BulletSpawner>();
                    foreach (var spawner in bulletSpawnersList)
                    {
                        if (spawner.id == "Phaser")
                        {
                            // Set Phaser Shots Level
                            spawner.phaserShotsLevel = skill.level;

                            // Set mode to TargetEnemy
                            spawner.mode = BulletSpawner.Mode.TargetEnemy;
            
                            // Set counter to -1 for infinite firing
                            if (spawner.timer != null)
                            {
                                spawner.timer.counter = -1;
                            }

                            break;
                        }
                    }
                }
                else if (skill.skillName.StartsWith("Wrecking Ball Level"))
                {
                    var bulletSpawnersList = player.GetComponents<BulletSpawners>();
                    foreach (var spawner in bulletSpawnersList)
                    {
                        if (spawner.id == "Wrecking Ball")
                        {
                            spawner.wreckingBallLevel = skill.level; // Use the level from the skill
                            break; // Found the spawner, exit loop
                        }
                    }
                }
                else if (skill.skillName.StartsWith("Red Laser Level"))
                {
                    ApplyLaserSkill(player, skill, "genericBulletRed", "RedLaserSpawner");
                }
                else if (skill.skillName.StartsWith("Blue Laser Level"))
                {
                    ApplyLaserSkill(player, skill, "bulletBlue", "BlueLaserSpawner");
                }
                else if (skill.skillName.StartsWith("Green Laser Level"))
                {
                    ApplyLaserSkill(player, skill, "genericBulletGreen", "GreenLaserSpawner");
                }
                else if (skill.skillName.StartsWith("Purple Laser Level"))
                {
                    ApplyLaserSkill(player, skill, "bulletPurple", "PurpleLaserSpawner");
                }
                else if (skill.skillName.StartsWith("Shield Level"))
                {
                    var collisionState = player.GetComponent<CollisionState>();
                    if (collisionState != null)
                    {
                        collisionState.ActivateShield(skill.level);
                    }
                }
                else if (skill.skillName.StartsWith("Slow Enemies Level"))
                {
                    // Apply the slowdown effect
                    float slowDownPercentage = 0.1f * skill.level; // 5% per level
                    EnemyAI.globalSpeedMultiplier = 1.0f - slowDownPercentage;
                    if (EnemyAI.globalSpeedMultiplier < 0.0f) EnemyAI.globalSpeedMultiplier = 0.2f; // Ensure multiplier doesn't go below 0
                }
                else if (skill.skillName.StartsWith("Slow Enemy Bullets Level"))
                {
                    // Apply the slowdown effect to enemy bullets
                    float slowDownPercentage = 0.1f * skill.level; // 5% per level
                    BulletBase.enemyBulletSpeedMultiplier = 1.0f - slowDownPercentage;
                    if (BulletBase.enemyBulletSpeedMultiplier < 0.0f) BulletBase.enemyBulletSpeedMultiplier = 0.2f;
                }
                else if (skill.skillName.StartsWith("Weapon Speed Level"))
                {
                    // Update the weapon speed level
                    weaponSpeedLevel = skill.level;

                    // Calculate the weapon speed multiplier
                    float weaponSpeedMultiplier = 1.0f + 0.15f * weaponSpeedLevel; // 10% increase per level

                    // Set the static variable in BulletSpawnerBase
                    BulletSpawnerBase.weaponSpeedMultiplier = weaponSpeedMultiplier;

                    // Find the player's ship
                    var playerShip = GameObject.FindGameObjectWithTag("Player");
                    if (playerShip != null)
                    {
                        // Get all BulletSpawner components
                        var bulletSpawners = playerShip.GetComponentsInChildren<BulletSpawner>();
                        foreach (var spawner in bulletSpawners)
                        {
                            spawner.ApplyWeaponSpeedMultiplier();
                        }

                        // Get all BulletSpawners components
                        var bulletSpawners2 = playerShip.GetComponentsInChildren<BulletSpawners>();
                        foreach (var spawner in bulletSpawners2)
                        {
                            spawner.ApplyWeaponSpeedMultiplier();
                        }
                    }
                }
                else if (skill.skillName == "Extra Score")
                {
                    // Add +10,000 points to the player's scoreboard
                    PlayerData.Get(0).scoreboard += 10000;
                }
                else if (skill.skillName.StartsWith("Drone Level"))
                {
                    ApplyDroneSkill(skill.level);
                }
                else if (skill.skillName.StartsWith("Bouncing Shot Level"))
                {
                    var bulletSpawner = player.GetComponent<BulletSpawner>();
                    if (bulletSpawner != null)
                    {
                        bulletSpawner.randomBouncingShotLevel = skill.level;
                        bulletSpawner.ApplyRandomBouncingShotLevel();
                    }
                }
                else if (skill.skillName.StartsWith("Engine Fire Level"))
                {
                    var playerControls = player.GetComponent<PlayerControls>();
                    if (playerControls != null)
                    {
                        playerControls.engineFireLevel = skill.level;
                    }
                }
            }
        }
        
        void HighlightSelectedSkill()
        {
            for (int i = 0; i < skillButtons.Count; i++)
            {
                GameObject skillButton = skillButtons[i];
        
                // Get the outline component
                Outline skillButtonOutline = skillButton.GetComponent<Outline>();

                if (i == selectedSkillIndex)
                {
                    // Highlight the selected button by enabling the outline
                    if (skillButtonOutline != null)
                    {
                        skillButtonOutline.enabled = true; // Enable the outline
                    }
                }
                else
                {
                    // Disable the outline for unselected buttons
                    if (skillButtonOutline != null)
                    {
                        skillButtonOutline.enabled = false; // Disable the outline
                    }
                }
            }
        }
        
        void HandleSkillSelectionInput()
        {
            // Navigate left
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                selectedSkillIndex--;
                if (selectedSkillIndex < 0)
                {
                    selectedSkillIndex = skillButtons.Count - 1; // Wrap around to the last skill
                }
                HighlightSelectedSkill();
            }
            // Navigate right
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                selectedSkillIndex++;
                if (selectedSkillIndex >= skillButtons.Count)
                {
                    selectedSkillIndex = 0; // Wrap around to the first skill
                }
                HighlightSelectedSkill();
            }

            // Select the skill
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                // Simulate button click
                GameObject selectedButton = skillButtons[selectedSkillIndex];
                Button buttonComponent = selectedButton.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.Invoke();
                }
            }
        }
        
        void ApplyLaserSkill(GameObject player, Skill skill, string targetTag, string laserSpawnerTag)
        {
            // Find or add the correct LaserSpawner for the specific laser type
            var laserSpawner = player.GetComponents<LaserSpawner>()
                .FirstOrDefault(ls => ls.targetTag == targetTag) ?? player.AddComponent<LaserSpawner>();

            // Configure the LaserSpawner with the selected skill's properties
            ConfigureLaserSpawner(laserSpawner, skill);

            // Ensure the max charges are set properly in the PickupLaser component
            var pickupLaser = player.GetComponent<PickupLaser>();
            if (pickupLaser != null)
            {
                pickupLaser.TurnOnLaser(laserSpawner);
            }
        }
        
        void ConfigureLaserSpawner(LaserSpawner laserSpawner, Skill skill)
        {
            laserSpawner.prefab = skill.prefab;       // Assign the prefab
            laserSpawner.targetTag = skill.targetTag;  // Ensure the correct targetTag is set
            laserSpawner.timer.interval = skill.interval;  // Set the firing interval
        }
        
        bool IsSkillAvailable(Skill skill)
        {
            if (skill.skillName == "Extra Score")
            {
                return false; // Never make Extra Score directly available through normal skill selection
            }

            string baseSkillName = GetBaseSkillName(skill.skillName);

            // Get the highest level of the acquired skill for this base skill
            int acquiredSkillLevel = acquiredSkills
                .Where(s => GetBaseSkillName(s) == baseSkillName)
                .Select(s => GetSkillLevel(s))
                .DefaultIfEmpty(0)
                .Max();

            if (acquiredSkillLevel >= skill.level)
            {
                // The player already has this level or a higher level of the skill
                return false;
            }

            bool isUnlockedSkill = PlayerProgression.Instance.unlockedSkills.Contains(baseSkillName);
            bool isInitialSkill = PlayerProgression.Instance.initialSkills.Contains(baseSkillName);

            if (!PlayerProgression.Instance.HasPlayedGame())
            {
                if (isInitialSkill)
                {
                    // During first game, allow initial skills up to their max level
                    if (skill.level == acquiredSkillLevel + 1)
                    {
                        return true;
                    }
                }
                else
                {
                    // Other skills are not available during first game
                    return false;
                }
            }
            else
            {
                // After first game, allow unlocked skills
                if (isUnlockedSkill)
                {
                    // The skill is available if it's the next level up
                    if (skill.level == acquiredSkillLevel + 1)
                    {
                        return true;
                    }
                }
            }

            return false; // Skill is not available
        }
        
        int GetSkillLevel(string skillName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(skillName, @"^.* Level (\d+)$");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                return 0; // Return 0 if the skill name doesn't match the expected pattern
            }
        }


        
        void UpdateSkillIconsDisplay()
        {
            int totalSlots = 10;

            for (int i = 0; i < totalSlots; i++)
            {
                Transform slotTransform = skillIconsPanel.transform.GetChild(i);
                GameObject skillSlot = slotTransform.gameObject;

                // Get the components
                Image iconImage = skillSlot.transform.Find("SkillIconImage")?.GetComponent<Image>();
                TMP_Text levelText = skillSlot.transform.Find("LevelDisplayText")?.GetComponent<TMP_Text>();

                if (i < acquiredSkills.Count)
                {
                    // We have an acquired skill for this slot
                    string skillName = acquiredSkills[i];
                    if (skills.TryGetValue(skillName, out Skill skill))
                    {
                        // Set the skill icon image
                        if (iconImage != null)
                        {
                            iconImage.sprite = skill.skillImage;
                            iconImage.color = Color.white; // Make sure the icon is visible
                        }

                        // Display the current level over max level
                        if (levelText != null)
                        {
                            int currentLevel = GetSkillLevel(skillName);
                            int maxLevel = skill.maxLevel; // Or get from skill.maxLevel
                            levelText.text = $"{currentLevel} / {maxLevel}";
                        }
                    }
                }
                else
                {
                    // No acquired skill for this slot; set to empty slot image
                    if (iconImage != null)
                    {
                        iconImage.sprite = emptySkillSlotImage;
                        iconImage.color = Color.white; // Ensure the icon is visible
                    }

                    // Clear level text
                    if (levelText != null)
                    {
                        levelText.text = "";
                    }
                }
            }
        }


        
        string GetBaseSkillName(string skillName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(skillName, @"^(.*) Level \d+$");
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            else
            {
                return skillName;
            }
        }
        
        void InitializeAcquiredSkills()
        {
            weaponSpeedLevel = 0;
            BulletSpawnerBase.weaponSpeedMultiplier = 1.0f; // Reset the multiplier
            // Reset enemy speed multiplier
            EnemyAI.globalSpeedMultiplier = 1.0f;
            BulletBase.enemyBulletSpeedMultiplier = 1.0f;
            
            // Clear any existing skills
            acquiredSkills.Clear();

            // Add the starting skill(s)
            acquiredSkills.Add("Main Gun Level 1");
            // acquiredSkills.Add("Red Laser Level 4");
            // acquiredSkills.Add("Blue Laser Level 4");
            // acquiredSkills.Add("Green Laser Level 4");
            // acquiredSkills.Add("Purple Laser Level 4");
            acquiredSkills.Add("Angled Shots Level 1");
            acquiredSkills.Add("Cannons Level 1");
            acquiredSkills.Add("3 Way Shooter Level 1");
            // acquiredSkills.Add("Speed Up Level 5");
            // acquiredSkills.Add("Health Upgrade Level 4");
            // acquiredSkills.Add("Homing Missile Level 5");
            acquiredSkills.Add("Homing Gun Level 1");
            // acquiredSkills.Add("Wrecking Ball Level 5");
            // acquiredSkills.Add("Shield Level 4");
            // acquiredSkills.Add("Slow Enemies Level 5");
            // acquiredSkills.Add("Slow Enemy Bullets Level 5");
            // acquiredSkills.Add("Weapon Speed Level 5");
            acquiredSkills.Add("Drone Level 1");
            // acquiredSkills.Add("Bouncing Shot Level 1");
            // acquiredSkills.Add("Engine Fire Level 5");

            // Update the skill icons display
            UpdateSkillIconsDisplay();
        }
        
        void ApplyDroneSkill(int level)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                // Find existing drone, if any
                var existingDrone = GameObject.FindWithTag("Drone");
                if (existingDrone == null)
                {
                    // No drone exists; summon a new drone
                    SummonDrone(player, level);
                }
                else
                {
                    // Drone exists; update its level
                    var droneScript = existingDrone.GetComponent<Drone>();
                    if (droneScript != null)
                    {
                        droneScript.SetDroneLevel(level);
                    }
                }

                // Update the acquired skills list
                string skillName = $"Drone Level {level}";
                string baseSkillName = GetBaseSkillName(skillName);
                int index = acquiredSkills.FindIndex(s => GetBaseSkillName(s) == baseSkillName);
                if (index != -1)
                {
                    acquiredSkills[index] = skillName;
                }
                else
                {
                    acquiredSkills.Add(skillName);
                }
                UpdateSkillIconsDisplay();
            }
        }
        
        void SummonDrone(GameObject player, int level)
        {
            if (dronePrefab == null)
            {
                return;
            }

            // Instantiate the drone prefab
            var droneInstance = Instantiate(dronePrefab, player.transform.position, Quaternion.Euler(0, 0, 90));
            droneInstance.tag = "Drone"; // Tag the drone for easy lookup

            var droneScript = droneInstance.GetComponent<Drone>();
            if (droneScript != null)
            {
                droneScript.controller = player;
                droneScript.SetDroneLevel(level); // Set initial level
            }
        }

        // Add to existing class
        public void DisableSkill(string skillName)
        {
            // Find all spawners with this skill and disable them
            var bulletSpawners = FindObjectsOfType<BulletSpawnerBase>();
            foreach (var spawner in bulletSpawners)
            {
                if (spawner.id == skillName)
                {
                    spawner.timer.counter = 0; // Disable firing
                }
            }
        }

        public void ActivateSuperSkill(string superSkillName)
        {
            if (superSkillName == "Guns Blazing")
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    var allSpawners = player.GetComponentsInChildren<BulletSpawner>();
                    foreach (var spawner in allSpawners)
                    {
                        // First disable Angled Shots and 3 Way Shooter
                        if (spawner.id == "Angled Shots" || spawner.id == "3 Way Shooter")
                        {
                            spawner.timer.counter = 0; // Disable these skills
                        }
                        // Then enable Guns Blazing
                        else if (spawner.id == "Guns Blazing")
                        {
                            spawner.timer.counter = -1; // Enable infinite firing
                        }
                    }
                }
            }
        }

    }
}

[System.Serializable]
public class Skill
{
    public GameObject prefab;
    public float interval;
    public int maxCharges;
    public string targetTag;
    public string skillName;
    public Sprite skillImage;
    public int level;    // Current level of the skill
    public int maxLevel; // Max level of the skill

    public Skill(GameObject prefab, float interval, int maxCharges, string targetTag, string skillName, Sprite skillImage, int level = 0, int maxLevel = 5)
    {
        this.prefab = prefab;
        this.interval = interval;
        this.maxCharges = maxCharges;
        this.targetTag = targetTag;
        this.skillName = skillName;
        this.skillImage = skillImage;
        this.level = level;
        this.maxLevel = maxLevel;
    }
}
