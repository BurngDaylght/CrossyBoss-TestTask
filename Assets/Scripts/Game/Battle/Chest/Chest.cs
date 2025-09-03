using System;
using UnityEngine;
using Zenject;

public class Chest : MonoBehaviour
{
    public event Action OnChestInteracted;
    
    private Animator _animator;
    private LevelLogic _levelLogic;
    
    [Inject]
    private void Construct(LevelLogic levelLogic)
    {
        _levelLogic = levelLogic;
    }
    
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        _levelLogic.OnLevelComplete += PlayOpenAnimation;
    }

    private void OnDisable()
    {
        _levelLogic.OnLevelComplete -= PlayOpenAnimation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerStats>(out _))
        {
            OnChestInteracted?.Invoke();
        }
    }
    
    private void PlayOpenAnimation()
    {
        _animator.SetBool("Opened", true);
    }
}
