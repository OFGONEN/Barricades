/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;

public class SpawnPoint : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared Variables" ) ] public SpawnPointSet spawn_point_set;
    [ BoxGroup( "Shared Variables" ) ] public EnemyPool enemyPool;
    [ BoxGroup( "Shared Variables" ) ] public SharedFloat shared_TotalEnemyCount;
    [ BoxGroup( "Setup" ) ] public int spawn_point_index;

	private SpawnPointData spawn_point_data;
	private Tween spawn_tween;
	private int spawn_tween_index = 0;
	private SpawnData spawn_tween_data;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		spawn_point_set.AddDictionary( spawn_point_index, this );
	}

    private void OnDisable()
    {
		spawn_point_set.RemoveDictionary( spawn_point_index );

		spawn_tween = spawn_tween.KillProper();
	}

	private void Awake()
	{
#if UNITY_EDITOR
		try
		{
			spawn_point_data = CurrentLevelData.Instance.levelData.spawn_point_data_array[ spawn_point_index ];
		}
		catch ( System.Exception )
		{
			FFLogger.LogError( "Level data spawn point array doesn't have Element at INDEX: " + spawn_point_index, CurrentLevelData.Instance.levelData );
		}
#else
		spawn_point_data = CurrentLevelData.Instance.levelData.spawn_point_data_array[ spawn_point_index ];
#endif
		int enemyCount = 0;
		for( var i = 0; i < spawn_point_data.spawn_data_array.Length; i++ )
		{
			enemyCount += spawn_point_data.spawn_data_array[ i ].spawn_count;
		}

		shared_TotalEnemyCount.sharedValue += enemyCount;

		StartSpawnTween( 0 );
	}
#endregion

#region API
#endregion

#region Implementation
	private void ExecuteSpawnData()
	{
        for( var i = 0; i < spawn_tween_data.spawn_count; i++ )
        {
			var enemy = enemyPool.GiveEntity();
			enemy.Spawn( transform.position );
		}

		if( spawn_tween_index < spawn_point_data.spawn_data_array.Length - 1 )
			StartSpawnTween( spawn_tween_index + 1 );
		else
			spawn_tween = spawn_tween.KillProper();
	}

	private void StartSpawnTween( int index)
	{
		spawn_tween_index = index;
		spawn_tween_data  = spawn_point_data.spawn_data_array[ spawn_tween_index ];
		spawn_tween       = DOVirtual.DelayedCall( spawn_tween_data.spawn_time * 60, ExecuteSpawnData );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
