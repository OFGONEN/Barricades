/* Created by and for usage of FF Studios (2021). */

using System;
using UnityEngine;

namespace FFStudio
{
	[ Serializable ]
	public struct TransformData
	{
		public Vector3 position;
		public Vector3 rotation; // Euler angles.
		public Vector3 scale; // Local scale.
	}

	[ Serializable ]
	public struct SpawnData
	{
		[ Tooltip( "Time in minutes" ) ] public float spawn_time;
		public int spawn_count;
	}

	[ Serializable ]
	public struct SpawnPointData
	{
		public SpawnData[] spawn_data_array;
	}
}
