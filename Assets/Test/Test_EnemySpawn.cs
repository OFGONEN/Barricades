/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Test_EnemySpawn : MonoBehaviour
{
#region Fields
    public EnemyPool enemyPool;
    public Transform spawnPosition;

    public int spawnCount;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void Spawn()
    {
        for( var i = 0; i < spawnCount; i++ )
        {
			var enemy = enemyPool.GiveEntity();
			enemy.Spawn( spawnPosition.position );
		}
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}