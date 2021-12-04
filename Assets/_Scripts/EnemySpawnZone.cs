using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnZone : MonoBehaviour
{
    // each spawn zone has an area things can spawn in, as well as a path for the enemies to get from
    // here to the defense point.

    [SerializeField] Vector2 dimensions;
    [SerializeField] Transform[] path;

    public GameObject spawnEnemyRandomly(GameObject prefab)
    {
        float x = Random.Range(0, dimensions.x);
        float z = Random.Range(0, dimensions.y);

        GameObject newGO = Instantiate(prefab, transform.position + new Vector3(x, 0, z), Quaternion.identity);

        // set path if Pathable
        IPathable p = newGO.GetComponent<IPathable>();
        if (p != null)
        {
            p.SetPath(path);
        }

        return newGO;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(transform.position + new Vector3(dimensions.x / 2, 0.5f, dimensions.y / 2),
            new Vector3(dimensions.x, 1, dimensions.y));
    }
}
