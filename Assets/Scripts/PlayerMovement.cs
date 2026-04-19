using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Движение")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    
    [Header("Поворот мышкой")]
    public float mouseSensitivity = 2f;
    
    [Header("Гравитация")]
    public float gravity = -9.81f;
    
    [Header("Ссылки")]
    public Camera playerCamera;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isRunning;
    private float xRotation = 0f;
    private bool cursorLocked = true;
    
    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("[PlayerMovement] CharacterController не найден на объекте. Добавьте компонент или проверьте объект игрока.");
            enabled = false;
            return;
        }

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogWarning("[PlayerMovement] Камера не назначена и не найдена во вложенных объектах.");
            }
        }
    }

    void Start()
    {
        SetCursorState(true);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorState(false);
        }

        if (!cursorLocked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetCursorState(true);
            }
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        
        transform.Rotate(Vector3.up * mouseX);
        
        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        Vector3 moveVelocity = move * currentSpeed;

        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        controller.Move((moveVelocity + velocity) * Time.deltaTime);
    }

    void SetCursorState(bool locked)
    {
        cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}