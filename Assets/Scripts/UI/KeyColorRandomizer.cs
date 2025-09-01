using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Zenject;

public class KeyColorRandomizer : MonoBehaviour
{
    [SerializeField] private ChestLock _lock;
    [SerializeField] private List<KeyDrag> _keyDragComponents;
    [SerializeField] private Color[] _possibleColors;

    private Chest _chest;
    
    [Inject]
    private void Construct(Chest chest)
    {
        _chest = chest;
    }

    private void OnEnable()
    {
        _chest.OnChestInteracted += RandomizeKeys;
    }
    
    private void OnDisable()
    {
        _chest.OnChestInteracted -= RandomizeKeys;
    }

    public void RandomizeKeys()
    {
        if (_keyDragComponents.Count < 3) 
        {
            Debug.LogError("Недостаточно ключей для генерации 3 правильных");
            return;
        }

        Color lockColor = _possibleColors[Random.Range(0, _possibleColors.Length)];
        lockColor.a = 1f;
        _lock.SetRandomColor(lockColor);

        List<int> indices = new List<int>();
        for (int i = 0; i < _keyDragComponents.Count; i++) indices.Add(i);

        for (int i = 0; i < 3; i++)
        {
            int randIndex = Random.Range(0, indices.Count);
            Color keyColor = lockColor;
            keyColor.a = 1f;
            _keyDragComponents[indices[randIndex]].keyImage.color = keyColor;
            indices.RemoveAt(randIndex);
        }

        foreach (int idx in indices)
        {
            Color randomColor;
            do
            {
                randomColor = _possibleColors[Random.Range(0, _possibleColors.Length)];
                randomColor.a = 1f;
            } while (randomColor == lockColor);

            _keyDragComponents[idx].keyImage.color = randomColor;
        }
    }
}
