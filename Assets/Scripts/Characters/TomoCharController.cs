using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float StopDistance;

    private Vector3 idleTargetPos;
    private Vector3 cameraLookPos;


    private Rigidbody _rigidBody;
    private Animator _animator;
    private float _currentSpeed;

    public Vector3 debugCross;
    public float distance;

    public System.Action CharacterTapped;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();

        TomoCharInteraction interaction = GetComponent<TomoCharInteraction>();
        interaction.CharacterTapped += delegate { SwitchState(ControllerState.LookAtCamera); };
        interaction.SelectEnded += delegate { SwitchState(ControllerState.Idle); };
    }
    private void Update()
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
        
        distance = Vector3.Distance(transform.position, MH.VectorOnYPlane(idleTargetPos, transform.position.y));
        if (distance > StopDistance)
        {

            transform.Rotate(Vector3.up, CalcRotateSpeed(idleTargetPos));

            Vector3 vel = transform.forward * CalcMoveSpeed(MoveAcceleration);
            _rigidBody.velocity = new Vector3(vel.x, _rigidBody.velocity.y, vel.z);
        }
        else
        {
            if (_currentSpeed > 0)
            {
                Vector3 vel = transform.forward * CalcMoveSpeed(-MoveDecelleration);
                _rigidBody.velocity = new Vector3(vel.x, _rigidBody.velocity.y, vel.z);
            }
        }
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
    }



}
