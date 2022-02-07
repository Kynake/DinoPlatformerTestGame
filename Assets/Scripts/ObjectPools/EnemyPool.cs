using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool
{
    protected override GameObject InstantiateNewObject()
    {
        var obj = base.InstantiateNewObject();
        if(obj != null)
        {
            if(obj.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.OnDeath = () => ReturnObjectToPool(obj);
                return obj;
            }

            Destroy(obj);
        }

        return null;
    }

    public virtual Enemy TakeEnemyFromPool()
    {
        var obj = TakeObjectFromPool();
        if(obj != null)
        {
            if(obj.TryGetComponent<Enemy>(out var res))
            {
                res.ResetAnimation();
                return res;
            }

            Destroy(obj);
        }

        return null;
    }

    public virtual void ReturnEnemyToPool(Enemy enemy)
    {
        enemy.ResetAnimation();
        ReturnObjectToPool(enemy.gameObject);
    }
}
