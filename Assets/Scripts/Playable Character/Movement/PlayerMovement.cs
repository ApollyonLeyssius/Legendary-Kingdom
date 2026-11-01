
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float jumpHeight = 1f;
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

    [Header("Animation")]
    [SerializeField] private bool useSprintAnimation = true;

    [Header("Debugging")]
    [SerializeField] private bool showAnimatorDebug = true;

    private static readonly int WalkingTrigger =
        Animator.StringToHash("IsWalking");

    private static readonly int IdleTrigger =
        Animator.StringToHash("IsIdle");

    private static readonly int JumpTrigger =
        Animator.StringToHash("IsJumping");

    private static readonly int SprintTrigger =
        Animator.StringToHash("IsSprinting");

    private enum MovementState
    {
        Idle,
        Walking,
        Sprinting
    }

    private MovementState currentMovementState = MovementState.Idle;
    private bool movementStateInitialized;

    private CharacterController controller;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private float verticalVelocity;
    private float cameraYaw;
    private float cameraPitch;
    private float rotationVelocity;

    private Vector3 cameraFocus;
    private Vector3 cameraFollowVelocity;

    private bool isSprinting;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        moveAction = new InputAction(
            "Move",
            InputActionType.Value
        );

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.AddBinding("<Gamepad>/leftStick");

        lookAction = new InputAction(
            "Look",
            InputActionType.Value
        );

        lookAction.AddBinding("<Mouse>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");

        jumpAction = new InputAction(
            "Jump",
            InputActionType.Button
        );

        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        sprintAction = new InputAction(
            "Sprint",
            InputActionType.Button
        );

        sprintAction.AddBinding("<Keyboard>/leftShift");
        sprintAction.AddBinding("<Gamepad>/leftStickPress");

        if (playerCamera == null)
        {
            Debug.LogError(
                "Assign the Main Camera to Player Camera.",
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
        moveAction?.Enable();
        lookAction?.Enable();
        jumpAction?.Enable();
        sprintAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        lookAction?.Disable();
        jumpAction?.Disable();
        sprintAction?.Disable();
    }

    private void Start()
    {
        cameraYaw = transform.eulerAngles.y;

        cameraPitch = Mathf.Clamp(
            startingPitch,
            minPitch,
            maxPitch
        );

        cameraFocus = transform.position
            + Vector3.up * cameraTargetHeight;

        UpdateCameraTransform();

        DebugAnimator();
    }

    private void Update()
    {
        Look();
        Move();
    }

    private void Look()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        bool usingMouse =
            lookAction.activeControl?.device is Mouse;

        float sensitivity = usingMouse
            ? mouseSensitivity
            : controllerSensitivity * Time.deltaTime;

        cameraYaw += lookInput.x * sensitivity;
        cameraPitch -= lookInput.y * sensitivity;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            minPitch,
            maxPitch
        );
    }

    private void Move()
    {
        Vector2 input = Vector2.ClampMagnitude(
            moveAction.ReadValue<Vector2>(),
            1f
        );

        bool isMoving = input.sqrMagnitude > 0.001f;

        isSprinting =
            isMoving && sprintAction.IsPressed();

        UpdateMovementAnimation(isMoving);

        Quaternion cameraHeading = Quaternion.Euler(
            0f,
            cameraYaw,
            0f
        );

        Vector3 moveDirection = cameraHeading
            * new Vector3(input.x, 0f, input.y);

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
                0f,
                smoothedAngle,
                0f
            );
        }
        else
        {
            rotationVelocity = 0f;
        }

        HandleJump();

        float currentSpeed = isSprinting
            ? sprintSpeed
            : moveSpeed;

        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalVelocity;

        CollisionFlags collisions = controller.Move(
            velocity * Time.deltaTime
        );

        if ((collisions & CollisionFlags.Above) != 0
            && verticalVelocity > 0f)
        {
            verticalVelocity = 0f;
        }
    }

    private void HandleJump()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (jumpAction.WasPressedThisFrame())
            {
                verticalVelocity = Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );

                if (animator != null)
                {
                    animator.SetTrigger(JumpTrigger);
                }
            }
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void UpdateMovementAnimation(bool isMoving)
    {
        MovementState newState;

        if (!isMoving)
        {
            newState = MovementState.Idle;
        }
        else if (isSprinting && useSprintAnimation)
        {
            newState = MovementState.Sprinting;
        }
        else
        {
            newState = MovementState.Walking;
        }

        if (movementStateInitialized &&
            newState == currentMovementState)
        {
            return;
        }

        currentMovementState = newState;
        movementStateInitialized = true;

        if (animator == null)
        {
            return;
        }

        // Clear previous movement requests.
        animator.ResetTrigger(WalkingTrigger);
        animator.ResetTrigger(IdleTrigger);
        animator.ResetTrigger(SprintTrigger);

        switch (currentMovementState)
        {
            case MovementState.Idle:
                animator.SetTrigger(IdleTrigger);
                break;

            case MovementState.Walking:
                animator.SetTrigger(WalkingTrigger);
                break;

            case MovementState.Sprinting:
                animator.SetTrigger(SprintTrigger);
                break;
        }

        if (showAnimatorDebug)
        {
            Debug.Log(
                "Player movement animation: " +
                currentMovementState
            );
        }
    }

    private void DebugAnimator()
    {
        if (!showAnimatorDebug)
        {
            return;
        }

        if (animator == null)
        {
            Debug.LogError(
                "PlayerMovement: Animator is missing!"
            );
            return;
        }

        Debug.Log(
            "Player Animator: " + animator.gameObject.name
        );

        Debug.Log(
            "Animator Controller: " +
            (animator.runtimeAnimatorController != null
                ? animator.runtimeAnimatorController.name
                : "NONE")
        );

        foreach (AnimatorControllerParameter parameter
                 in animator.parameters)
        {
            Debug.Log(
                "Animator Parameter: " +
                parameter.name +
                " | Type: " +
                parameter.type
            );
        }

        CheckAnimatorTrigger("IsWalking");
        CheckAnimatorTrigger("IsIdle");
        CheckAnimatorTrigger("IsJumping");
        CheckAnimatorTrigger("IsSprinting");
    }

    private void CheckAnimatorTrigger(string parameterName)
    {
        foreach (AnimatorControllerParameter parameter
                 in animator.parameters)
        {
            if (parameter.name != parameterName)
            {
                continue;
            }

            if (parameter.type ==
                AnimatorControllerParameterType.Trigger)
            {
                Debug.Log(
                    "Animator Trigger OK: " + parameterName
                );
            }
            else
            {
                Debug.LogError(
                    parameterName + " is not a Trigger!"
                );
            }

            return;
        }

        Debug.LogError(
            "Missing Animator Trigger: " + parameterName
        );
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
            cameraPitch,
            cameraYaw,
            0f
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
        sprintAction?.Dispose();
    }
}
