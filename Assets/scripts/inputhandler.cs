using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class inputhandler : MonoBehaviour
{
    public Dictionary<string, bool> inputs = new Dictionary<string, bool>();
    [SerializeField] private PlayerInput playerInput;
    public bool sprint;

    public Vector2 move;
    public Vector2 mousepos;

    InputAction moveAction;
    InputAction shootAction;
    InputAction jumpAction;
    InputAction shooterAction;
    InputAction reloadAction;
    InputAction InteractAction;
    InputAction SprintAction;

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
        SprintAction = playerInput.actions["Sprint"];
        Debug.Log("Input Handler Awake");
    }

    // Update is called once per frame
    void Update()
    {
        move = moveAction.ReadValue<Vector2>();
        inputs["shoot"] = shootAction.IsPressed();
        inputs["jump"] = jumpAction.triggered;
        inputs["reload"] = reloadAction.triggered;
        inputs["interact"] = InteractAction.triggered;
        inputs["Sprint"] = SprintAction.IsPressed();
        mousepos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}
