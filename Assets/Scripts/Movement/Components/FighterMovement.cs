using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Movement.Components
{
    [Serializable]
    public class Action
    {
        [SerializeField] private float _movPerFrame;
        public float MovPerFrame { get => _movPerFrame; }
        [SerializeField] private uint _staminaCost;
        public uint StaminaCost { get => _staminaCost; }
        [SerializeField] private uint _damage;
        public uint Damage { get => _damage; }
        [SerializeField] private float _durantion;
        public float Duration { get => _durantion; }
        [SerializeField] private float _actionRelativeTime;
        public float ActionRelativeTime { get => _actionRelativeTime * _durantion; }
    }

    public class Timer
    {
        private readonly float _maxSeconds;
        private float _seconds;
        private bool _set;

        public Timer(float maxSeconds)
        {
            _maxSeconds = maxSeconds;
            _seconds = _maxSeconds;
            _set = false;
        }

        public void Reset()
        {
            _set = false;
        }

        public bool Update()
        {
            if (_set) return false;
            _seconds -= Time.deltaTime;
            if (_seconds <= 0)
            {
                _seconds = _maxSeconds;
                _set = true;
                return true;
            }
            return false;
        }
    }

    //[RequireComponent(typeof(Rigidbody2D)),
    //RequireComponent(typeof(Animator))]
    public sealed class FighterMovement : MonoBehaviour, ISubject, IMoveableReceiver, IFighterReceiver, ITakeDamage
    {
        [SerializeField] private LayerMask enemy;

        private bool _canDoAction = true;
        public bool CanDoAction { get => _canDoAction && !_tired; }
        LinkedList<IObserver> Observers { get; set; }

        private bool _lookingRight = true;

        [SerializeField] private float _attack1Range;
        [SerializeField] private float _attack2Range;
        [SerializeField] private float _attack3Range;
        [SerializeField] private float _dodgeAttackRange;
        [SerializeField] private float _parryAttackRange;
        [SerializeField] private float _adavancedAttackRange;
        [SerializeField] private float _advancedParryRange;
        [SerializeField] private float _advancedDodgeRange;

        [SerializeField] private Transform _enemy;

        [SerializeField] private float _staminaRecovery;

        [SerializeField] private Action _move;
        [SerializeField] private Action _dodge;
        private Timer _dodgeDurationTimer;
        private Timer _dodgeInvecibleTimer;
        private Timer _dodgeTimer;
        [SerializeField] private Action _dodgeAttack;
        private Timer _dodgeAttackDurationTimer;
        private Timer _dodgeAttackTimer;
        [SerializeField] private Action _attack1;
        private Timer _attack1DurationTimer;
        private Timer _attack1Timer;
        [SerializeField] private Action _attack2;
        private Timer _attack2DurationTimer;
        private Timer _attack2Timer;
        [SerializeField] private Action _attack3;
        private Timer _attack3DurationTimer;
        private Timer _attack3Timer;
        [SerializeField] private Action _block;
        private Timer _blockDurationTimer;
        private Timer _blockPositionTimer;
        private Timer _blockTimer;
        [SerializeField] private Action _parry;
        private Timer _parryDurationTimer;
        private Timer _parryPositionTimer;
        private Timer _parryTimer;
        [SerializeField] private Action _advancedAttack;
        private Timer _advancedAttackDurationTimer;
        private Timer _advancedAttackTimer;
        [SerializeField] private Action _advancedParry;
        private Timer _advancedParryDurationTimer;
        private Timer _advancedParryTimer;
        [SerializeField] private Action _advancedParryAttack;
        private Timer _advancedParryAttackDurationTimer;
        private Timer _advancedParryAttackTimer;
        [SerializeField] private Action _advancedDodge;
        private Timer _advancedDodgeDurationTimer;
        private Timer _advancedDodgeTimer;
        [SerializeField] private Action _heal;
        private Timer _healDurationTimer;
        private Timer _healTimer;
        [SerializeField] private Action _takeDamage;
        private Timer _takeDamageDurationTimer;
        private Timer _takeDamageTimer;
        [SerializeField] private Action _die;
        private Timer _dieDurationTimer;

        private float speed;
        public float airSpeed = 0.2f;
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

        [SerializeField] private float invulneravilityTime;
        [SerializeField] private float _activeParryTime;

        [SerializeField] private float _timeToBlockPosition;

        [SerializeField] private uint _maxStamina;
        public uint MaxStamina { get => _maxStamina; }

        private uint _actualStamina;
        public uint ActualStamina { get => _actualStamina; }

        [SerializeField] private uint _maxAdvancedAttackCharges;
        public uint MaxAdvancedAttackCharges { get => _maxAdvancedAttackCharges; }
        public float AdvancedAttackProgress { get; private set; }
        public uint AdvancedAttackCharges { get; private set; }

        [SerializeField] private uint _maxAdvancedParryCharges;
        public uint MaxAdvancedParryCharges { get => _maxAdvancedParryCharges; }
        public float AdvancedParryProgress { get; private set; }
        public uint AdvancedParryCharges { get; private set; }

        [SerializeField] private uint _maxAdvancedDodgeCharges;
        public uint MaxAdvancedDodgeCharges { get => _maxAdvancedDodgeCharges; }
        public float AdvancedDodgeProgress { get; private set; }
        public uint AdvancedDodgeCharges { get; private set; }

        /// <summary>
        /// Define los maximos puntos de vida del personaje.
        /// </summary>
        [SerializeField] private uint _maxHealth;
        /// <summary>
        /// Permite leer los maximos puntos del vida del personaje.
        /// </summary>
        public uint MaxHealth { get => _maxHealth; }

        /// <summary>
        /// Define cuantos puntos de vida recuperan las cargas de curacion.
        /// </summary>
        [SerializeField] private uint _healingChargesRecovery;

        /// <summary>
        /// Define cuantas cargas de curacion como maximo puede llevar este personaje.
        /// </summary>
        [SerializeField] private uint _maxHealingCharges;
        /// <summary>
        /// Permite leer el maximo numero de cargas de curacion del personaje.
        /// </summary>
        public uint MaxHealingCharges { get => _maxHealingCharges; }

        /// <summary>
        /// Define la salud actual del personaje.
        /// </summary>
        private uint _actualHealth;
        /// <summary>
        /// Permite leer la salud actual del personaje.
        /// </summary>
        public uint ActualHealth { get => _actualHealth; }

        /// <summary>
        /// Define cuantas cargas de curacion le quedan a este personaje.
        /// </summary>
        private uint _actualHealingCharges;
        /// <summary>
        /// Permite leer cuantas cargas de curacion le quedan a este personaje.
        /// </summary>
        public uint ActualHealingCharges { get => _actualHealingCharges; }

        private bool _tired = false;
        private bool _dead = false;
        private bool _tookDamage = false;

        [Range(-2, 2)]
        private int comboState = -1;

        private bool _invencible = false;

        private bool _canParry;

        private bool _moving;
        private bool _dodging;
        private bool _attacking;
        private bool _blocking;
        private bool _parrying;
        private bool _advancedAttacking;
        private bool _advancedParrying;
        private bool _advancedDodging;
        private bool _healing;
        private bool _attackBlocked;
        private bool _advancedParryActivated;

        private bool _parryPosition;
        private bool _blockPostion;
        private bool _advancedParryPosition;

        private void Awake()
        {
            speed = _move.MovPerFrame;
            Observers = new();
            _actualHealingCharges = _maxHealingCharges;
            _actualHealth = _maxHealth;
            _actualStamina = _maxStamina;

            _takeDamageDurationTimer = new(_takeDamage.Duration);
            _takeDamageTimer = new(_takeDamage.ActionRelativeTime);
            _dieDurationTimer = new(_die.Duration);
            _advancedParryAttackTimer = new(_advancedParryAttack.ActionRelativeTime);
            _advancedParryAttackDurationTimer = new(_advancedParryAttack.Duration);
            _advancedAttackDurationTimer = new(_advancedAttack.Duration);
            _advancedDodgeDurationTimer = new(_advancedDodge.Duration);
            _advancedParryDurationTimer = new(_advancedParry.Duration);
            _attack1DurationTimer = new(_attack1.Duration);
            _attack2DurationTimer = new(_attack2.Duration);
            _attack3DurationTimer = new(_attack3.Duration);
            _blockDurationTimer = new(_block.Duration);
            _dodgeAttackDurationTimer = new(_dodge.Duration);
            _dodgeDurationTimer = new(_dodge.Duration);
            _healDurationTimer = new(_heal.Duration);
            _parryDurationTimer = new(_parry.Duration);
            _advancedAttackTimer = new(_advancedAttack.ActionRelativeTime);
            _advancedDodgeTimer = new(_advancedDodge.ActionRelativeTime);
            _advancedParryTimer = new(_advancedParry.ActionRelativeTime);
            _attack1Timer = new(_attack1.ActionRelativeTime);
            _attack2Timer = new(_attack2.ActionRelativeTime);
            _attack3Timer = new(_attack3.ActionRelativeTime);
            _blockPositionTimer = new(_timeToBlockPosition);
            _blockTimer = new(_block.ActionRelativeTime);
            _dodgeInvecibleTimer = new(invulneravilityTime);
            _dodgeTimer = new(_dodge.ActionRelativeTime);
            _dodgeAttackTimer = new(_dodgeAttack.ActionRelativeTime);
            _healTimer = new(_heal.ActionRelativeTime);
            _parryTimer = new(_parry.ActionRelativeTime);
            _parryPositionTimer = new(_activeParryTime);
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
            if (_dead)
            {
                Dying();
                return;
            }
            if (_tired)
            {
                return;
            }
            _grounded = Physics2D.OverlapCircle(_feet.position, 0.1f, _floor);
            _animator.SetInteger("speed", _direction);
            _animator.SetBool("ToEnemy", (_lookingRight && _direction == 1) || (!_lookingRight && _direction == -1));

            _lookingRight = (_enemy.position - transform.position).x == 0 ? _lookingRight : (_enemy.position - transform.position).x > 0;
            transform.localScale = new Vector3(_lookingRight ? 1 : -1, 1, 1);

            if (_dodging)
            {
                DodgeUpdate();
            }
            if (_attacking)
            {
                Debug.Log("atacando al enemigo");
                AttackUpdate();
            }
            if (_blocking)
            {
                BlockUpdate();
            }
            if (_parrying)
            {
                ParryUpdate();
            }
            if (_advancedAttacking)
            {
                AdvancedAttackUpdate();
            }
            if (_advancedParrying)
            {
                AdvancedParryUpdate();
            }
            if (_advancedParryActivated)
            {
                AdvancedParryAttackUpdate();
            }
            if (_advancedDodging)
            {
                AdvancedDodgeUpdate();
            }
            if (_healing)
            {
                HealingUpdate();
            }
            if (_attackBlocked)
            {
                AttackBlockedUpdate();
            }
            if (_tookDamage)
            {
                TakeDamageUpdate();
            }

            //_animator.SetBool(AnimatorGrounded, this._grounded);
            ActivateAnimatorParams();
            Notify();

            Debug.Log(_canDoAction);
        }
        private void FixedUpdate()
        {
            transform.position += new Vector3(speed * _direction, 0, 0);
            if (!_grounded && _rigidbody2D.velocity.y < .3f && _rigidbody2D.velocity.y > 0)
            {
                Debug.Log("add force");
                _rigidbody2D.AddForce(-Vector2.up * downForce, ForceMode2D.Impulse);
            }

            if (_canDoAction)
            {
                RecoverStamina();
            }
        }

        public void Move(IMoveableReceiver.Direction direction)
        {
            _moving = true;
            speed = _move.MovPerFrame;
            if (direction == IMoveableReceiver.Direction.None) {
                _direction = 0;
                _moving = false;
            }
            else if (direction == IMoveableReceiver.Direction.Left)
            {
                _direction = -1;
            }
            else
            {
                _direction = 1;
            }
            //bool lookingRight = _direction == 1;
            //bool lookingLeft = _direction == -1;
            //transform.localScale = new Vector3(_direction == 0 ? transform.localScale.x : _direction, 1, 1);
        }

        public void AddObserver(IObserver observer)
        {
            Observers.AddLast(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            Observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var o in Observers)
            {
                o.UpdateState();
            }
        }

        private void RecoverHealth(uint health)
        {
            if (ActualHealth + health > MaxHealth)
            {
                _actualHealth = MaxHealth;
                return;
            }
            _actualHealth += health;

        }

        private void RecoverStamina()
        {
            if (_actualStamina + _staminaRecovery / Time.deltaTime >= _maxStamina)
            {
                _actualStamina = _maxStamina;
            }
            _actualStamina += (uint)(_staminaRecovery / Time.deltaTime);
            if (_actualStamina >= _maxStamina * 0.8f && _tired)
            {
                _tired = false;
            }
        }

        private void RestStamina(uint staminaCost)
        {
            if (_actualStamina - staminaCost <= 0)
            {
                _actualStamina = 0;
                _tired = true;
                return;
            }
            _actualStamina -= staminaCost;
            _tired = false;
        }

        private void RestHealth(uint damage)
        {
            if (_actualHealth - damage <= 0)
            {
                _actualHealth = 0;
                _dead = true;
                return;
            }
            _actualHealth -= damage;
            _dead = false;
        }

        public void Dodge()
        {
            ResetTimers();
            ResetActionStates();
            _dodging = true;
            //ResetAnimatorParams();
            comboState = -2;
            RestStamina(_dodge.StaminaCost);
            _invencible = true;
            _canDoAction = false;
            Notify();
        }

        private void DodgeUpdate()
        {
            speed = _dodge.MovPerFrame;
            _direction = _lookingRight ? -1 : 1;
            if (_dodgeInvecibleTimer.Update())
            {
                _invencible = false;
            }
            if (_dodgeTimer.Update())
            {
                _canDoAction = true;
            }
            if (_dodgeDurationTimer.Update())
            {
                _dodging = false;
                comboState = -1;
                speed = 0;
                ResetTimers();
            }
        }

        public void Attack()
        {
            Debug.Log("Orden de atacar recibida");
            ResetTimers();
            ResetActionStates();
            _attacking = true;
            //ResetAnimatorParams();
            comboState++;
            switch (comboState)
            {
                case -1:
                    RestStamina(_dodgeAttack.StaminaCost);
                    break;

                case 0:
                    RestStamina(_attack1.StaminaCost);
                    break;

                case 1:
                    RestStamina(_attack2.StaminaCost);
                    break;

                case 2:
                    RestStamina(_attack3.StaminaCost);
                    break;

                default:
                    return;
            }
            _canDoAction = false;
            Notify();
        }

        private void AttackUpdate()
        {
            Timer attackTimer;
            Timer durationTimer;
            float range;
            Action attack;
            float progressAddition;
            switch (comboState)
            {
                case -1:
                    attackTimer = _dodgeAttackTimer;
                    durationTimer = _dodgeAttackDurationTimer;
                    range = _dodgeAttackRange;
                    attack = _dodgeAttack;
                    progressAddition = UnityEngine.Random.Range(47, 61);
                    break;

                case 0:
                    attackTimer = _attack1Timer;
                    durationTimer = _attack1DurationTimer;
                    range = _attack1Range;
                    attack = _attack1;
                    progressAddition = UnityEngine.Random.Range(35, 42);
                    break;

                case 1:
                    attackTimer = _attack2Timer;
                    durationTimer = _attack2DurationTimer;
                    range = _attack2Range;
                    attack = _attack2;
                    progressAddition = UnityEngine.Random.Range(38, 57);
                    break;

                case 2:
                    attackTimer = _attack3Timer;
                    durationTimer = _attack3DurationTimer;
                    range = _attack3Range;
                    attack = _attack3;
                    progressAddition = UnityEngine.Random.Range(51, 77);
                    break;

                default:
                    attackTimer = null;
                    durationTimer = null;
                    range = 0f;
                    attack = null;
                    progressAddition = 0;
                    break;
            }

            speed = attack.MovPerFrame;
            _direction = _lookingRight ? 1 : -1;

            if (attackTimer.Update())
            {
                var collider = Physics2D.Raycast(Vector2.zero, _lookingRight ? Vector2.right : Vector2.left, range, enemy).collider;
                if (collider != null)
                {
                    collider.gameObject.GetComponent<ITakeDamage>().TakeDamage(attack.Damage);
                    AdvancedAttackAddProgress(progressAddition);
                }
                _canDoAction = true;
            }

            if (durationTimer.Update())
            {
                comboState = -1;
                ResetTimers();
                speed = 0;
                _attacking = false;
            }
        }

        public void Block()
        {
            ResetTimers();
            ResetActionStates();
            _blocking = true;
            //ResetAnimatorParams();
            comboState = -1;
            _canDoAction = false;
            Notify();
            _canParry = true;
        }

        private void BlockUpdate()
        {
            if (_canParry)
            {
                _parryPosition = true;
                if (_parryPositionTimer.Update())
                {
                    _parryPositionTimer.Reset();
                    _parryPosition = false;
                    _canParry = false;
                }
                return;
            }
            _blockPostion = true;
        }

        private void AttackBlockedUpdate()
        {
            if (_blockDurationTimer.Update())
            {
                _canDoAction = true;
                ResetTimers();
            }
        }

        public void CancelBlock()
        {
            ResetTimers();
            comboState = 0;
            _blocking = false;
            _canParry = true;
            _blockPostion = false;
            _parryPosition = false;
        }

        private void ParryUpdate()
        {
            speed = 0;
            _direction = _lookingRight ? 1 : -1;
            if (_parryTimer.Update())
            {
                var collider = Physics2D.Raycast(Vector2.zero, _lookingRight ? Vector2.right : Vector2.left, _parryAttackRange, enemy).collider;
                if (collider != null)
                {
                    collider.gameObject.GetComponent<ITakeDamage>().TakeDamage(_parry.Damage);                    
                }
                _canDoAction = true;
                comboState = 0;
            }
            if (_parryDurationTimer.Update())
            {
                comboState = -1;
                ResetTimers();
                _parrying = false;
            }
        }

        public void AdvancedAttack()
        {
            if (AdvancedAttackCharges == 0) return;
            ResetTimers();
            ResetActionStates();
            _advancedAttacking = true;
            RestStamina(_advancedAttack.StaminaCost);
            AdvancedAttackCharges--;
            //ResetAnimatorParams();
            comboState = 1;
            _canDoAction = false;
        }

        private void AdvancedAttackUpdate()
        {
            speed = _advancedAttack.MovPerFrame;
            _direction = _lookingRight ? 1 : -1;

            if (_advancedAttackTimer.Update())
            {
                var collider = Physics2D.Raycast(Vector2.zero, _lookingRight ? Vector2.right : Vector2.left, _parryAttackRange, enemy).collider;
                if (collider != null) collider.gameObject.GetComponent<ITakeDamage>().TakeDamage(_parry.Damage);
                _canDoAction = true;
                comboState = 0;
            }
            if (_advancedDodgeDurationTimer.Update())
            {
                comboState = -1;
                ResetTimers();
                speed = 0;
                _advancedAttacking = false;
            }
        }

        public void AdvancedParry()
        {
            if (AdvancedParryCharges == 0) return;
            ResetActionStates();
            ResetTimers();
            _advancedParrying = true;
            RestStamina(_advancedParry.StaminaCost);
            AdvancedParryCharges--;
            //ResetAnimatorParams();
            comboState = 1;
            speed = 0;
            _canDoAction = false;
        }

        private void AdvancedParryUpdate()
        {
            speed = _advancedParry.MovPerFrame;
            _direction = _lookingRight ? 1 : -1;

            if (_advancedParryDurationTimer.Update())
            {
                _advancedParryPosition = false;
                _advancedParrying = false;
                _canDoAction = true;
                speed = 0;
                ResetTimers();
            }
        }

        private void AdvancedParryAttackUpdate()
        {
            speed = _advancedParryAttack.MovPerFrame;
            _direction = _lookingRight ? 1 : -1;

            if (_advancedParryAttackTimer.Update())
            {
                var collider = Physics2D.Raycast(Vector2.zero, _lookingRight ? Vector2.right : Vector2.left, _parryAttackRange, enemy).collider;
                if (collider != null) collider.gameObject.GetComponent<ITakeDamage>().TakeDamage(_parry.Damage);
                _canDoAction = true;
                comboState = 0;
            }
            if (_advancedParryAttackDurationTimer.Update())
            {
                comboState = -1;
                ResetTimers();
                speed = 0;
                _advancedParryActivated = false;
            }
        }

        public void AdvancedDodge()
        {
            if (AdvancedDodgeCharges == 0) return;
            ResetTimers();
            ResetActionStates();
            _advancedDodging = true;
            RestStamina(_advancedDodge.StaminaCost);
            AdvancedDodgeCharges--;
            //ResetAnimatorParams();
            comboState = 1;
            _invencible = true;
            _canDoAction = false;
        }

        private void AdvancedDodgeUpdate()
        {
            speed = _advancedDodge.MovPerFrame;
            _direction = _lookingRight ? 1 : -1;

            if (_advancedDodgeTimer.Update())
            {
                var collider = Physics2D.Raycast(Vector2.zero, _lookingRight ? Vector2.right : Vector2.left, _parryAttackRange, enemy).collider;
                if (collider != null) collider.gameObject.GetComponent<ITakeDamage>().TakeDamage(_parry.Damage);
                _canDoAction = true;
                comboState = 0;
            }
            if (_advancedDodgeDurationTimer.Update())
            {
                comboState = -1;
                ResetTimers();
                speed = 0;
                _advancedDodging = false;
            }
        }

        public void Heal()
        {
            if (_actualHealingCharges == 0) return;
            ResetTimers();
            ResetActionStates();
            _healing = true;
            _actualHealingCharges--;
            //ResetAnimatorParams();
            comboState = -1;
            _canDoAction = false;
        }

        private void HealingUpdate()
        {
            speed = _heal.MovPerFrame;
            _direction = _lookingRight ? 1 : -1;

            if (_healTimer.Update())
            {
                RecoverHealth(_healingChargesRecovery);
                _canDoAction = true;
                comboState = -1;
            }
            if (_healDurationTimer.Update())
            {
                comboState = -1;
                _healing = false;
                speed = 0;
                ResetTimers();
            }
        }

        private void Dying()
        {
            speed = _die.MovPerFrame;
            _direction = _lookingRight ? 1 : -1;

            if (_dieDurationTimer.Update())
            {
                speed = 0;
                GameManager.instance.ReloadGame();
            }
        }

        private void AdvancedAttackAddProgress(float progress)
        {
            if (AdvancedAttackProgress == 100 && AdvancedAttackCharges == MaxAdvancedAttackCharges) return;
            AdvancedAttackProgress += progress;
            if (AdvancedAttackProgress >= 100)
            {
                if (AdvancedAttackCharges == MaxAdvancedAttackCharges)
                {
                    AdvancedAttackProgress = 100;
                    return;
                }
                AdvancedAttackCharges++;
                AdvancedAttackProgress -= 100;
            }
        }

        private void AdvancedParryAddProgress(float progress)
        {
            if (AdvancedParryProgress == 100 && AdvancedParryCharges == MaxAdvancedParryCharges) return;
            AdvancedParryProgress += progress;
            if (AdvancedParryProgress >= 100)
            {
                if (AdvancedParryCharges == MaxAdvancedParryCharges)
                {
                    AdvancedParryProgress = 100;
                    return;
                }
                AdvancedParryCharges++;
                AdvancedParryProgress -= 100;
            }
        }

        private void AdvancedDodgeAddProgress(float progress)
        {
            if (AdvancedDodgeProgress == 100 && AdvancedDodgeCharges == MaxAdvancedDodgeCharges) return;
            AdvancedDodgeProgress += progress;
            if (AdvancedDodgeProgress >= 100)
            {
                if (AdvancedDodgeCharges == MaxAdvancedDodgeCharges)
                {
                    AdvancedDodgeProgress = 100;
                    return;
                }
                AdvancedDodgeCharges++;
                AdvancedDodgeProgress -= 100;
            }
        }

        private void ResetTimers()
        {
            _advancedAttackDurationTimer.Reset();
            _advancedAttackTimer.Reset();
            _advancedDodgeDurationTimer.Reset();
            _advancedDodgeTimer.Reset();
            _advancedParryAttackDurationTimer.Reset();
            _advancedParryAttackTimer.Reset();
            _advancedParryDurationTimer.Reset();
            _advancedParryTimer.Reset();
            _attack1DurationTimer.Reset();
            _attack1Timer.Reset();
            _attack2DurationTimer.Reset();
            _attack2Timer.Reset();
            _attack3DurationTimer.Reset();
            _attack3Timer.Reset();
            _blockDurationTimer.Reset();
            _blockPositionTimer.Reset();
            _blockTimer.Reset();
            _dodgeAttackDurationTimer.Reset();
            _dodgeAttackTimer.Reset();
            _dodgeDurationTimer.Reset();
            _dodgeInvecibleTimer.Reset();
            _dodgeTimer.Reset();
            _healDurationTimer.Reset();
            _healTimer.Reset();
            _parryDurationTimer.Reset();
            _parryPositionTimer.Reset();
            _parryTimer.Reset();
            _takeDamageDurationTimer.Reset();
            _takeDamageTimer.Reset();
            _dieDurationTimer.Reset();
        }

        private void ResetActionStates()
        {
            _dodging = false;
            _attacking = false;
            _blocking = false;
            _parrying = false;
            _advancedAttacking = false;
            _advancedParrying = false;
            _advancedDodging = false;
            _healing = false;
            _attackBlocked = false;
            _advancedParryActivated = false;
        }

        //private void ResetAnimatorParams()
        //{
        //    for (int i = 0; i < _animator.parameterCount; i++)
        //    {
        //        if (_animator.parameters[i].name.ToLower() == "ToEnemy".ToLower()) continue;
        //        _animator.SetBool(_animator.parameters[i].name, false);
        //    }
        //}

        private void ActivateAnimatorParams()
        {
            _animator.SetBool("Moving", _moving);
            _animator.SetBool("Blocking", _blocking);
            _animator.SetBool("Parrying", _parrying);
            _animator.SetBool("Dodging", _dodging);
            _animator.SetBool("Attacking", _attacking);
            _animator.SetBool("AdvancedAttacking", _advancedAttacking);
            _animator.SetBool("AdvancedParrying", _advancedParrying);
            _animator.SetBool("AdvancedDodging", _advancedDodging);
            _animator.SetBool("Healing", _healing);
            _animator.SetBool("Dead", _dead);
        }

        public void TakeDamage(uint damage)
        {
            if (_invencible)
            {
                AdvancedDodgeAddProgress(UnityEngine.Random.Range(40, 63));
                return;
            }
            if (_parryPosition)
            {
                AdvancedParryAddProgress(UnityEngine.Random.Range(60, 83));
                _parrying = true;
                return;
            }
            if (_blockPostion)
            {
                AdvancedParryAddProgress(UnityEngine.Random.Range(37, 41));
                _attackBlocked = true;
                return;
            }
            if (_advancedParryPosition)
            {
                _advancedParryActivated = true;
                return;
            }

            RestHealth(damage);
            _tookDamage = true;
        }

        private void TakeDamageUpdate()
        {
            speed = _takeDamage.MovPerFrame;
            _direction = _lookingRight ? 1 : -1;

            if (_takeDamageTimer.Update())
            {
                _canDoAction = true;
                comboState = -1;
            }
            if (_takeDamageDurationTimer.Update())
            {
                _tookDamage = false;
                comboState = -1;
                _takeDamageTimer.Reset();
                _takeDamageDurationTimer.Reset();
            }
        }

        public void EnviromentHeal(uint healPoints)
        {
            RecoverHealth((uint)(healPoints * Time.deltaTime));
        }
    }
}