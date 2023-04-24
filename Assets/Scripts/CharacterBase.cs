using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    
    private Vector3 _targetPos;
    private bool _isMove;

    public Action<Vector3> OnStartMove;
    public Action OnStopMove;

    public Action<string> OnEnterState;
    public Action<string> OnExitState;
    
    public Vector3 ViewDirection { get; set; }

    private string _currentState;

    private bool _canMove = true;
    
    private void Awake()
    {
        _currentState = "Idle";
        OnEnterState(_currentState);
    }

    void Update()
    {
        if (_canMove && Input.GetMouseButtonDown(0))
        {
            InputRaycast();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Die();
        }
        MoveTo();
    }

    private void InputRaycast()
    {
        Vector3 screenPos = Input.mousePosition;
        _targetPos = Camera.main.ScreenToWorldPoint(screenPos);
        _targetPos.z = 0;
        Debug.Log("target: "+_targetPos);
        _isMove = true;
        OnStartMove?.Invoke(_targetPos);
        //
        OnExitState?.Invoke(_currentState);
        _currentState = "Move";
        OnEnterState?.Invoke(_currentState);
    }

    private void MoveTo()
    {
        if (!_isMove)
            return;
        
        if(Vector3.Distance(transform.position, _targetPos) > 0.02f)
        {
            // Calculate the direction to move the GameObject
            Vector3 moveDirection = (_targetPos - transform.position).normalized;
            Vector3 targetPoint = moveDirection * _speed * Time.deltaTime;
            transform.position += new Vector3(targetPoint.x, targetPoint.y, 0);
            ViewDirection = moveDirection;
        }
        else
        {
            OnExitState?.Invoke(_currentState);
            _currentState = "Idle";
            OnEnterState?.Invoke(_currentState);
            OnStopMove?.Invoke();
            _isMove = false;
        }
    }
    
    private void Die()
    {
        _canMove = false;
        OnExitState?.Invoke(_currentState);
        OnEnterState?.Invoke("Die");
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _targetPos);
    }
    
    
    private void RotateCharacterTo()
    {
        Vector3 moveDirection = (_targetPos - transform.position).normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        Debug.Log("Angle: "+angle);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //Vector3 targetLook = new Vector3(_targetPos.x, _targetPos.y, transform.position.z);
        //transform.LookAt(targetLook);
        
        
        //Vector3 newDirection = Vector3.RotateTowards(transform.forward, moveDirection, 0, 0.0f);
        //transform.rotation = Quaternion.LookRotation(newDirection);

        //Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        //transform.rotation = Vector3.RotateTowards(transform.rotation, q,0,0);
    }
}
