using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject PooledObject = null;
    public int PreloadAmount = 5;

    protected Stack<GameObject> _objectPool;

    protected virtual void Awake()
    {
        EnsurePool();
    }

    protected virtual void EnsurePool()
    {
        if(_objectPool != null)
        {
            return;
        }

        if(PooledObject != null && PreloadAmount > 0)
        {
            _objectPool = new Stack<GameObject>(PreloadAmount);
            for(int i = 0; i < PreloadAmount; i++)
            {
                InstantiateNewObjectInPool();
            }
        }
        else
        {
            _objectPool = new Stack<GameObject>();
        }
    }

    protected virtual GameObject InstantiateNewObject()
    {
        if(PooledObject != null)
        {
            var item = Instantiate(PooledObject, transform.position, Quaternion.identity);
            if(item != null)
            {
                item.SetActive(false);
                return item;
            }
        }

        return null;
    }

    protected virtual void InstantiateNewObjectInPool() {
        var obj = InstantiateNewObject();
        if(obj != null)
        {
            _objectPool.Push(obj);
        }
    }

    public virtual GameObject TakeObjectFromPool()
    {
        EnsurePool();

        if(_objectPool.Count == 0)
        {
            return InstantiateNewObject();
        }

        return _objectPool.Pop();
    }

    public virtual void ReturnObjectToPool(GameObject obj)
    {
        EnsurePool();

        obj.SetActive(false);
        obj.transform.position = transform.position;
        _objectPool.Push(obj);
    }
}
