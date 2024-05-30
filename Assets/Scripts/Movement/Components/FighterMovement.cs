using System;
using UnityEngine;

namespace Movement.Components
{
    //[RequireComponent(typeof(Rigidbody2D)),
     //RequireComponent(typeof(Animator))]
    public sealed class FighterMovement : MonoBehaviour, IMoveableReceiver, IJumperReceiver, IFighterReceiver
    {
        public float speed = 1.0f;
        public float airSpeed = 0.2f;
        public float jumpForce = 1.0f;
        public float downForce = 2.0f;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private Transform _feet;
        private LayerMask _floor;

        [Range(-1, 1)]
        public int _direction = 0;
        private bool _grounded = true;

        public delegate void _ReciveDamage();
        public event _ReciveDamage reciveDamage;

        private float invulneravilityTime = 1;
        
        private static readonly int AnimatorSpeed = Animator.StringToHash("speed");
        private static readonly int AnimatorVSpeed = Animator.StringToHash("vspeed");
        private static readonly int AnimatorGrounded = Animator.StringToHash("grounded");
        private static readonly int AnimatorAttack1 = Animator.StringToHash("attack1");
        private static readonly int AnimatorAttack2 = Animator.StringToHash("attack2");
        private static readonly int AnimatorHit = Animator.StringToHash("hit");
        private static readonly int AnimatorDie = Animator.StringToHash("die");

        private void Awake()
        {
            
        }
        void Start()
        {
            
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            

            _feet = transform.Find("Feet");
            _floor = LayerMask.GetMask("Floor");

            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
           
        }
               

        void Update()
        {
            _grounded = Physics2D.OverlapCircle(_feet.position, 0.1f, _floor);
            //_animator.SetFloat(AnimatorSpeed, this._direction);
            //_animator.SetFloat(AnimatorVSpeed, this._rigidbody2D.velocity.y);
            //_animator.SetBool(AnimatorGrounded, this._grounded);
            
        }
        private void FixedUpdate()
        {
            transform.position += new Vector3(((_grounded)?speed  : airSpeed)* _direction, 0, 0);
            if(!_grounded && _rigidbody2D.velocity.y < .3f && _rigidbody2D.velocity.y > 0)
            {
                Debug.Log("add force");
                _rigidbody2D.AddForce(-Vector2.up * downForce, ForceMode2D.Impulse);
            }
        }

        public void Move(IMoveableReceiver.Direction direction)
        {
            _direction = (direction == IMoveableReceiver.Direction.None)? 0 : (direction == IMoveableReceiver.Direction.Left)? -1 : 1;
            bool lookingRight = _direction == 1;
            transform.localScale = new Vector3(lookingRight ? 1 : -1, 1, 1);
        }
        ////////////Jump
        public void Jump(IJumperReceiver.JumpStage stage)
        {
            if (!_grounded)
            {
                if(stage == IJumperReceiver.JumpStage.Landing && _rigidbody2D.velocity.y > 0)
                {
                    _rigidbody2D.velocity = Vector3.zero;
                    _rigidbody2D.angularVelocity = 0;
                }
                return;
            }
            if(stage == IJumperReceiver.JumpStage.Jumping)
            {
                _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

        }
        ////////////////

        
        public void Attack1()
        {
            _animator.SetTrigger("lightAttack");
        }
        
        

        public void Attack2()
        {
            _animator.SetTrigger("heavyAttack");
        }
        public void TakeHit()
        {
            
            if (invulneravilityTime > 0) return;
            invulneravilityTime = 1;
            _animator.SetTrigger("hit");
        }
        public void Die(ulong player)
        {
        }
        
    }
}