using System;
using UnityEngine;

namespace Playniax.Ignition
{
    public class StateManager : MonoBehaviour
    {
        [System.Serializable]
        public class Base
        {
            public float delay = .5f;
            public float sustain = .75f;
            public float scaleStep = .025f;
            public Font font;
            public int fontSize = 48;
            public Color color = Color.white;
        }

        [System.Serializable]
        public class GameCompletedSettings : Base
        {
            [Multiline]
            public string text = "CONGRTULATIONS!,GAME COMPLETED!";
        }

        [System.Serializable]
        public class IntroSettings : Base
        {
            [Multiline]
            public string text = "GET READY FOR\nLEVEL %LEVEL%";
        }

        [System.Serializable]
        public class LevelCompletedSettings : Base
        {
            [Multiline]
            public string text = "LEVEL %LEVEL%\nCOMPLETED";
        }

        [System.Serializable]
        public class ReplaySettings : Base
        {
            [Multiline]
            public string text = "FAIL!,TRY AGAIN!";
        }

        [System.Serializable]
        public class GameOverSettings
        {
            public float delay = 1.5f;
        }

        public IntroSettings introSettings;
        public LevelCompletedSettings levelCompletedSettings;
        public GameCompletedSettings gameCompletedSettings;
        public ReplaySettings replaySettings;
        public GameOverSettings gameOverSettings;
        public static StateManager instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<StateManager>();

                return _instance;
            }
        }
        public bool isMessengerBusy
        {
            get
            {
                if (_messengerState != 0) return true; else return false;
            }
        }
        public virtual void OnGameCompleted()
        {
            if (EasyGameUI.instance) _Message(gameCompletedSettings, gameCompletedSettings.delay, gameCompletedSettings.text, EasyGameUI.instance.LevelUp);
        }

        public virtual void OnGameOver()
        {
            if (EasyGameUI.instance) EasyGameUI.instance.GameOver(gameOverSettings.delay);
        }

        public virtual void OnOneDown()
        {
            if (EasyGameUI.instance) _Message(replaySettings, replaySettings.delay, replaySettings.text, EasyGameUI.instance.Reload);
        }

        public virtual void OnLevelCompleted()
        {
            if (EasyGameUI.instance) _Message(levelCompletedSettings, levelCompletedSettings.delay, levelCompletedSettings.text, EasyGameUI.instance.LevelUp);
        }

        public virtual bool isGameOver
        {
            get
            {
                if (PlayerGroup.GetList().Count == 0 && PlayerData.CountLives() == 0 && EasyGameUI.instance != null && EasyGameUI.instance.gameOver && EasyGameUI.instance.gameOver.isActiveAndEnabled == false && EasyGameUI.instance.inGame.isActiveAndEnabled == true) return true;

                return false;
            }
        }

        public virtual bool isKilled
        {
            get
            {
                if (PlayerGroup.GetList().Count == 0 && PlayerData.CountLives() > 0) return true;

                return false;
            }
        }

        public virtual bool isLastLevel
        {
            get
            {
                if (EasyGameUI.instance != null) return EasyGameUI.instance.isLastLevel;

                return false;
            }
        }
        public virtual bool isLevelCompleted
        {
            get
            {
                if (EasyGameUI.instance != null) return EasyGameUI.instance.isLevelCompleted;

                return false;
            }
        }
        void Start()
        {
            if (EasyGameUI.instance)
            {
                gameCompletedSettings.text = _Fetch(gameCompletedSettings.text);
                introSettings.text = _Fetch(introSettings.text);
                levelCompletedSettings.text = _Fetch(levelCompletedSettings.text);

                if (EasyGameUI.instance.effects.messenger.isActiveAndEnabled == false)
                {
                    _Message(introSettings, introSettings.delay, introSettings.text);
                }
            }
        }
        void Update()
        {
#if UNITY_EDITOR
            _TestKeys();
#endif
            _UpdateMonitor();
            _UpdateMessenger();
        }

        string _Fetch(string text)
        {
            if (EasyGameUI.instance)
            {
                text = text.Replace("%LEVEL%", (EasyGameUI.instance.levelIndex + 1).ToString());
            }
            else
            {
                text = text.Replace("%LEVEL%", "0");
            }

            return text;
        }

        void _Message(Base mode, float delay, string message, Action OnMessageDone = null)
        {
            if (EasyGameUI.instance)
            {
                EasyGameUI.instance.effects.messenger.gameObject.SetActive(false);

                _index = 0;
                _messengerState = 0;

                _mode = mode;
                _timer = delay;
                _sequence = message.Split(","[0]);
                _onMessageDone = OnMessageDone;
            }
        }

        void _UpdateMonitor()
        {
            if (_monitoringSuspended == false)
            {
                if (isGameOver == true)
                {
                    OnGameOver();

                    _monitoringSuspended = true;
                }
                else if (isKilled == true)
                {
                    OnOneDown();

                    _monitoringSuspended = true;
                }
                else if (isLevelCompleted)
                {
                    if (isLastLevel)
                    {
                        OnGameCompleted();

                        _monitoringSuspended = true;
                    }
                    else
                    {
                        OnLevelCompleted();

                        _monitoringSuspended = true;
                    }
                }
            }
        }

        void _TestKeys()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                var settings = levelCompletedSettings;

                _Message(settings, settings.delay, settings.text, EasyGameUI.instance.LevelUp);
            }
        }

        void _UpdateMessenger()
        {
            if (_messengerState == 0 && EasyGameUI.instance && EasyGameUI.instance.effects.messenger.isActiveAndEnabled == false && _mode != null && _sequence != null && _sequence.Length > 0)
            {
                if (_timer > 0)
                {
                    _timer -= 1 * Time.deltaTime;
                }
                else
                {
                    EasyGameUI.instance.effects.Message(EasyGameUI.GetFont(_mode.font), _mode.fontSize, _mode.color, _sequence[_index], _mode.sustain, _mode.scaleStep);

                    _index += 1;

                    if (_index >= _sequence.Length)
                    {
                        _index = 0;
                        _mode = null;
                        _timer = 0;
                        _sequence = null;

                        _messengerState = 1;
                    }
                }
            }

            if (_messengerState == 1 && EasyGameUI.instance && EasyGameUI.instance.effects.messenger.isActiveAndEnabled == false)
            {
                if (_onMessageDone != null)
                {
                    _onMessageDone();
                    _onMessageDone = null;
                }

                _messengerState = 0;
            }
        }

        static StateManager _instance;

        int _index;
        int _messengerState;
        Base _mode;
        bool _monitoringSuspended;
        Action _onMessageDone;
        string[] _sequence;
        float _timer;
    }
}
