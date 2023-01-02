using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Main Properties")]
    [SerializeField] float speed = 6.5f;
    [SerializeField] float jumpForce = 5.5f;
    [SerializeField] Transform head;
    [SerializeField] Transform _camera;
    [SerializeField] LayerMask ground;

    float x, y, horizontalInput, verticalInput;
    Rigidbody rb;
    bool grounded;

    void Awake()
    {
        InputManager.Instance = new InputManager();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        DefineRigidbodyValues();
        CursorValues(CursorLockMode.Locked, false);
    }

    void DefineRigidbodyValues()
    {
        rb.mass = transform.localScale.y;
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.freezeRotation = true;
    }

    void CursorValues(CursorLockMode lockState, bool visible)
    {
        Cursor.lockState = lockState;
        Cursor.visible = visible;
    }

    void Update()
    {
        GetInputs();
		horizontalInput = InputManager.Instance.horizontalInput;
        verticalInput = InputManager.Instance.verticalInput;
        Look();
    }

    void GetInputs()
    {
        // Detect basic movement
        if (Input.GetKeyDown(InputManager.Instance.forwardKey)) InputManager.Instance.verticalInput = 1f;
        if (Input.GetKeyUp(InputManager.Instance.forwardKey) && InputManager.Instance.verticalInput != 0f) InputManager.Instance.verticalInput = 0f;
        if (Input.GetKeyDown(InputManager.Instance.backwardKey)) InputManager.Instance.verticalInput = -1f;
        if (Input.GetKeyUp(InputManager.Instance.backwardKey) && InputManager.Instance.verticalInput != 0f) InputManager.Instance.verticalInput = 0f;
        if (Input.GetKeyDown(InputManager.Instance.leftKey)) InputManager.Instance.horizontalInput = -1f;
        if (Input.GetKeyUp(InputManager.Instance.leftKey) && InputManager.Instance.horizontalInput != 0f) InputManager.Instance.horizontalInput = 0f;
        if (Input.GetKeyDown(InputManager.Instance.rightKey)) InputManager.Instance.horizontalInput = 1f;
        if (Input.GetKeyUp(InputManager.Instance.rightKey) && InputManager.Instance.horizontalInput != 0f) InputManager.Instance.horizontalInput = 0f;
    }


    void Look()
    {

        x += -Input.GetAxis("Mouse Y") * InputManager.Instance.mouseSensitivityY * Time.fixedDeltaTime;
        y += Input.GetAxis("Mouse X") * InputManager.Instance.mouseSensitivityX * Time.fixedDeltaTime;

        x = Mathf.Clamp(x, -90, 90);

        transform.localRotation = Quaternion.Euler(0, y, 0);
        head.localRotation = Quaternion.Euler(x, 0, 0);

        // Apply rotation to the actuall camera;
        _camera.position = head.position;
        _camera.rotation = head.rotation;
    }

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    void Move()
    {
		Vector3 moveDir = transform.forward * verticalInput + transform.right * horizontalInput;
        if (grounded) {
            rb.MovePosition(transform.position + moveDir.normalized * speed * Time.deltaTime);
        }
        else
        {
            rb.MovePosition(transform.position + moveDir.normalized * speed * Time.deltaTime * .5f);
        }
    }

    void Jump()
    {
        if (Input.GetKey(InputManager.Instance.jumpKey) && grounded) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionStay(Collision col)
    {
        int layer = col.gameObject.layer;
        if (ground == (ground | (1 >> layer))) grounded = true;
    }

    void OnCollisionExit(Collision col)
    {
        int layer = col.gameObject.layer;
        if (ground == (ground | (1 >> layer))) grounded = false;
    }
}
public class InputManager
{
    public enum ClickingMethod { Left, Middle, Right }
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public ClickingMethod primaryClick = ClickingMethod.Left;
    public ClickingMethod secondaryClick = ClickingMethod.Right;
    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;
	public float horizontalInput, verticalInput;
    public static InputManager Instance;

    public int PrimaryClickNoCool()
    {
        if (primaryClick == ClickingMethod.Left) return 0;
        else if (primaryClick == ClickingMethod.Middle) return 2;
        return 1;
    }

    public int SecondaryClickNoCool()
    {
        if (secondaryClick == ClickingMethod.Left) return 0;
        else if (secondaryClick == ClickingMethod.Middle) return 2;
        return 1;
    }

    public string PrimaryClickCool()
    {
        if (primaryClick == ClickingMethod.Left) return "Fire1";
        else if (primaryClick == ClickingMethod.Middle) return "Fire3";
        return "Fire2";
    }

    public string SecondaryClickCool()
    {
        if (secondaryClick == ClickingMethod.Left) return "Fire1";
        else if (secondaryClick == ClickingMethod.Middle) return "Fire3";
        return "Fire2";
    }
}
