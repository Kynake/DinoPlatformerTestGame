using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnDirection
{
    Left = -1,
    Right = 1
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyPool _enemyPool = null;

    public SpawnDirection SpawnDirection = SpawnDirection.Left;

    [Min(0.1f)]
    public float SpawnInterval = 2;

    [Min(1)]
    public int MaxConcurrentSpawns = 3;

    [Min(0)]
    public float SpawnedEnemySpeed = 3;

    private int _concurrentSpawns = 0;
    private float _currentSpawnInterval;
    private Coroutine _spawnCoroutine;
    private WaitForSeconds _coroutineWait;

    private void OnEnable()
    {
        _currentSpawnInterval = SpawnInterval;
        _coroutineWait = new WaitForSeconds(SpawnInterval);
        _spawnCoroutine = StartCoroutine(SpawnWithInterval());
    }

    private void OnDisable()
    {
        StopCoroutine(_spawnCoroutine);
    }

    private IEnumerator SpawnWithInterval()
    {
        while(true)
        {
            if(_currentSpawnInterval != SpawnInterval)
            {
                _currentSpawnInterval = SpawnInterval;
                _coroutineWait = new WaitForSeconds(_currentSpawnInterval);
            }

            if(_concurrentSpawns < MaxConcurrentSpawns && _enemyPool != null)
            {
                var enemy = _enemyPool.TakeEnemyFromPool();

                _concurrentSpawns++;
                enemy.OnDeath += DecreaseConcurrentCounter;

                enemy.transform.position = transform.position;
                enemy.transform.localScale = new Vector3((int) SpawnDirection, 1, 1);
                enemy.Speed = SpawnedEnemySpeed;

                enemy.gameObject.SetActive(true);
            }

            yield return _coroutineWait;
        }
    }

    private void DecreaseConcurrentCounter() => _concurrentSpawns--;
}
