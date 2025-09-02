using UnityEngine;
using Zenject;

public class CameraFollow : MonoBehaviour
{
    [Header("Speeds")]
    [Range(0, 15)] [SerializeField] private float _speedMovement = 3f;
    [Range(0, 15)] [SerializeField] private float _speedRotation = 5f;

    [Header("Offsets (Normal)")]
    [SerializeField] private Vector3 _normalPositionOffset;
    [SerializeField] private Vector3 _normalLookOffset = Vector3.zero;

    [Header("Offsets (Battle)")]
    [SerializeField] private Vector3 _battlePositionOffset;
    [SerializeField] private Vector3 _battleLookOffset = Vector3.zero;

    [Header("Transition")]
    [SerializeField] private float _offsetTransitionSpeed = 5f;

    [Header("Options")]
    [SerializeField] private bool _snapOnSwitch = false;

    private Transform _target;
    private BattlePlatform _battlePlatform;

    private Vector3 _currentPositionOffset;
    private Vector3 _currentLookOffset;
    private Vector3 _targetPositionOffset;
    private Vector3 _targetLookOffset;

    private bool _isInBattle = false;

    [Inject]
    private void Construct(PlayerRoadMovement playerMovement, BattlePlatform battlePlatform)
    {
        _target = playerMovement.gameObject.transform;
        _battlePlatform = battlePlatform;
    }

    private void Start()
    {
        _currentPositionOffset = _targetPositionOffset = _normalPositionOffset;
        _currentLookOffset = _targetLookOffset = _normalLookOffset;

        if (_target != null)
        {
            Vector3 targetCameraPosition = _target.position + _currentPositionOffset;
            transform.position = targetCameraPosition;
            transform.LookAt(_target.position + _currentLookOffset);
        }
    }

    private void OnEnable()
    {
        if (_battlePlatform != null)
        {
            _battlePlatform.OnPlayerEnterBattleZone += SwitchToBattleOffset;
            _battlePlatform.OnPlayerExitBattleZone += SwitchToNormalOffset;
        }
    }

    private void OnDisable()
    {
        if (_battlePlatform != null)
        {
            _battlePlatform.OnPlayerEnterBattleZone -= SwitchToBattleOffset;
            _battlePlatform.OnPlayerExitBattleZone -= SwitchToNormalOffset;
        }
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        _targetPositionOffset = _isInBattle ? _battlePositionOffset : _normalPositionOffset;
        _targetLookOffset = _isInBattle ? _battleLookOffset : _normalLookOffset;

        _currentPositionOffset = Vector3.Lerp(_currentPositionOffset, _targetPositionOffset, _offsetTransitionSpeed * Time.deltaTime);
        _currentLookOffset = Vector3.Lerp(_currentLookOffset, _targetLookOffset, _offsetTransitionSpeed * Time.deltaTime);

        MoveCamera();
        RotateCamera();
    }

    private void MoveCamera()
    {
        Vector3 targetCameraPosition = _target.position + _currentPositionOffset;
        transform.position = Vector3.Lerp(transform.position, targetCameraPosition, _speedMovement * Time.deltaTime);
    }

    private void RotateCamera()
    {
        Vector3 lookTarget = _target.position + _currentLookOffset;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _speedRotation * Time.deltaTime);
    }

    private void SwitchToBattleOffset()
    {
        _isInBattle = true;

        if (_snapOnSwitch)
            SnapToTargetOffsets();
    }

    private void SwitchToNormalOffset()
    {
        _isInBattle = false;

        if (_snapOnSwitch)
            SnapToTargetOffsets();
    }

    private void SnapToTargetOffsets()
    {
        _targetPositionOffset = _isInBattle ? _battlePositionOffset : _normalPositionOffset;
        _targetLookOffset = _isInBattle ? _battleLookOffset : _normalLookOffset;

        _currentPositionOffset = _targetPositionOffset;
        _currentLookOffset = _targetLookOffset;

        if (_target != null)
        {
            transform.position = _target.position + _currentPositionOffset;
            transform.LookAt(_target.position + _currentLookOffset);
        }
    }

    public void SetNormalOffsets(Vector3 posOffset, Vector3 lookOffset)
    {
        _normalPositionOffset = posOffset;
        _normalLookOffset = lookOffset;

        if (!_isInBattle)
            _targetPositionOffset = _normalPositionOffset;
    }

    public void SetBattleOffsets(Vector3 posOffset, Vector3 lookOffset)
    {
        _battlePositionOffset = posOffset;
        _battleLookOffset = lookOffset;

        if (_isInBattle)
            _targetPositionOffset = _battlePositionOffset;
    }
}
