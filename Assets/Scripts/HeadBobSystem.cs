using UnityEngine;
using UnityEngine.InputSystem;

public class HeadBobSystem : MonoBehaviour, Controls.IPlayerActions
{
    [Range(0.01f, 0.2f)]
    public float amount = 0.05f;
    [Range(1f, 30f)]

    public float frequency = 12.0f;

    [Range(1f, 30f)]
    public float smooth = 10.0f;

    Vector3 _startPos;
    private Controls _controls;
    private Vector2 _moveInput;
    private float _timer;

    private void Awake()
    {
        _controls = new Controls();
        _controls.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        if(_controls != null)
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
        _startPos = transform.localPosition;
    }

    void Update()
    {
        CheckForHeadBobTrigger();
    }

    private void CheckForHeadBobTrigger()
    {
        float inputMagnitude = new Vector3(_moveInput.x, 0, _moveInput.y).magnitude;

        if(inputMagnitude > 0)
        {
            StartHeadBob();
        } else
        {
            StopHeadBob();
        }
    }

    private Vector3 StartHeadBob()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amount * 1.4f;
        pos.x += Mathf.Cos(Time.time * frequency / 2f) * amount * 1.6f;

        transform.localPosition = Vector3.Lerp(transform.localPosition, _startPos + pos, smooth * Time.deltaTime);

        return pos;
    }

    private void StopHeadBob()
    {
        if (transform.localPosition == _startPos) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, _startPos, smooth * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }
}
