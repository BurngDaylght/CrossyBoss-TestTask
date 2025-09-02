using System.Collections.Generic;
using UnityEngine;

public class CustomPool<T> where T : MonoBehaviour
{
    private T _prefab;
    private Stack<T> _objects;
    private Transform _parent;
    private int _maxSize;

    public CustomPool(T prefab, int prewarmObjects, Transform parent = null, int maxSize = 50)
    {
        _objects = new Stack<T>();
        _prefab = prefab;
        _parent = parent;
        _maxSize = maxSize;

        for (int i = 0; i < prewarmObjects; i++)
        {
            var obj = Create();
            obj.gameObject.SetActive(false);
            _objects.Push(obj);
        }
    }
    
    public T Get()
    {
        T obj;
        
        if (_objects.Count > 0)
        {
            obj = _objects.Pop();
        }
        else
        {
            obj = Create();
        }

        obj.gameObject.SetActive(true);
        
        return obj;
    }
    
    public void Release(T obj)
    {
        if (_objects.Count >= _maxSize)
        {
            GameObject.Destroy(obj.gameObject);
        }
        else
        {
            obj.gameObject.SetActive(false);
            _objects.Push(obj);
        }
    }
    
    private T Create()
    {
        return GameObject.Instantiate(_prefab, _parent);
    }
}
