using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputAction movementInput;
    [SerializeField] private Transform GroundCheck;
    private int MovementSpeed = 5;
    private float JumpHeight = 1;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnEnable()
    {
        movementInput.Enable();
    }

    private void Walk()
    {
        Vector2 input = movementInput.ReadValue<Vector2>();

        Vector3 walking = new Vector3(input.x, 5f, input.y);
       
        transform.Translate(walking * MovementSpeed * Time.deltaTime, Space.World);

        Debug.Log("Input registered");
    }

    private void Update()
    {
        Walk();
    }


    // Update is called once per frame
    private void OnDisable()
    {
        movementInput.Disable();
    }
}
