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
    }
}
