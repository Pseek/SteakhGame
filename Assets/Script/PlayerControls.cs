using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerControls : MonoBehaviour
{
    public Rigidbody _rb;

    [Header("Movement")]
    public Vector2 _direction;
    
    public float _moveSpeed = 5f;
    public float _currentSpeed = 5f;
    public float _runSpeed = 7.5f;
    private bool _isRunning;

    [Header("Jump")]

    public bool isJumped = false;
    public bool isGrounded =false;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public Vector3 groundCheckerSize = Vector3.one;
    public Transform groundCheckerTransform;

    public States currentStates = States.IDLE;
    public enum States
    {
        IDLE,JOGG,RUN,FALL,SNEAK,JUMP
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       OnCheckGround();
       OnStateUdpate();

       
    }

    public void OnCheckGround()
    {
        Collider[] ground = Physics.OverlapBox(groundCheckerTransform.position, groundCheckerSize,Quaternion.identity,groundLayer);

        isGrounded = ground.Length > 0;  
    }
    public void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawCube(groundCheckerTransform.position, groundCheckerSize);
    }

    public void OnStateEnter()
    {
        switch(currentStates)
        {
            case States.IDLE:
                break;
            case States.JOGG:
                _currentSpeed = _moveSpeed;
                break;
            case States.RUN: 
                _currentSpeed = _runSpeed;
                break;
            case States.FALL: 
                break;
            case States.SNEAK: 
                break;
            case States.JUMP:
                _rb.velocity = new Vector3(_rb.velocity.x , 10f , _rb.velocity.z);
                break;

        }
    }
    public void OnStateUdpate()
    {
        switch (currentStates)
        {
            case States.IDLE:

                if (_direction.magnitude > 0f)
                {
                    TransitionToState(States.JOGG);
                }
                if ( _direction.magnitude > 0f &&_isRunning)
                {
                    TransitionToState(States.RUN);
                }
                if (isJumped && isGrounded)
                {
                    TransitionToState(States.JUMP);
                }

                break;
            case States.JOGG:   
                
                _rb.velocity = new Vector3(_direction.x * _currentSpeed, _rb.velocity.y, _direction.y * _currentSpeed ) ;

                if (_direction.magnitude < 0.1f)
                {
                    TransitionToState(States.IDLE);
                }
                if (_isRunning && _direction.magnitude > 0.1f)
                {
                    TransitionToState(States.RUN);
                }
                if (isJumped && isGrounded)
                {
                    TransitionToState(States.JUMP);
                }
                break;
            case States.RUN:

                _rb.velocity = new Vector3(_direction.x * _currentSpeed, _rb.velocity.y, _direction.y * _currentSpeed);

                if (_direction.magnitude < 0.1f)
                {
                    TransitionToState(States.IDLE);
                }
                if (_direction.magnitude > 0.1f && !_isRunning)
                {
                    TransitionToState(States.JOGG);
                }
                if (isJumped && isGrounded)
                {
                    TransitionToState(States.JUMP);
                }
                break;
            case States.JUMP:

                _rb.velocity = new Vector3(_direction.x * _currentSpeed, _rb.velocity.y, _direction.y * _currentSpeed);
                
                if(_rb.velocity.y <= 0f)
                {
                    TransitionToState(States.FALL);
                }

                    break;
            case States.FALL:

                _rb.velocity = new Vector3(_direction.x * _currentSpeed, _rb.velocity.y, _direction.y * _currentSpeed);

                if (isGrounded)
                {
                    if (_direction.magnitude < 0.1f)
                    {
                        TransitionToState(States.IDLE);
                    }
                    if (_direction.magnitude > 0.1f && !_isRunning)
                    {
                        TransitionToState(States.JOGG);
                    }
                    if (_isRunning && _direction.magnitude > 0.1f)
                    {
                        TransitionToState(States.RUN);
                    }
                }
                break;
            case States.SNEAK:
                break;

        }
    }
    public void OnStateExit()
    {
        switch (currentStates)
        {
            case States.IDLE:
                break;
            case States.JOGG:
                break;
            case States.RUN:
                break;
            case States.JUMP:
                break;
            case States.FALL:
                break;
            case States.SNEAK:
                break;

        }
    }
    public void TransitionToState(States newState)
    {
        OnStateExit();
        currentStates = newState;
        OnStateEnter();
    }

    public void Move(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _direction = context.ReadValue<Vector2>();
                break;
            case InputActionPhase.Canceled:
                _direction = Vector2.zero;
                break;
        }
    }

    public void Run(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
               _isRunning = true;
                break;
            case InputActionPhase.Canceled:
                _isRunning = false;
                break;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                isJumped = true;
                break;
            case InputActionPhase.Canceled:
                isJumped = false;   
                break;
        }
    }
}
