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

    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser[] colliderListener_Enter_array;
    [ BoxGroup( "Setup" ) ] public ColliderListener_Exit_EventRaiser[] colliderListener_Exit_array;
    [ BoxGroup( "Setup" ) ] public Animator animator;
    [ BoxGroup( "Setup" ) ] public Transform origin_shoot;

	// Private \\
	private Dictionary<int, Enemy> enemy_dictionary = new Dictionary<int, Enemy>( 64 );
	private Enemy current_target;
	private int current_target_instanceID;
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
        for( var i = 0; i < colliderListener_Enter_array.Length; i++ )
        {
			colliderListener_Enter_array[ i ].triggerEvent += OnEntityEnter;
		}

        for( var i = 0; i < colliderListener_Exit_array.Length; i++ )
        {
			colliderListener_Exit_array[ i ].triggerEvent += OnEntityExit;
        }
    }

    private void OnDisable()
    {
        for( var i = 0; i < colliderListener_Enter_array.Length; i++ )
        {
			colliderListener_Enter_array[ i ].triggerEvent -= OnEntityEnter;
		}

        for( var i = 0; i < colliderListener_Exit_array.Length; i++ )
        {
			colliderListener_Exit_array[ i ].triggerEvent -= OnEntityExit;
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
    private void OnEntityEnter( Collider other )
    {
		var enemy = other.GetComponent< ColliderListener >().AttachedComponent as Enemy;
		var enemy_instanceID = enemy.GetInstanceID();
		var enemy_is_new = !enemy_dictionary.ContainsKey( enemy_instanceID );

		if( enemy_is_new )
		{
			enemy_dictionary.Add( enemy_instanceID, enemy );
			SetShootTarget( enemy, enemy_instanceID );
		}
	}

    private void OnEntityExit( Collider other )
    {
		var enemy = other.GetComponent< ColliderListener >().AttachedComponent as Enemy;
		var enemy_instanceID = enemy.GetInstanceID();
		var enemy_is_shootTarget = enemy_instanceID == current_target_instanceID;

		enemy_dictionary.Remove( enemy_instanceID );

		if( enemy_is_shootTarget )
		{
			current_target.onDeath -= OnEnemyDeath;
			current_target = null;

			SetShootTarget_Random();
		}
	}

	private void SetShootTarget( Enemy target, int instanceID )
	{
		if( current_target == null )
		{
			current_target = target;
			current_target_instanceID = instanceID;

			current_target.onDeath += OnEnemyDeath;
		}

		updateMethod = OnUpdate_Shoot;
		current_look_weight = 1f;
		animator.SetBool( "fire", true );
	}

	private void OnEnemyDeath()
	{
		enemy_dictionary.Remove( current_target_instanceID );

		current_target.onDeath -= OnEnemyDeath;
		current_target = null;

		SetShootTarget_Random();
	}

	private Enemy GiveRandomEnemy()
	{
		Enemy enemy = null;

		int index_random = Random.Range( 0, enemy_dictionary.Keys.Count );
		int counter      = 0;

		foreach( var pair in enemy_dictionary )
		{
			if( counter == index_random )
			{
				enemy = pair.Value;
				break;
			}
		}

		return enemy;
	}

	private void SetShootTarget_Random()
	{
		var enemy_random = GiveRandomEnemy();

		if( enemy_random == null )
        {
			animator.SetBool( "fire", false );
			updateMethod = ExtensionMethods.EmptyMethod;
			current_look_weight = 0;
		}
		else
			SetShootTarget( enemy_random, enemy_random.GetInstanceID() );
    }

    private void OnUpdate_Shoot()
    {
		current_look_position = current_target.transform.position;
		transform.LookAtAxis( current_look_position, Vector3.up );

		if( current_target_lastShoot < Time.time )
		{
			current_target_lastShoot = Time.time + GameSettings.Instance.guard_shoot_rate;
			Shoot();
		}
	}

	private void Shoot()
	{
		var bullet = bulletPool.GiveEntity();

		var origin_shoot_position = origin_shoot.position;
		var direction = current_look_position + Vector3.up * GameSettings.Instance.guard_shoot_height - origin_shoot_position;

		bullet.Spawn( origin_shoot_position, direction.normalized );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
