/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Guard : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared Variables" ) ] public BulletPool bulletPool;

    [ BoxGroup( "Setup" ) ] public ColliderListener_Stay_EventRaiser[] colliderListener_Stay_Array;
    [ BoxGroup( "Setup" ) ] public ColliderListener_Exit_EventRaiser[] colliderListener_Exit_Array;
    [ BoxGroup( "Setup" ) ] public Animator animator;
    [ BoxGroup( "Setup" ) ] public Transform origin_shoot;

	// Private \\
	private Enemy current_target;
	private float current_target_lastShoot;

	private float current_look_weight;
	private Vector3 current_look_position;

	// Delegate
	private UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
        for( var i = 0; i < colliderListener_Stay_Array.Length; i++ )
        {
			colliderListener_Stay_Array[ i ].triggerEvent += OnEntity_Stay;
		}

        for( var i = 0; i < colliderListener_Exit_Array.Length; i++ )
        {
			colliderListener_Exit_Array[ i ].triggerEvent += OnEntity_Exit;
		}
    }

    private void OnDisable()
    {
        for( var i = 0; i < colliderListener_Exit_Array.Length; i++ )
        {
			colliderListener_Exit_Array[ i ].triggerEvent -= OnEntity_Exit;
		}

        for( var i = 0; i < colliderListener_Exit_Array.Length; i++ )
        {
			colliderListener_Exit_Array[ i ].triggerEvent -= OnEntity_Exit;
		}
    }

    private void Awake()
    {
		updateMethod = ExtensionMethods.EmptyMethod;
		current_look_weight = 0;
	}

    private void Update()
    {
		updateMethod();
	}


	private void OnAnimatorIK( int layerIndex )
	{
		animator.SetLookAtPosition( current_look_position );
		animator.SetLookAtWeight( current_look_weight, 1f, 1f, 1f );
	}
#endregion

#region API
#endregion

#region Implementation
    private void OnEntity_Stay( Collider other )
    {
		var enemy = other.GetComponent< ColliderListener >().AttachedComponent as Enemy;

		// Has current target and it is alive
		if( ( current_target != null && current_target.IsAlive ) || enemy == null ) return;

		current_target = enemy;
		updateMethod   = OnUpdate_Shoot;
		ShootAtCurrentTarget();
	}

	private void OnEntity_Exit( Collider other )
	{
		var enemy = other.GetComponent< ColliderListener >().AttachedComponent as Enemy;

		if( current_target == enemy )
			StopShooting();
	}

    private void OnUpdate_Shoot()
    {
		if( current_target.IsAlive && !current_target.IsInside )
			ShootAtCurrentTarget();
		else
			StopShooting();
	}

	private void ShootAtCurrentTarget()
	{
		animator.SetBool( "fire", true );

		current_look_position = current_target.transform.position;
		transform.LookAtAxis( current_look_position, Vector3.up );

		if( current_target_lastShoot < Time.time )
		{
			current_target_lastShoot = Time.time + GameSettings.Instance.guard_shoot_rate;
			Shoot();
		}
	}

	private void StopShooting()
	{
		current_target = null;
		updateMethod = ExtensionMethods.EmptyMethod;
		animator.SetBool( "fire", false );
	}

	private void Shoot()
	{
		var bullet = bulletPool.GiveEntity();

		var origin_shoot_position = origin_shoot.position;
		var direction = current_target.GiveShootPosition() - origin_shoot_position;

		bullet.Spawn( origin_shoot_position, direction.normalized, GameSettings.Instance.guard_bullet_speed );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
