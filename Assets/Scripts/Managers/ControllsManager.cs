using Movement.Commands;
using Movement.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllsManager : MonoBehaviour, IObserver
{
    [SerializeField] private FighterMovement _character;
    public FighterMovement Character
    {
        get => _character;
        set
        {
            _character = value;
            SetCharacter(_character);
        }
    }

    private enum Actions
    {
        Dodge,
        Stop,
        WalkLeft,
        WalkRight,
        Attack,
        Block,
        CancelBlock,
        AdvancedAttack,
        AdvancedParry,
        AdvancedDodge,
        Heal,
    }

    public InputActionAsset inputActions;
    public string movementMap;
    private InputAction dodge;
    private InputAction move;
    private InputAction attack;
    private InputAction block;
    private InputAction advancedAttack;
    private InputAction advancedParry;
    private InputAction advancedDodge;
    private InputAction heal;

    private bool _canDoAction = true;

    private Dictionary<Actions, ICommand> _commands;

    public void SetCharacter(FighterMovement character)
    {
        _commands = new Dictionary<Actions, ICommand> {
            { Actions.Dodge, new DodgeCommand(character) },
            { Actions.Stop, new StopCommand(character) },
            { Actions.WalkLeft, new WalkLeftCommand(character) },
            { Actions.WalkRight, new WalkRightCommand(character) },
            { Actions.Attack, new AttackCommand(character) },
            { Actions.Block, new BlockCommand(character) },
            { Actions.CancelBlock, new CancelBlockCommand(character) },
            { Actions.AdvancedAttack, new AdvancedAttackCommand(character) },
            { Actions.AdvancedParry, new AdvancedParryCommand(character) },
            { Actions.AdvancedDodge, new AdvancedDodgeCommand(character) },
            { Actions.Heal, new HealCommand(character) },
                //{ "pause", }
            };

        character.AddObserver(this);

        dodge = inputActions.FindAction($"{movementMap}/Dodge");
        dodge.performed += OnDodge;
        dodge.Enable();

        move = inputActions.FindAction($"{movementMap}/Move");
        move.performed += OnMove;
        move.canceled += OnMove;
        move.Enable();

        attack = inputActions.FindAction($"{movementMap}/Attack");
        attack.performed += OnAttack;
        attack.Enable();

        block = inputActions.FindAction($"{movementMap}/Block");
        block.performed += OnBlock;
        block.canceled += OnCancelBlock;
        block.Enable();

        advancedAttack = inputActions.FindAction($"{movementMap}/AdvancedAttack");
        advancedAttack.performed += OnAdvancedAttack;
        advancedAttack.Enable();

        advancedParry = inputActions.FindAction($"{movementMap}/AdvancedParry");
        advancedParry.performed += OnAdvancedParry;
        advancedParry.Enable();

        advancedDodge = inputActions.FindAction($"{movementMap}/AdvancedDodge");
        advancedDodge.performed += OnAdvancedDodge;
        advancedDodge.Enable();

        heal = inputActions.FindAction($"{movementMap}/Heal");
        heal.performed += OnHeal;
        heal.Enable();
    }

    private void OnHeal(InputAction.CallbackContext context)
    {
        if (!_canDoAction) return;
        _commands[Actions.Heal].Execute();
    }

    private void OnAdvancedDodge(InputAction.CallbackContext context)
    {
        if (!_canDoAction) return;
        _commands[Actions.AdvancedDodge].Execute();
    }

    private void OnAdvancedParry(InputAction.CallbackContext context)
    {
        if (!_canDoAction) return;
        _commands[Actions.AdvancedParry].Execute();
    }

    private void OnAdvancedAttack(InputAction.CallbackContext context)
    {
        if (!_canDoAction) return;
        _commands[Actions.AdvancedAttack].Execute();
    }

    public static ControllsManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SetCharacter(Character);
            return;
        }
        Destroy(gameObject);


    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!_canDoAction) return;
        float value = context.ReadValue<float>();

        if (value == 0f)
        {
            _commands[Actions.Stop].Execute();
        }
        else if (value == 1f)
        {
            _commands[Actions.WalkRight].Execute();
        }
        else
        {
            _commands[Actions.WalkLeft].Execute();
        }
    }

    public void UpdateState()
    {
        _canDoAction = Character.CanDoAction;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!_canDoAction) return;
        _commands[Actions.Dodge].Execute();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!_canDoAction) return;
        _commands[Actions.Attack].Execute();
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (!_canDoAction) return;
        _commands[Actions.Block].Execute();
    }

    public void OnCancelBlock(InputAction.CallbackContext context)
    {
        _commands[Actions.CancelBlock].Execute();
    }
}

