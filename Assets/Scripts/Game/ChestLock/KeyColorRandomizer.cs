using UnityEngine;
using System.Collections.Generic;
using Zenject;

[System.Serializable]
public struct ChestKeyPair
{
    public Sprite lockSprite;
    public Sprite keySprite;
}

public class KeyColorRandomizer : MonoBehaviour
{
    [SerializeField] private ChestLock _lock;
    [SerializeField] private List<KeyDrag> _keyDragComponents;
    [SerializeField] private ChestKeyPair[] _possiblePairs;

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
            Debug.LogError("There are not enough keys to generate 3 correct ones");
            return;
        }

        ChestKeyPair pair = _possiblePairs[Random.Range(0, _possiblePairs.Length)];

        _lock.SetRandomSprite(pair.lockSprite);
        _lock.SetAcceptableKeys(new Sprite[] { pair.keySprite });

        List<int> indices = new List<int>();
        for (int i = 0; i < _keyDragComponents.Count; i++) indices.Add(i);

        for (int i = 0; i < 3; i++)
        {
            int randIndex = Random.Range(0, indices.Count);
            _keyDragComponents[indices[randIndex]].keyImage.sprite = pair.keySprite;
            indices.RemoveAt(randIndex);
        }

        foreach (int idx in indices)
        {
            ChestKeyPair randomPair;
            do
            {
                randomPair = _possiblePairs[Random.Range(0, _possiblePairs.Length)];
            } while (randomPair.lockSprite == pair.lockSprite);

            _keyDragComponents[idx].keyImage.sprite = randomPair.keySprite;
        }
    }
}
