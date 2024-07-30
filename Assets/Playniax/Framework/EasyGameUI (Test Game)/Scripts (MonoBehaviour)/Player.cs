using UnityEngine;

namespace Playniax.Ignition.TestGame
{

    public class Player : MonoBehaviour
    {
        public float maxSpeed = 3;
        public float jumpHeight = 14;
        public float mainColliderRadius = .4f;
        public Collider2D mainCollider;
        public Collider2D secondCollider;
        public AudioProperties collectableSound;
        public AudioProperties dieSound;

        public void GameOver()
        {
            if (EasyGameUI.instance) EasyGameUI.instance.GameOver();        // Call this for Game Over
        }

        public void Outro()
        {
            dieSound.Play();

            mainCollider.enabled = false;
            secondCollider.enabled = false;
            _rigidbody.velocity = Vector3.up * 10;
            _state = 1;
        }

        public void LevelUp()
        {
            if (EasyGameUI.instance) EasyGameUI.instance.LevelUp();         // Call this when the player successfully finishes a level
        }

        public void Score()
        {
            PlayerData.Get(0).scoreboard += 10;                             // Use the built-in counter to keep track of how much points the player scores
        }

        void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (_state == 0)
            {
                _isGrounded = _IsGrounded();

                _direction = Input.GetAxis("Horizontal");

                if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow)) && _isGrounded == true)
                {
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpHeight);
                }

                if (Mathf.Abs(_direction) < Mathf.Abs(_previousDirection)) _breaking = true;

                _Collisions();

                if (CameraHelpers.IsVisible(_renderer) == false)
                {
                    dieSound.Play();

                    _timer = 1;
                    _state = 2;
                }

                _previousDirection = _direction;
            }
            if (_state == 1)
            {
                transform.localScale += Vector3.one * Time.deltaTime;

                if (CameraHelpers.IsVisible(_renderer) == false)
                {
                    _timer = 1;
                    _state = 2;
                }
            }
            if (_state == 2)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0)
                {
                    PlayerData.Get(0).lives -= 1;                                   // Use the built-in counter to keep track of how many lifes the player has left

                    if (PlayerData.Get(0).lives <= 0)
                    {
                        Destroy(gameObject);

                        GameOver();                                                 // No lifes left? Call GameOver
                    }
                    else
                    {
                        _Restore();
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (_state == 0)
            {
                _isGrounded = _IsGrounded();

                if (_isGrounded == null)
                {
                    float move = Mathf.Clamp(_direction * 5, -1, 1);

                    move = Mathf.Clamp(move * 10, -20, 20);

                    if (_breaking == false) _rigidbody.AddRelativeForce(new Vector3(move, 0, 0));
                }
                else
                {
                    float move = Mathf.Clamp(_direction * 5, -1, 1);

                    move = Mathf.Clamp(move * 40, -40, 40);

                    if (_breaking == false) _rigidbody.AddRelativeForce(new Vector3(move, 0, 0));
                }

                _breaking = false;
            }
        }

        void _Collisions()
        {
            Collider2D[] colliders = FindObjectsOfType<Collider2D>();

            if (colliders.Length == 0) return;

            for (int i = 0; i < colliders.Length; i++)
            {
                Collider2D collider = colliders[i];

                if (collider == mainCollider) continue;
                if (collider == secondCollider) continue;
                if (collider.Distance(mainCollider).isOverlapped == false) continue;

                Collectable collectable = collider.GetComponent<Collectable>();

                Danger danger = collider.GetComponent<Danger>();

                if (collectable != null)
                {
                    Destroy(collider.gameObject);

                    PlayerData.Get(0).scoreboard += collectable.points;

                    collectableSound.Play();

                    PlayerData.Get(0).collectables -= 1;

                    if (PlayerData.Get(0).collectables <= 0) LevelUp();
                }
                else if (danger != null)
                {
                    Outro();

                    return;
                }
            }
        }

        Collider2D _IsGrounded()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, mainColliderRadius);

            if (colliders.Length == 0) return null;

            for (int i = 0; i < colliders.Length; i++)
            {
                Collider2D collider = colliders[i];

                if (collider.GetComponent<Collectable>() != null) continue;

                if (collider != mainCollider && collider != secondCollider && collider.Distance(secondCollider).isOverlapped) return null;
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                Collider2D collider = colliders[i];

                if (collider.GetComponent<Collectable>() != null) continue;

                if (collider != mainCollider && collider != secondCollider && collider.Distance(mainCollider).isOverlapped) return collider;
            }

            return null;
        }

        void _Restore()
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;

            mainCollider.enabled = true;
            secondCollider.enabled = true;
            _rigidbody.velocity = Vector3.zero;

            _state = 0;
        }

        bool _breaking;
        float _direction = 0;
        Collider2D _isGrounded;
        float _previousDirection;
        SpriteRenderer _renderer;
        Rigidbody2D _rigidbody;
        int _state;
        float _timer;
    }
}