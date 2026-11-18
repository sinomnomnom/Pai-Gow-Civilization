using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, Controls.IPlayerActions
{
    [SerializeField] private float sensitivity = 0.001f;
    [SerializeField] private float smoothing = 2.0f;

    private GameObject character;
    private Controls _controls;
    private Vector2 _lookInput;

    private Vector2 mouseLook;
    private Vector2 smoothV;

    private void Awake()
    {
        _controls = new Controls();
        _controls.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        if (_controls != null)
        {
            _controls.Player.Enable();
        }
    }

    private void OnDisable()
    {
        if (_controls != null)
        {
            _controls.Player.Disable();
        }
    }

    private void Start()
    {
        if (transform.parent != null)
        {
            character = transform.parent.gameObject;
        }
  

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (character == null) return;

        Vector2 md = _lookInput * (sensitivity / 10);

        float smoothFactor = Mathf.Max(1f, smoothing);
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothFactor);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothFactor);

        mouseLook += smoothV;

        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, Vector3.up);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }
}
