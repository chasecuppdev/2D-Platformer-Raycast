using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControllerNew : MonoBehaviour
{
    #region Public Properties
    public float runSpeed = 4;
    public float acceleration = 20;
    public float jumpSpeed = 5;
    public float gravity = 15;
    public Vector2 influence = new Vector2(5, 5);
    public AudioClip[] sounds;

    #endregion
    //--------------------------------------------------------------------------------
    #region Private Properties
    Animator animator;
    AudioSource audioSource;
    Vector3 defaultScale;
    float groundY;
    bool grounded;
    float stateStartTime;

    float timeInState
    {
        get { return Time.time - stateStartTime; }
    }

    const string kIdleAnim = "Idle";
    const string kRunAnim = "Run";
    const string kJumpStartAnim = "JumpStart";
    const string kJumpFallAnim = "JumpFall";
    const string kJumpLandAnim = "JumpLand";

    enum State
    {
        Idle,
        RunningRight,
        RunningLeft,
        JumpingUp,
        JumpingDown,
        Landing
    }
    State state;
    Vector2 velocity;
    float horzInput;
    bool jumpJustPressed;
    bool jumpHeld;
    int airJumpsDone = 0;

    #endregion
    //--------------------------------------------------------------------------------
    #region MonoBehaviour Events
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        defaultScale = transform.localScale;
        groundY = transform.position.y;
    }

    void Update()
    {
        // Gather inputs
        horzInput = Input.GetAxisRaw("Horizontal");
        jumpJustPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");

        // Update state
        ContinueState();

        // Update position
        UpdateTransform();
    }

    #endregion
    //--------------------------------------------------------------------------------
    #region Public Methods
    public void PlaySound(string name)
    {
        if (!audioSource.enabled) return;
        foreach (AudioClip clip in sounds)
        {
            if (clip.name == name)
            {
                audioSource.clip = clip;
                audioSource.Play();
                return;
            }
        }
        Debug.LogWarning(gameObject + ": AudioClip not found: " + name);
    }
    #endregion
    //--------------------------------------------------------------------------------
    #region Private Methods
    void SetOrKeepState(State state)
    {
        if (this.state == state) return;
        EnterState(state);
    }

    void ExitState()
    {
    }

    void EnterState(State state)
    {
        ExitState();
        switch (state)
        {
            case State.Idle:
                animator.Play(kIdleAnim);
                break;
            case State.RunningLeft:
                animator.Play(kRunAnim);
                Face(-1);
                break;
            case State.RunningRight:
                animator.Play(kRunAnim);
                Face(1);
                break;
            case State.JumpingUp:
                animator.Play(kJumpStartAnim);
                velocity.y = jumpSpeed;
                break;
            case State.JumpingDown:
                animator.Play(kJumpFallAnim);
                break;
            case State.Landing:
                animator.Play(kJumpLandAnim);
                airJumpsDone = 0;
                break;
        }

        this.state = state;
        stateStartTime = Time.time;
    }

    void ContinueState()
    {
        switch (state)
        {

            case State.Idle:
                RunOrJump();
                break;

            case State.RunningLeft:
            case State.RunningRight:
                if (!RunOrJump()) EnterState(State.Idle);
                break;

            case State.JumpingUp:
                if (velocity.y < 0) EnterState(State.JumpingDown);
                if (jumpJustPressed && airJumpsDone < 1)
                {
                    EnterState(State.JumpingUp);
                    airJumpsDone++;
                }
                break;

            case State.JumpingDown:
                if (grounded) EnterState(State.Landing);
                if (jumpJustPressed && airJumpsDone < 1)
                {
                    EnterState(State.JumpingUp);
                    airJumpsDone++;
                }
                break;

            case State.Landing:
                if (timeInState > 0.2f) EnterState(State.Idle);
                else if (timeInState > 0.1f) RunOrJump();
                break;
        }
    }

    bool RunOrJump()
    {
        if (jumpJustPressed && grounded) SetOrKeepState(State.JumpingUp);
        else if (horzInput < 0) SetOrKeepState(State.RunningLeft);
        else if (horzInput > 0) SetOrKeepState(State.RunningRight);
        else return false;
        return true;
    }


    void Face(int direction)
    {
        transform.localScale = new Vector3(defaultScale.x * direction, defaultScale.y, defaultScale.z);
    }

    void UpdateTransform()
    {

        if (grounded)
        {
            float targetSpeed = 0;
            switch (state)
            {
                case State.RunningLeft:
                    targetSpeed = -runSpeed;
                    break;
                case State.RunningRight:
                    targetSpeed = runSpeed;
                    break;
            }
            velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // vertical influence directly counteracts gravity
            if (jumpHeld) velocity.y += influence.y * Time.deltaTime;

            // horizontal influence is an acceleration towards the target speed
            // (just like when running, but the acceleration should be much lower)
            float targetSpeed = horzInput * runSpeed;
            velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, influence.x * Time.deltaTime);
        }
        velocity.y -= gravity * Time.deltaTime;

        Vector3 newPos = transform.position + (Vector3)(velocity * Time.deltaTime);
        if (newPos.y < groundY)
        {
            newPos.y = groundY;
            velocity.y = 0;
            grounded = true;
        }
        else grounded = false;
        transform.position = newPos;
    }

    #endregion
}
