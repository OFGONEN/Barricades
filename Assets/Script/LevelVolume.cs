/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

public class LevelVolume : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public Vector3 volume_position_local;
    [ BoxGroup( "Setup" ) ] public Vector3 volume_size;
    
    private Vector3 volume_position_world;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		volume_position_world = transform.TransformPoint( volume_position_local );
	}
#endregion

#region API
    public Vector3 GiveRandomPosition()
    {
		var position_min = volume_position_world - volume_size / 2;
		var position_max = volume_position_world + volume_size / 2;

		var random_x = Random.Range( position_min.x, position_max.x );
		var random_z = Random.Range( position_min.z, position_max.z );

		return new Vector3( random_x, 0, random_z );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    private Vector3 position_random;
    private void OnDrawGizmosSelected()
    {
		Handles.color = Color.red;
		Handles.DrawWireCube( transform.TransformPoint( volume_position_local ), volume_size);
		Handles.color = Color.yellow;
		Handles.DrawWireCube( position_random, Vector3.one * 0.5f );

		Handles.color = Color.blue;
		Handles.DrawWireCube( transform.GetChild( 0 ).position, Vector3.one * 0.5f );
	}

    [ Button() ]
    private void RandomPosition()
    {
		volume_position_world = transform.TransformPoint( volume_position_local );
		position_random = GiveRandomPosition();
	}
#endif
#endregion
}
