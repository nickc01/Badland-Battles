using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //A list of all the spawners in the game
    static HashSet<EnemySpawner> _spawners = new HashSet<EnemySpawner>();
    /// <summary>
    /// A list of all the spawners in the game
    /// </summary>
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


    //Called when gizmos are about to be drawn
    private void OnDrawGizmos()
    {
        //The height of the gizmo cube
        float height = 2f;

        //The color of the gizmo cube
        Gizmos.color = new Color(0,1,1,0.25f);

        //Draw a cube where the enemy spawner is located
        Gizmos.DrawCube(transform.position + new Vector3(0f, height / 2f, 0f),new Vector3(1,height,1));
    }
}
