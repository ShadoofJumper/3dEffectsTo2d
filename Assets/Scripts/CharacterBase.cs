using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    
    private Vector3 _targetPos;
    private bool _isMove;
    
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
        // Cast a ray from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) // Check if the ray hits a plane
        {
            // Set the position of the GameObject to the hit point
            Debug.Log("target: "+_targetPos);
            _targetPos = hit.point;
            _isMove = true;
            RotateCharacterTo();
        }
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
            transform.position += new Vector3(targetPoint.x, 0, targetPoint.z);
        }
        else
        {
            _isMove = false;
        }
    }
    
    private void RotateCharacterTo()
    {
        Vector3 moveDirection = (_targetPos - transform.position).normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        Debug.Log("Angle: "+angle);
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 targetLook = new Vector3(_targetPos.x, transform.position.y, _targetPos.z);
        transform.LookAt(targetLook);
    }
}
