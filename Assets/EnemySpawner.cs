using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    static HashSet<EnemySpawner> _spawners = new HashSet<EnemySpawner>();
    public static IEnumerable<EnemySpawner> Spawners => _spawners;

    private void Awake()
    {
        if (enabled)
        {
            _spawners.Add(this);
        }
    }

    private void OnEnable()
    {
        _spawners.Add(this);
    }

    private void OnDisable()
    {
        _spawners.Remove(this);
    }

    private void OnDestroy()
    {
        _spawners.Remove(this);
    }



    private void OnDrawGizmos()
    {
        float height = 2f;

        Gizmos.color = new Color(0,1,1,0.25f);

        Gizmos.DrawCube(transform.position + new Vector3(0f, height / 2f, 0f),new Vector3(1,height,1));
    }
}
