using System.Runtime.InteropServices;
using UnityEngine;

namespace Playniax.Pyro
{
    [System.Serializable]
    public class SpriteState
    {
        public Animation[] animations;

        [System.Serializable]
        public class Animation
        {
            public string name = "Animation";
            public Sprite[] sprites;
            public float frameTime;
            public SpriteRenderer spriteRenderer;

            public Animation()
            {
                _pingPong = 1;
            }

            public Animation(string name)
            {
                this.name = name;
            }
            public bool Play(bool backwards = false)
            {
                return Play(spriteRenderer, frameTime, backwards);
            }
            public bool Play(float frameTime, bool backwards = false)
            {
                return Play(spriteRenderer, frameTime, backwards);
            }
            public bool Play(SpriteRenderer spriteRenderer, float frameTime, bool backwards = false)
            {
                var cycle = false;

                _frameTime += Time.deltaTime;

                if (_frameTime >= frameTime)
                {
                    _frameTime = 0;

                    _frame += backwards ? -1 : 1;
                }

                if (_frame < 0)
                {
                    _frame = sprites.Length - 1;

                    cycle = true;
                }
                else if (_frame > sprites.Length - 1)
                {
                    _frame = 0;

                    cycle = true;
                }

                spriteRenderer.sprite = sprites[_frame];

                return cycle;
            }
            public bool PlayOnce(bool backwards = false)
            {
                return PlayOnce(spriteRenderer, frameTime, backwards);
            }
            public bool PlayOnce(float frameTime, bool backwards = false)
            {
                return PlayOnce(spriteRenderer, frameTime, backwards);
            }
            public bool PlayOnce(SpriteRenderer spriteRenderer, float frameTime, bool backwards = false)
            {
                _frameTime += Time.deltaTime;

                if (_frameTime >= frameTime)
                {
                    _frameTime = 0;

                    _frame += backwards ? -1 : 1;
                }

                if (_frame < 0)
                {
                    _frame = 0;

                    return true;
                }
                else if (_frame > sprites.Length - 1)
                {
                    _frame = sprites.Length - 1;

                    return true;
                }

                spriteRenderer.sprite = sprites[_frame];

                return false;
            }
            public void PlayPingPong()
            {
                PlayPingPong(spriteRenderer, frameTime);
            }

            public void PlayPingPong(float frameTime)
            {
                PlayPingPong(spriteRenderer, frameTime);
            }
            public void PlayPingPong(SpriteRenderer spriteRenderer, float frameTime)
            {
                _frameTime += Time.deltaTime;

                if (_frameTime >= frameTime)
                {
                    _frameTime = 0;

                    _frame += _pingPong;
                }

                if (_frame < 0)
                {
                    _frame = 0;

                    _pingPong = -_pingPong;
                }
                else if (_frame > sprites.Length - 1)
                {
                    _frame = sprites.Length - 1;

                    _pingPong = -_pingPong;
                }

                spriteRenderer.sprite = sprites[_frame];
            }
            public int GetFrame()
            {
                return _frame % sprites.Length;
            }
            public int GetFrames()
            {
                return sprites.Length;
            }
            public Sprite GetSprite()
            {
                return sprites[_frame];
            }
            public Sprite GetSprite(int frame)
            {
                return sprites[frame];
            }
            public bool isLastFrame
            {
                get
                {
                    if (_frame == sprites.Length - 1) return true;
                    return false;
                }
            }
            public void SetFrame(int frame)
            {
                _frame = frame % sprites.Length;

                spriteRenderer.sprite = sprites[_frame];
            }

            public void SetFrame(SpriteRenderer spriteRenderer, int frame)
            {
                _frame = frame % sprites.Length;

                spriteRenderer.sprite = sprites[_frame];
            }

            public void Reset()
            {
                _frame = 0;
                _frameTime = 0;
                //_pingPong = 1;
            }

            int _frame;
            float _frameTime;
            int _pingPong;
        }

        public SpriteState(Animation[] animations)
        {
            this.animations = animations;
        }
        public Animation Find(string name)
        {
            for (int i = 0; i < animations.Length; i++)
            {
                if (animations[i] != null && animations[i].name == name)
                {
                    //if (_find != name) animations[i].Reset();

                    //_find = name;

                    return animations[i];
                }
            }
            return null;
        }

        //string _find;
    }
}
