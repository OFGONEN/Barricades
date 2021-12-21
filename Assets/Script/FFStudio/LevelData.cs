/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using NaughtyAttributes;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "LevelData", menuName = "FF/Data/LevelData" ) ]
	public class LevelData : ScriptableObject
    {
		[ BoxGroup( "Setup" ), Scene() ] public int sceneIndex;
        [ BoxGroup( "Setup" ) ] public bool overrideAsActiveScene;


        [ BoxGroup( "Level Data" ), Tooltip( "Spawn rate of collectable" ) ] public float collectable_spawn_rate;
        [ BoxGroup( "Level Data" ), Tooltip( "Max spawn count of collectable" ) ] public int collectable_spawn_max;

        [ BoxGroup( "Level Data" ), Tooltip( "Spawn Data" ) ] public SpawnPointData[] spawn_point_data_array;

        public int GetSpawnCountdown()
        {
			float countdown = float.MaxValue;

            for( var i = 0; i < spawn_point_data_array.Length; i++ )
            {
				var data_array = spawn_point_data_array[ i ];

                for( var a = 0; a < data_array.spawn_data_array.Length; a++ )
                {
					var spawn_data = data_array.spawn_data_array[ a ];

					countdown = Mathf.Min( countdown, spawn_data.spawn_time );
				}
			}

			return Mathf.RoundToInt( countdown * 60f);
		}
    }
}
