using UnityEngine;

public class PlayerRotationController : MonoBehaviour
{
    [Header("General")]
    public float mouseSensitivity;

    public Transform playerTransform;

    public PlayerMotor playerMotor;

    [Header("Camera Bobbing")]
    public float maxCameraBobOffset;
    public float cameraBobSpeed;

    [HideInInspector] public float yaw;
    [HideInInspector] public float pitch;
    [HideInInspector] public float roll;

    Transform m_transform;

    float m_currentFrameMouseX;

    float m_cameraBobInterpolationParam = 0.5f;
    bool  m_isCameraBobbingUp           = true;
    float m_initialCameraY;

    public float CurrentFrameMouseX
    {
        get
        {
            return m_currentFrameMouseX;
        }
    }

    float m_currentFrameMouseY;

    public float CurrentFrameMouseY
    {
        get
        {
            return m_currentFrameMouseY;
        }
    }

    void Start()
    {
        m_transform = transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        yaw = playerTransform.eulerAngles.y;

        m_initialCameraY = m_transform.localPosition.y;

        ApplyCameraBobbing(); //apply 1 frame of camera bobbing at the start to allow a smoother transition once moving starts
    }

    void Update()
    {
        m_currentFrameMouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
        m_currentFrameMouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;

        pitch -= m_currentFrameMouseY;
        pitch =  Mathf.Clamp(pitch, -80f, 80f);

        Vector3 newRotation = m_transform.localRotation.eulerAngles;
        newRotation.x = pitch;
        m_transform.localRotation = Quaternion.Euler(newRotation);

        yaw += m_currentFrameMouseX;
        playerTransform.localRotation = Quaternion.Euler(0f, yaw, 0f);

        if (playerMotor.IsGrounded)
        {
            ApplyCameraBobbing();
        }
    }

    public Vector3 GetEulerRotation()
    {
        return new Vector3(pitch, yaw, roll);
    }

    public void SetEulerRotation(Vector3 newEulerAngles)
    {
        pitch = newEulerAngles.x;
        yaw   = newEulerAngles.y;
        roll  = newEulerAngles.z;

        Vector3 newCameraRotation = m_transform.localRotation.eulerAngles;
        newCameraRotation.x = pitch;
        newCameraRotation.z = roll;
        m_transform.localRotation = Quaternion.Euler(newCameraRotation);

        playerTransform.localRotation = Quaternion.Euler(0f, yaw, 0f);
    }

    public void SetRoll(float a_newRoll)
    {
        roll = a_newRoll;
        Vector3 newRotation = m_transform.localRotation.eulerAngles;
        newRotation.z = roll;
        m_transform.localRotation = Quaternion.Euler(newRotation);
    }

    void ApplyCameraBobbing()
    {
        float playerVelocityMagnitudeXZ = new Vector3(playerMotor.Velocity.x, 0f, playerMotor.Velocity.z).magnitude;

        float velocityRelativeToMax = playerVelocityMagnitudeXZ / playerMotor.maxMovementSpeedGrounded;

        m_cameraBobInterpolationParam += cameraBobSpeed * velocityRelativeToMax * Time.deltaTime;
        if (m_cameraBobInterpolationParam > 1f)
        {
            m_isCameraBobbingUp = !m_isCameraBobbingUp;
            m_cameraBobInterpolationParam = 0f;
        }

        Vector3 newPosition = m_transform.localPosition;
        float startY;
        float endY;
        if (m_isCameraBobbingUp)
        {
            startY = m_initialCameraY - maxCameraBobOffset;
            endY   = m_initialCameraY + maxCameraBobOffset;
        }
        else
        {
            startY = m_initialCameraY + maxCameraBobOffset;
            endY   = m_initialCameraY - maxCameraBobOffset;
        }
        newPosition.y = Mathf.Lerp(startY, endY, m_cameraBobInterpolationParam);

        m_transform.localPosition = newPosition;
    }
}
