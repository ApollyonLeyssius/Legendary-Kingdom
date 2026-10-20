using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            
        }
    }
}
