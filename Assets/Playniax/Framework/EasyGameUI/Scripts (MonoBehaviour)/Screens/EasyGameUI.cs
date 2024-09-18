#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
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

        public GameObject redLaserPrefab;  // Assign in Inspector
        public GameObject blueLaserPrefab; // Assign in Inspector
        public GameObject greenLaserPrefab;  // Assign in Inspector
        public GameObject purpleLaserPrefab; // Assign in Inspector
        public Sprite mainGunImage; // Assign in Inspector
        public Sprite angledShotsImage; // Assign in Inspector
        public Sprite cannonImage; // Assign in Inspector
        public Sprite threeWayShooterImage; // Assign in Inspector
        public Sprite speedUpImage; // Assign in Inspector
        public Sprite increaseHealthImage ; // Assign in Inspector

        public Sprite redLaserImage;  // Assign in Inspector (Image for Red Laser)
        public Sprite blueLaserImage; // Assign in Inspector (Image for Blue Laser)
        public Sprite greenLaserImage;  // Assign in Inspector (Image for Green Laser)
        public Sprite purpleLaserImage; // Assign in Inspector (Image for Purple Laser)
        

        private Dictionary<string, Skill> skills;
        private HashSet<string> acquiredSkills = new HashSet<string>(); // Tracks acquired skills
        private Skill selectedSkill;
        private List<GameObject> skillButtons = new List<GameObject>(); // To keep track of instantiated buttons
        
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
            }
        }

        // Is called when player selects the replay button.
        public void ReplayButton()
        {
            StartCoroutine(Load(_StartGame, inGame.gameObject, false, true, advertisementSettings.sceneName, preLevelSettings.sceneName, null, null));
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
            skills = new Dictionary<string, Skill>
            {
                { "Red Laser Level 1", new Skill(redLaserPrefab, 1f, 10, "genericBulletRed", "Red Laser Level 1", redLaserImage) },
                { "Red Laser Level 2", new Skill(redLaserPrefab, 0.7f, 30, "genericBulletRed", "Red Laser Level 2", redLaserImage) },
                { "Red Laser Level 3", new Skill(redLaserPrefab, 0.4f, 50, "genericBulletRed", "Red Laser Level 3", redLaserImage) },
                { "Red Laser Level 4", new Skill(redLaserPrefab, 0.2f, 100, "genericBulletRed", "Red Laser Level 4", redLaserImage) },
                { "Blue Laser Level 1", new Skill(blueLaserPrefab, 1.0f, 10, "bulletBlue", "Blue Laser Level 1", blueLaserImage) },
                { "Blue Laser Level 2", new Skill(blueLaserPrefab, 0.7f, 30, "bulletBlue", "Blue Laser Level 2", blueLaserImage) },
                { "Blue Laser Level 3", new Skill(blueLaserPrefab, 0.4f, 50, "bulletBlue", "Blue Laser Level 3", blueLaserImage) },
                { "Blue Laser Level 4", new Skill(blueLaserPrefab, 0.2f, 100, "bulletBlue", "Blue Laser Level 4", blueLaserImage) },
                { "Green Laser Level 1", new Skill(greenLaserPrefab, 1.0f, 10, "genericBulletGreen", "Green Laser Level 1", greenLaserImage) },
                { "Green Laser Level 2", new Skill(greenLaserPrefab, 0.7f, 30, "genericBulletGreen", "Green Laser Level 2", greenLaserImage) },
                { "Green Laser Level 3", new Skill(greenLaserPrefab, 0.4f, 50, "genericBulletGreen", "Green Laser Level 3", greenLaserImage) },
                { "Green Laser Level 4", new Skill(greenLaserPrefab, 0.2f, 100, "genericBulletGreen", "Green Laser Level 4", greenLaserImage) },
                { "Purple Laser Level 1", new Skill(purpleLaserPrefab, 1.0f, 10, "bulletPurple", "Purple Laser Level 1", purpleLaserImage) },
                { "Purple Laser Level 2", new Skill(purpleLaserPrefab, 0.7f, 30, "bulletPurple", "Purple Laser Level 2", purpleLaserImage) },
                { "Purple Laser Level 3", new Skill(purpleLaserPrefab, 0.4f, 50, "bulletPurple", "Purple Laser Level 3", purpleLaserImage) },
                { "Purple Laser Level 4", new Skill(purpleLaserPrefab, 0.2f, 100, "bulletPurple", "Purple Laser Level 4", purpleLaserImage) },
            };
            
            skills.Add("Main Gun Level 1", new Skill(null, 0f, 0, "", "Main Gun Level 1", mainGunImage, 1));
            skills.Add("Main Gun Level 2", new Skill(null, 0f, 0, "", "Main Gun Level 2", mainGunImage, 2));
            skills.Add("Main Gun Level 3", new Skill(null, 0f, 0, "", "Main Gun Level 3", mainGunImage, 3));
            skills.Add("Main Gun Level 4", new Skill(null, 0f, 0, "", "Main Gun Level 4", mainGunImage, 4));
            skills.Add("Main Gun Level 5", new Skill(null, 0f, 0, "", "Main Gun Level 5", mainGunImage, 5));
            
            skills.Add("Angled Shots Level 1", new Skill(null, 0f, 0, "", "Angled Shots Level 1", angledShotsImage, 1));
            skills.Add("Angled Shots Level 2", new Skill(null, 0f, 0, "", "Angled Shots Level 2", angledShotsImage, 2));
            skills.Add("Angled Shots Level 3", new Skill(null, 0f, 0, "", "Angled Shots Level 3", angledShotsImage, 3));
            skills.Add("Angled Shots Level 4", new Skill(null, 0f, 0, "", "Angled Shots Level 4", angledShotsImage, 4));
            skills.Add("Angled Shots Level 5", new Skill(null, 0f, 0, "", "Angled Shots Level 5", angledShotsImage, 5));
            
            skills.Add("Cannons Level 1", new Skill(null, 0f, 0, "", "Cannons Level 1", cannonImage, 1));
            skills.Add("Cannons Level 2", new Skill(null, 0f, 0, "", "Cannons Level 2", cannonImage, 2));
            skills.Add("Cannons Level 3", new Skill(null, 0f, 0, "", "Cannons Level 3", cannonImage, 3));
            skills.Add("Cannons Level 4", new Skill(null, 0f, 0, "", "Cannons Level 4", cannonImage, 4));
            skills.Add("Cannons Level 5", new Skill(null, 0f, 0, "", "Cannons Level 5", cannonImage, 5));
            
            // Add 3 Way Shooter skills
            skills.Add("3 Way Shooter Level 1", new Skill(null, 0f, 0, "", "3 Way Shooter Level 1", threeWayShooterImage, 1));
            skills.Add("3 Way Shooter Level 2", new Skill(null, 0f, 0, "", "3 Way Shooter Level 2", threeWayShooterImage, 2));
            skills.Add("3 Way Shooter Level 3", new Skill(null, 0f, 0, "", "3 Way Shooter Level 3", threeWayShooterImage, 3));
            skills.Add("3 Way Shooter Level 4", new Skill(null, 0f, 0, "", "3 Way Shooter Level 4", threeWayShooterImage, 4));
            
            skills.Add("Speed Up Level 1", new Skill(null, 0f, 0, "", "Speed Up Level 1", speedUpImage, 1));
            skills.Add("Speed Up Level 2", new Skill(null, 0f, 0, "", "Speed Up Level 2", speedUpImage, 2));
            skills.Add("Speed Up Level 3", new Skill(null, 0f, 0, "", "Speed Up Level 3", speedUpImage, 3));
            skills.Add("Speed Up Level 4", new Skill(null, 0f, 0, "", "Speed Up Level 4", speedUpImage, 4));
            skills.Add("Speed Up Level 5", new Skill(null, 0f, 0, "", "Speed Up Level 5", speedUpImage, 5));
            
            skills.Add("Health Upgrade Level 1", new Skill(null, 0f, 0, "", "Health Upgrade Level 1", increaseHealthImage, 1));
            skills.Add("Health Upgrade Level 2", new Skill(null, 0f, 0, "", "Health Upgrade Level 2", increaseHealthImage, 2));
            skills.Add("Health Upgrade Level 3", new Skill(null, 0f, 0, "", "Health Upgrade Level 3", increaseHealthImage, 3));
            skills.Add("Health Upgrade Level 4", new Skill(null, 0f, 0, "", "Health Upgrade Level 4", increaseHealthImage, 4));
            skills.Add("Health Upgrade Level 5", new Skill(null, 0f, 0, "", "Health Upgrade Level 5", increaseHealthImage, 5));
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

            TimingHelper.Paused = true;
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
            Time.timeScale = 0; // Pause the game
            skillSelectionPanel.SetActive(true);

            // Get a list of skills and filter out Level 2+ skills if their previous level isn't acquired
            List<Skill> availableSkills = skills.Values
                .Where(skill => IsSkillAvailable(skill))
                .OrderBy(x => UnityEngine.Random.value)
                .Take(3)
                .ToList();

            // Clear any existing buttons
            foreach (var button in skillButtons)
            {
                Destroy(button);
            }
            skillButtons.Clear();

            // Create and display the buttons for the three selected skills
            foreach (var skill in availableSkills)
            {
                GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);
                skillButton.GetComponentInChildren<TMP_Text>().text = skill.skillName;
                skillButton.GetComponent<Image>().sprite = skill.skillImage; // Assign the correct image to the button
                skillButton.GetComponent<Button>().onClick.AddListener(() => OnSkillSelected(skill.skillName));
                skillButtons.Add(skillButton);
            }
        }
        
        void OnSkillSelected(string skillName)
        {
            Skill selectedSkill = skills[skillName];
            acquiredSkills.Add(skillName);  // Mark the skill as acquired
            skillSelectionPanel.SetActive(false);
            ApplySkill(selectedSkill);
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
                else
                {
                    // Existing code for laser spawners
                    var laserSpawner = player.GetComponents<LaserSpawner>()
                        .FirstOrDefault(ls => ls.targetTag == skill.targetTag) ?? player.AddComponent<LaserSpawner>();

                    // Configure the laser spawner with the selected skill's properties
                    ConfigureLaserSpawner(laserSpawner, skill);

                    // Ensure the max charges are set properly in the PickupLaser component
                    var pickupLaser = player.GetComponent<PickupLaser>();
                    if (pickupLaser != null)
                    {
                        pickupLaser.max = skill.maxCharges;
                        pickupLaser.IncreaseLaserCharges(laserSpawner);
                    }
                }
            }
        }
        
        void ConfigureLaserSpawner(LaserSpawner laserSpawner, Skill skill)
        {
            laserSpawner.prefab = skill.prefab;      // Assign the prefab
            laserSpawner.targetTag = skill.targetTag;
            laserSpawner.timer.interval = skill.interval;
        }
        
        bool IsSkillAvailable(Skill skill)
        {
            // Exclude skills that have already been acquired
            if (acquiredSkills.Contains(skill.skillName))
            {
                return false;
            }

            // Handle skills dynamically based on their level and prerequisites
            var match = System.Text.RegularExpressions.Regex.Match(skill.skillName, @"^(.*) Level (\d+)$");
            if (match.Success)
            {
                string baseSkillName = match.Groups[1].Value;
                int level = int.Parse(match.Groups[2].Value);

                if (level == 1)
                {
                    return true; // Level 1 skills are available if not acquired yet
                }
                else
                {
                    string prerequisiteSkill = $"{baseSkillName} Level {level - 1}";
                    return acquiredSkills.Contains(prerequisiteSkill);
                }
            }
            else
            {
                // Skill name doesn't match expected pattern, exclude it
                return false;
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
    public int level; // Level of the skill

    public Skill(GameObject prefab, float interval, int maxCharges, string targetTag, string skillName, Sprite skillImage, int level = 0)
    {
        this.prefab = prefab;
        this.interval = interval;
        this.maxCharges = maxCharges;
        this.targetTag = targetTag;
        this.skillName = skillName;
        this.skillImage = skillImage;
        this.level = level; // Initialize the level
    }
}