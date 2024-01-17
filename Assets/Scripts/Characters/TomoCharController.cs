using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class TomoCharController : MonoBehaviour
{

    public enum ControllerState
    {
        Idle,
        LookAtCamera,
        Speaking
    }
    private ControllerState _state;
    [SerializeField] private float MaxSpeed;
    [SerializeField] private float MoveAcceleration;
    [SerializeField] private float MoveDecelleration;
    [SerializeField] private float RotateSpeed;

    private Vector3 cameraLookPos;


    private Rigidbody _rigidBody;
    private Animator _animator;
    private float _currentSpeed;

    public Vector3 debugCross;


    #region Idle Behaviour
    [SerializeField] private Vector2 idleStateTimes;
    [SerializeField] private float chanceOfIdleMovement;
    [SerializeField] private LayerMask blockingMask;
    [SerializeField] private float raycastDis;
    private Vector3 currentDir;
    private float stateTime;
    private float currentIdleStateTime;
    private bool stopIdleMovement;

    [SerializeField] private bool performGroundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDis;
    [SerializeField] private float groundCheckRad;
    private bool isFalling;
    [HideInInspector] public bool waitForFallingAnim;
    #endregion
    public System.Action CharacterTapped;

    Ray groundRay;
#if UNITY_EDITOR
    public bool debugRaycast;
    public Color raycastCol;
#endif
    private bool initialDelayGroundCheck;
    private void Awake()
    {
        groundRay = new Ray(transform.position, Vector3.down);
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();

        TomoCharInteraction interaction = GetComponent<TomoCharInteraction>();
        interaction.CharacterTapped += delegate { SwitchState(ControllerState.LookAtCamera); };
        interaction.SelectEnded += delegate { SwitchState(ControllerState.Idle); };
    }
    protected virtual void Update()
    {
        switch (_state)
        {
            case ControllerState.Idle:
                PerformIdleBehaviour();
                break;
            case ControllerState.LookAtCamera:
                PerformLookAtCamera();
                break;
            case ControllerState.Speaking:
                break;
        }



    }
    private void OnEnable()
    {
        initialDelayGroundCheck = false;
        isFalling = false;
        UpdateAnimator();
        StartCoroutine(DelayInitialGroundCheck());
    }
    private void OnDisable()
    {
        initialDelayGroundCheck = false;
        isFalling = false;
        UpdateAnimator();
    }
    private IEnumerator DelayInitialGroundCheck()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        initialDelayGroundCheck = true;
    }
    private void FixedUpdate()
    {
        if (performGroundCheck && initialDelayGroundCheck)
        {
            groundRay.origin = transform.position + Vector3.up * .5f;
            if (!Physics.SphereCast(groundRay, groundCheckRad, groundCheckDis + 0.5f, groundMask))
            {
                isFalling = true;
                waitForFallingAnim = true;
            }
            else
            {
                isFalling = false;
            }
        }
    }

    private void LateUpdate()
    {
        UpdateAnimator();
    }

    public void SwitchState(ControllerState newState)
    {
        _state = newState;
        switch (newState)
        {
            case ControllerState.Idle:
                currentIdleStateTime = 0;
                currentDir = Vector3.zero;
                stopIdleMovement = true;
                stateTime = Random.Range(idleStateTimes.x, idleStateTimes.y);
                break;
            case ControllerState.LookAtCamera:
                cameraLookPos = Camera.main.transform.position;
                break;
            case ControllerState.Speaking:
                break;
        }
    }

    #region Idle Behaviour
    private void PerformIdleBehaviour()
    {
        if (waitForFallingAnim)
        {
            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
            return;
        }
        if(currentIdleStateTime > stateTime)
        {
            currentIdleStateTime = 0;
            stateTime = Random.Range(idleStateTimes.x, idleStateTimes.y);
            if(Random.Range(0,1f) < chanceOfIdleMovement)
            {
                currentDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                stopIdleMovement = false;
            }
            else
            {
                currentDir = Vector3.zero;
                stopIdleMovement = true;
            }
        }

        if (stopIdleMovement || Physics.Raycast(transform.position, currentDir, raycastDis, blockingMask))
        {
            stopIdleMovement = true;
            if (_currentSpeed > 0)
            {
                Vector3 vel = transform.forward * CalcMoveSpeed(-MoveDecelleration);
                _rigidBody.velocity = new Vector3(vel.x, _rigidBody.velocity.y, vel.z);
            }
        }
        else
        {
            transform.Rotate(Vector3.up, CalcRotateSpeed(transform.position + currentDir));

            Vector3 vel = transform.forward * CalcMoveSpeed(MoveAcceleration);
            _rigidBody.velocity = new Vector3(vel.x, _rigidBody.velocity.y, vel.z);
            
        }
        currentIdleStateTime += Time.deltaTime;
    }

    #endregion
    private float CalcRotateSpeed(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;

        if (Vector3.Angle(transform.forward, MH.VectorOnYPlane(targetPos, transform.position.y)) > 90)
        {
            float side = Mathf.Sign(Vector3.Dot(transform.right, dir));
            return 1 * side * RotateSpeed;
        }
        Vector3 cross = Vector3.Cross(transform.forward, dir);
        debugCross = cross;
        Debug.DrawLine(transform.position, transform.position + debugCross, Color.magenta);
        return Mathf.Clamp(cross.y, -1, 1) * RotateSpeed;
    }
    private float CalcMoveSpeed(float speedAdditive)
    {

        _currentSpeed += speedAdditive * Time.fixedDeltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, MaxSpeed);

        return _currentSpeed;
    }



    #region Look At Camera
    private void PerformLookAtCamera()
    {
        Vector3 flattenedCamPos = cameraLookPos;
        flattenedCamPos.y = transform.position.y;

        transform.Rotate(Vector3.up, CalcRotateSpeed(MH.VectorOnYPlane(flattenedCamPos, transform.position.y)));

        if (_currentSpeed > 0)
        {
            Vector3 vel = transform.forward * CalcMoveSpeed(-MoveDecelleration);
            _rigidBody.velocity = new Vector3(vel.x, _rigidBody.velocity.y, vel.z);
        }

    }
    #endregion
    
    private void UpdateAnimator()
    {
        _animator.SetFloat("MoveSpeed", _currentSpeed / MaxSpeed);
        _animator.SetBool("IsFalling", isFalling);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (debugRaycast)
        {
            if (Application.isPlaying)
            {
                Debug.DrawLine(transform.position, transform.position + currentDir * raycastDis, raycastCol);
            }
            else
            {
                Debug.DrawLine(transform.position, transform.position + transform.forward * raycastDis, raycastCol);
            }

            Debug.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDis, Color.yellow);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDis, groundCheckRad);

        }
    }
#endif
}
