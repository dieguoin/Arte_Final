using Movement.Commands;
using Movement.Components;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class ControllsManager : MonoBehaviour
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

    public InputActionAsset inputActions;
    public string movementMap;
    private InputAction move;
    private InputAction jump;
    private InputAction lightAttack;
    private InputAction heavyAttack;


    private Dictionary<string, ICommand> _commands;

    public void SetCharacter(FighterMovement character)
    {
        _commands = new Dictionary<string, ICommand> {
                { "stop", new StopCommand(character) },
                { "walk-left", new WalkLeftCommand(character) },
                { "walk-right", new WalkRightCommand(character) },
                { "jump", new JumpCommand(character) },
                { "land", new LandCommand(character) },
                { "attack1", new Attack1Command(character) },
                { "attack2", new Attack2Command(character) }
                //{ "pause", }
            };

        move = inputActions.FindAction(movementMap + "/Mover");
        move.performed += OnMove;
        move.canceled += OnMove;
        move.Enable();


        jump= inputActions.FindAction(movementMap + "/Jump");
        jump.performed += OnJump;
        jump.canceled += OnJump;
        jump.Enable();

        lightAttack = inputActions.FindAction(movementMap + "/LightAttack");
        lightAttack.performed += context =>
        {
            _commands["attack1"].Execute();
        };
        lightAttack.Enable();

        heavyAttack = inputActions.FindAction(movementMap + "/HeavyAttack");
        heavyAttack.performed += context =>
        {
            _commands["attack2"].Execute();
        }; 
        heavyAttack.Enable();
    }

    public static ControllsManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            SetCharacter(Character);
            return;
        }
        Destroy(gameObject);
        

    }

    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        Debug.Log($"OnMove called {context.action}");

        if (value == 0f)
        {
            _commands["stop"].Execute();
        }
        else if (value == 1f)
        {
            _commands["walk-right"].Execute();
        }
        else
        {
            _commands["walk-left"].Execute();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        Debug.Log($"OnJump called {context.ReadValue<float>()}");

        if (value == 0f)
        {
            _commands["land"].Execute();
        }
        else
        {
            _commands["jump"].Execute();
        }
    }
    private void OnAttack1(InputAction.CallbackContext context)
    {
        _commands["attack1"].Execute();
    }
    private void OnAttack2(InputAction.CallbackContext context)
    {
        _commands["attack2"].Execute();

    }
}

