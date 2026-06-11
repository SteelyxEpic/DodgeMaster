using UnityEngine;
using UnityEngine.InputSystem;

public class inputhandler : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    public Vector2 move;
    public bool shoot;
    public bool jump;
    public bool reload;
    public bool interact;

    InputAction moveAction;
    InputAction shootAction;
    InputAction jumpAction;
    InputAction shooterAction;
    InputAction reloadAction;
    InputAction InteractAction;

    public static inputhandler ins;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (ins == null)
        {
            ins = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        moveAction = playerInput.actions["Move"];
        shootAction = playerInput.actions["Fire"];
        jumpAction = playerInput.actions["Jump"];
        reloadAction = playerInput.actions["reload"];
        InteractAction = playerInput.actions["Interact"];
        Debug.Log("Input Handler Awake");
    }

    // Update is called once per frame
    void Update()
    {
        move = moveAction.ReadValue<Vector2>();
        shoot = shootAction.IsPressed();
        jump = jumpAction.triggered;
        reload = reloadAction.triggered;
        interact = InteractAction.triggered;
    }
}
