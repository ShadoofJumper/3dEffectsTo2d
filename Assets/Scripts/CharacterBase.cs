using System;
using UnityEditor;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    
    private Vector3 _targetPos;
    private bool _isMove;

    public Action<Vector3> OnStartMove;
    public Action OnStopMove;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InputRaycast();
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
        //RotateCharacterTo();
        /*
        // Cast a ray from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) // Check if the ray hits a plane
        {
            // Set the position of the GameObject to the hit point
            Debug.Log("target: "+_targetPos);
            _targetPos = hit.point;
            _isMove = true;
            OnStartMove?.Invoke(_targetPos);
            RotateCharacterTo();
        }
        */
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
        }
        else
        {
            OnStopMove?.Invoke();
            _isMove = false;
        }
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
