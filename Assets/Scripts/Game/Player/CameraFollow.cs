using UnityEngine;
using Zenject;

public class CameraFollow : MonoBehaviour
{
    [Range(0, 15)] [SerializeField] private float _speedMovement = 3f;
    [Range(0, 15)] [SerializeField] private float _speedRotation = 5f;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _lookOffset = Vector3.zero;
    
    private Transform _target;
    
    [Inject]
    private void Construct(PlayerRoadMovement playerMovement)
    {
        _target = playerMovement.gameObject.transform;
    }
    
    private void Start()
    {
        Vector3 targetCameraPosition = _target.position + _positionOffset;
        transform.position = targetCameraPosition;

        transform.LookAt(_target.position + _lookOffset);
    }

    private void LateUpdate()
    {
        MoveCamera();
        RotateCamera();
    }

    private void MoveCamera()
    {
        Vector3 targetCameraPosition = _target.position + _positionOffset;
        transform.position = Vector3.Lerp(transform.position, targetCameraPosition, _speedMovement * Time.deltaTime);
    }

    private void RotateCamera()
    {
        Vector3 lookTarget = _target.position + _lookOffset;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _speedRotation * Time.deltaTime);
    }
}
