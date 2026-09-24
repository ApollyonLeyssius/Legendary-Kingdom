using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -20f;

    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float controllerSensitivity = 120f;
    [SerializeField] private float cameraDistance = 4.5f;
    [SerializeField] private float cameraTargetHeight = 1.5f;
    [SerializeField] private float cameraFollowSmoothTime = 0.06f;
    [SerializeField] private float startingPitch = 15f;
    [SerializeField] private float minPitch = -25f;
    [SerializeField] private float maxPitch = 65f;

    private CharacterController controller;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private float verticalVelocity;
    private float cameraYaw;
    private float cameraPitch;
    private float rotationVelocity;

    private Vector3 cameraFocus;
    private Vector3 cameraFollowVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        moveAction = new InputAction("Move", InputActionType.Value);

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.AddBinding("<Gamepad>/leftStick");

        lookAction = new InputAction("Look", InputActionType.Value);
        lookAction.AddBinding("<Mouse>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        if (playerCamera == null)
        {
            Debug.LogError(
                "Sleep de Main Camera naar het veld Player Camera.",
                this
            );

            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
    }

    private void Start()
    {
        cameraYaw = transform.eulerAngles.y;
        cameraPitch = Mathf.Clamp(startingPitch, minPitch, maxPitch);

        cameraFocus = transform.position
            + Vector3.up * cameraTargetHeight;

        UpdateCameraTransform();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        Look();
        Move();
    }

    private void Look()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        bool usingMouse = lookAction.activeControl?.device is Mouse;

        // Muisdelta is al een verplaatsing per inputupdate.
        // Stickinput wordt omgerekend naar graden per seconde.
        float sensitivity = usingMouse
            ? mouseSensitivity
            : controllerSensitivity * Time.deltaTime;

        cameraYaw += lookInput.x * sensitivity;
        cameraPitch -= lookInput.y * sensitivity;

        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
    }

    private void Move()
    {
        Vector2 input = Vector2.ClampMagnitude(
            moveAction.ReadValue<Vector2>(),
            1f
        );

        // Gebruik alleen de horizontale camerahoek voor beweging.
        Quaternion cameraHeading = Quaternion.Euler(
            0f, cameraYaw, 0f
        );

        Vector3 moveDirection = cameraHeading
            * new Vector3(input.x, 0f, input.y);

        // Draai het personage soepel naar de looprichting.
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(
                moveDirection.x,
                moveDirection.z
            ) * Mathf.Rad2Deg;

            float smoothedAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(
                0f, smoothedAngle, 0f
            );
        }
        else
        {
            rotationVelocity = 0f;
        }

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (jumpAction.WasPressedThisFrame())
            {
                verticalVelocity = Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        // Loopsnelheid beïnvloedt alleen horizontale beweging.
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;

        CollisionFlags collisions = controller.Move(
            velocity * Time.deltaTime
        );

        // Stop opstijgen bij een botsing tegen een plafond.
        if ((collisions & CollisionFlags.Above) != 0
            && verticalVelocity > 0f)
        {
            verticalVelocity = 0f;
        }
    }

    private void LateUpdate()
    {
        Vector3 targetFocus = transform.position
            + Vector3.up * cameraTargetHeight;

        cameraFocus = Vector3.SmoothDamp(
            cameraFocus,
            targetFocus,
            ref cameraFollowVelocity,
            cameraFollowSmoothTime
        );

        UpdateCameraTransform();
    }

    private void UpdateCameraTransform()
    {
        Quaternion cameraRotation = Quaternion.Euler(
            cameraPitch, cameraYaw, 0f
        );

        Vector3 cameraPosition = cameraFocus
            - cameraRotation * Vector3.forward * cameraDistance;

        playerCamera.SetPositionAndRotation(
            cameraPosition,
            cameraRotation
        );
    }

    private void OnDestroy()
    {
        moveAction?.Dispose();
        lookAction?.Dispose();
        jumpAction?.Dispose();
    }
}