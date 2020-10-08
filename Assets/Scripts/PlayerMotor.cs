using System;
using UnityEngine;

//For this player motor, I took the version I created for GDW and modified it to fit the context

public class PlayerMotor : MonoBehaviour
{
    [SerializeField] PlayerRotationController rotationController;

    [Header("Grounded Basic Movement")]
    [SerializeField] float accelerationRateGrounded = 0f;
    [SerializeField] float decelerationRateGrounded = 0f;
    [SerializeField] float maxMovementSpeedGrounded = 0f;

    [Header("Aerial Basic Movement")]
    [SerializeField] float accelerationRateAirborne = 0f;
    [SerializeField] float decelerationRateAirborne = 0f;
    [SerializeField] float maxMovementSpeedAirborne = 0f;

    [Header("Vertical Forces")]
    [SerializeField] float jumpHeight       = 0f;
    [SerializeField] float gravityStrength  = 0f;
    [SerializeField] float terminalVelocity = 0f;

    const float GROUNDED_VELOCITY_Y = -2f;

    const float COYOTE_TIME      = 0.05f;
    const float JUMP_BUFFER_TIME = 0.07f;

    const float SLOPE_RIDE_DISTANCE_LIMIT           = 3f;  //the max distance above a slope where the player can be considered to be "on" it
    const float SLOPE_RIDE_DOWNWARDS_FORCE_STRENGTH = 35f; //the strength of the downwards force applied to pull the player onto a slope that they're going down

    CharacterController m_characterController;
    Transform           m_transform;

    float m_lastTimeGrounded;
    float m_lastTimeJumpInputted = -999f;

    float m_currentAccelerationRate;
    float m_currentDecelerationRate;
    float m_currentMaxMovementSpeed;

    float m_jumpVelocityY;

    Vector3 m_velocity;
    public Vector3 Velocity
    {
        get
        {
            return m_velocity;
        }
    }

    bool m_isGrounded;
    public bool IsGrounded
    {
        get
        {
            return m_isGrounded;
        }
    }

    public float JumpHeight
    {
        get
        {
            return jumpHeight;
        }
        set
        {
            jumpHeight = value;
            UpdateJumpVelocityY();
        }
    }

    public float GetGroundedVelocityY()
    {
        return GROUNDED_VELOCITY_Y;
    }

    public void ResetVelocity()
    {
        m_velocity = Vector3.zero;
    }

    public void TeleportToPosition(Vector3 a_teleportPosition)
    {
        m_characterController.enabled = false;
        m_transform.position = a_teleportPosition;
        m_characterController.enabled = true;
    }

    void UpdateJumpVelocityY()
    {
        m_jumpVelocityY = Mathf.Sqrt(jumpHeight * 2f * gravityStrength); //calculate velocity needed in order to reach desired jump height
    }

    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_transform           = transform;

        UpdateJumpVelocityY();
    }

    public void Move(bool a_forward, bool a_backward, bool a_right, bool a_left, bool a_jump)
    {
        if (a_jump)
        {
            m_lastTimeJumpInputted = Time.time;
        }

        PerformGroundCheck();

        UpdateCurrentMovementVariables();

        int verticalAxis   = Convert.ToInt32(a_forward) - Convert.ToInt32(a_backward);
        int horizontalAxis = Convert.ToInt32(a_right)   - Convert.ToInt32(a_left);

        rotationController.SetRoll(0f);

        ProcessBasicMovement(verticalAxis, horizontalAxis);

        ApplyDeceleration(verticalAxis, horizontalAxis);

        //clamp magnitude of velocity on the xz plane
        Vector3 velocityXZ = new Vector3(m_velocity.x, 0f, m_velocity.z);
        velocityXZ = Vector3.ClampMagnitude(velocityXZ, m_currentMaxMovementSpeed);

        m_velocity = new Vector3(velocityXZ.x, m_velocity.y, velocityXZ.z);

        m_velocity.y -= gravityStrength * Time.deltaTime; //apply gravity

        //if the player is grounded, downwards velocity should be reset
        if (m_isGrounded && m_velocity.y < 0f)
        {
            m_velocity.y = GROUNDED_VELOCITY_Y; //don't set it to 0 or else the player might float above the ground a bit
        }

        if (m_lastTimeJumpInputted + JUMP_BUFFER_TIME >= Time.time)
        {
            Jump();
        }

        m_velocity.y = Mathf.Max(m_velocity.y, -terminalVelocity);

        m_characterController.Move(m_velocity * Time.deltaTime);

        //reset vertical velocity if the player hits a ceiling
        if ((m_characterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            m_velocity.y = 0f;
        }

        //after standard movement stuff is done, check if the player should be glued to a slope
        PerformOnSlopeLogic();
    }

    void PerformGroundCheck()
    {
        m_isGrounded = m_characterController.isGrounded;

        if (m_isGrounded)
        {
            m_lastTimeGrounded = Time.time;
        }
    }

    void UpdateCurrentMovementVariables()
    {
        if (m_isGrounded)
        {
            m_currentAccelerationRate = accelerationRateGrounded;
            m_currentDecelerationRate = decelerationRateGrounded;
            m_currentMaxMovementSpeed = maxMovementSpeedGrounded;
        }
        else
        {
            m_currentAccelerationRate = accelerationRateAirborne;
            m_currentDecelerationRate = decelerationRateAirborne;
            m_currentMaxMovementSpeed = maxMovementSpeedAirborne;
        }
    }

    void ProcessBasicMovement(int a_verticalAxis, int a_horizontalAxis)
    {
        //calculate movement direction
        Vector3 moveDir = m_transform.right   * a_horizontalAxis
                        + m_transform.forward * a_verticalAxis;

        moveDir = Vector3.Normalize(moveDir);

        //apply basic movement
        m_velocity += moveDir * m_currentAccelerationRate * Time.deltaTime;
    }

    void ApplyDeceleration(int a_verticalAxis, int a_horizontalAxis)
    {
        float   frameDecelerationAmount = m_currentDecelerationRate * Time.deltaTime;
        Vector3 localVelocity           = m_transform.InverseTransformDirection(m_velocity);

        //apply deceleration if the axis isn't being moved on
        if (a_verticalAxis == 0)
        {
            if (Mathf.Abs(localVelocity.z) > frameDecelerationAmount)
                m_velocity -= m_transform.forward * Mathf.Sign(localVelocity.z) * frameDecelerationAmount;
            else
                m_velocity -= m_transform.forward * localVelocity.z;
        }

        if (a_horizontalAxis == 0)
        {
            if (Mathf.Abs(localVelocity.x) > frameDecelerationAmount)
                m_velocity -= m_transform.right * Mathf.Sign(localVelocity.x) * frameDecelerationAmount;
            else
                m_velocity -= m_transform.right * localVelocity.x;
        }
    }

    void Jump()
    {
        if (m_lastTimeGrounded + COYOTE_TIME >= Time.time)
        {
            m_velocity.y = m_jumpVelocityY;

            m_lastTimeJumpInputted = -999f; //reset last jump time
        }
    }

    //this function should be called AFTER standard movement is applied for the current update
    void PerformOnSlopeLogic()
    {
        //calculate new isGrounded since player movement was just updated
        bool wasGrounded   = m_isGrounded;
        bool newIsGrounded = m_characterController.isGrounded;

        //glue the player to the slope if they're moving down one (fixes bouncing when going down slopes)
        if (!newIsGrounded && wasGrounded && m_velocity.y < 0f)
        {
            Vector3 pointAtBottomOfPlayer = m_transform.position - (Vector3.down * m_characterController.height / 2f);

            RaycastHit hit;
            if (Physics.Raycast(pointAtBottomOfPlayer, Vector3.down, out hit, SLOPE_RIDE_DISTANCE_LIMIT))
            {
                m_characterController.Move(Vector3.down * SLOPE_RIDE_DOWNWARDS_FORCE_STRENGTH * Time.deltaTime);
            }
        }
    }
}