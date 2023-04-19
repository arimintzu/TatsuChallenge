using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput { get; private set; }
    //public InputStats inputStats;
    private void Awake()
    {

        GameManager.OnGameOver += DisableInput;
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= DisableInput;
    }

    void DisableInput()
    {
        inputDisabled = true;
        //Reset Input
        FrameInput = new FrameInput();
    }

    public bool inputDisabled;
    private void Update()
    {
        if (inputDisabled) return;
        FrameInput = Gather();
    }
#if ENABLE_INPUT_SYSTEM
    private PlayerInputActions _actions;
    private InputAction _move, _jump, _dash, _attack;

    private void Awake() {
        _actions = new PlayerInputActions();
        _move = _actions.Player.Move;
        _jump = _actions.Player.Jump;
        _dash = _actions.Player.Dash;
        _attack = _actions.Player.Attack;
    }

    private void OnEnable() => _actions.Enable();

    private void OnDisable() => _actions.Disable();

    private FrameInput Gather() {
        return new FrameInput {
            JumpDown = _jump.WasPressedThisFrame(),
            JumpHeld = _jump.IsPressed(),
            DashDown = _dash.WasPressedThisFrame(),
            AttackDown = _attack.WasPressedThisFrame(),
            Move = _move.ReadValue<Vector2>()
        };
    }

#elif ENABLE_LEGACY_INPUT_MANAGER
    private FrameInput Gather()
    {
        return new FrameInput
        {
            //JumpDown = Input.GetKeyDown(inputStats.keyJump),
            //JumpHeld = Input.GetKey(inputStats.keyJump),
            //DashDown = Input.GetKeyDown(inputStats.keyDash),
            //AttackDown = Input.GetKeyDown(inputStats.keyAttack),
            //SwitchWeaponDown = Input.GetKeyDown(inputStats.keySwitchWeapon),
            //InteractDown = Input.GetKeyDown(inputStats.keyInteract),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
        };
    }
#endif
}

public struct FrameInput
{
    public Vector2 Move;
    //public bool JumpDown;
    //public bool JumpHeld;
    //public bool DashDown;
    //public bool AttackDown;
    //public bool SwitchWeaponDown;
    //public bool InteractDown;
}
