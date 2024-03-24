using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float rotateSpeed;
    private PlayerInput _inputActions;
    private CharacterController _controller;
    private Animator _animator;
    private Vector2 _movementInput;
    private Vector3 _currentMovement;
    private Quaternion _rotateDir;
    private bool _isRunning;
    private bool _isWalking;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private PhotonView _pv;
    [SerializeField] private CameraFlow myCamScript;

    private void OnMovementActions(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _movementInput.x;
        _currentMovement.z = _movementInput.y;
        _isWalking = _movementInput.x != 0 || _movementInput.y != 0;
    }

    private void Awake()
    {
        _pv = GetComponentInParent<PhotonView>();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _inputActions = new PlayerInput();
        _inputActions.CharacterControls.Movement.started += OnMovementActions;
        _inputActions.CharacterControls.Movement.performed += OnMovementActions;
        _inputActions.CharacterControls.Movement.canceled += OnMovementActions;
        _inputActions.CharacterControls.Run.started += OnRun;
        _inputActions.CharacterControls.Run.canceled += OnRun;
        _inputActions.CharacterControls.Movement.started += OnCameraMovement;
        _inputActions.CharacterControls.Movement.performed += OnCameraMovement;
        _inputActions.CharacterControls.Movement.canceled += OnCameraMovement;
        if (!_pv.IsMine)
        {
            Destroy(myCamScript.gameObject);
        }
    }

    private void OnEnable()
    {
        _inputActions.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _inputActions.CharacterControls.Disable();
    }

    private void PlayerRotate()
    {
        if (_isWalking)
        {
            _rotateDir = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_currentMovement),
                Time.deltaTime * rotateSpeed);
            transform.rotation = _rotateDir;
        }
    }

    private void AnimateControl()
    {
        _animator.SetBool(IsWalking, _isWalking);
        _animator.SetBool(IsRunning, _isRunning);
    }

    private void Update()
    {
        if (!_pv.IsMine) return;
        AnimateControl();
        PlayerRotate();
    }

    private void FixedUpdate()
    {
        if (!_pv.IsMine) return;
        _controller.Move(_currentMovement * Time.fixedDeltaTime);
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        _isRunning = context.ReadValueAsButton();
    }
    
    private void OnCameraMovement(InputAction.CallbackContext context)
    {
        myCamScript.SetOffset(_currentMovement);
    }
    
    public void Respawn()
    {
        _controller.enabled = false;
        transform.position = Vector3.up;
        _controller.enabled = true;
    }

}
