using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public InputAction fireAction { get; private set; }
    public InputAction moveAction { get; private set; }

    private void Awake()
    {
        // Инициализация действий
        fireAction = new InputAction(name: "Fire");
        fireAction.AddBinding("<Keyboard>/space");
        
        moveAction = new InputAction(name: "Move", type: InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // Активируем действия
        fireAction.Enable();
        moveAction.Enable();
    }

    private void OnDestroy()
    {
        fireAction?.Dispose();
        moveAction?.Dispose();
    }
}