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
    public float MaxSpeed;
    public float MoveAcceleration;
    public float MoveDecelleration;
    public float RotateSpeed;
    public float StopDistance;

    public Transform TargetTransform;


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
        interaction.DialogueEnded += delegate { SwitchState(ControllerState.Idle); };

        if (TargetTransform == null)
        {
            TargetTransform = GameObject.Find("TOMOCHARTargetPos").transform;
        }
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
    }

    #region Idle Behaviour
    private void PerformIdleBehaviour()
    {
        distance = Vector3.Distance(transform.position, MH.VectorOnYPlane(TargetTransform.position, transform.position.y));
        if (distance > StopDistance)
        {

            transform.Rotate(Vector3.up, CalcRotateSpeed(TargetTransform.position));

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
        Vector3 flattenedCamPos = Camera.main.transform.position;

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
