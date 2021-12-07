/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;
using UnityEditor;

public class Test_Bullet : MonoBehaviour
{
#region Fields
    public BulletPool bulletPool;

	public float cooldown;

	// Delegates \\
	private UnityMessage updateMethod;
    private float time;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		StopShooting();
	}

    private void Update()
    {
		updateMethod();
	}
#endregion

#region API
    [ Button() ]
    public void SpawnBullet()
    {
        if( time > Time.time ) return;

		time = cooldown + Time.time;
		var bullet = bulletPool.GiveEntity();
		bullet.Spawn( transform.position, transform.forward );
	}

    [ Button() ]
    public void StartShooting()
    {
		updateMethod = SpawnBullet;
	}

    [ Button() ]
    public void StopShooting()
    {
		updateMethod = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
		Handles.ArrowHandleCap( GetInstanceID(), transform.position, transform.rotation, 10, EventType.Ignore );
	}
#endif
#endregion
}
