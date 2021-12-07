/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Turret : Entity, IInteractable
{
#region Fields
    [ BoxGroup( "Shared Variables" ) ] public EnemySet enemySet;
    [ BoxGroup( "Shared Variables" ) ] public BulletPool bulletPool;

    [ BoxGroup( "Setup" ) ] public GameObject aliveMesh;
	[ BoxGroup( "Setup" ) ] public GameObject deadMesh;
	[ BoxGroup( "Setup" ) ] public Transform rotateObject;
	[ BoxGroup( "Setup" ) ] public Transform shootOrigin;
	[ BoxGroup( "Setup" ) ] public bool startDead;


	// Private \\
	private float lastShoot;
	private UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		updateMethod = ExtensionMethods.EmptyMethod;

		if( startDead )
			Die();
        else
        {
			health = GameSettings.Instance.turret_maxHealth;
			Revive();
		}
	}

	private void Update()
	{
		updateMethod();
	}
#endregion

#region API
	public Collider GiveHealthCollider()
    {
		return colliderListener_Health_Enter.AttachedCollider;
	}

	public Vector3 GiveDepositPoint()
    {
		return Vector3.zero;
	}

    public void GetDeposit( int count, DepositType type )     
    {
		//TODO(OFG): spawn deposited particle effect
		health = Mathf.Min( health + count * ( ( int )type + 1 ), GameSettings.Instance.spike_maxHealth );

		if( !isAlive )
			Revive();
	}

    public void GetDamage( int count )
    {
		//TODO(OFG): spawn damage particle effect
		health -= count;
		bool isDead = health <= 0;

		if( isDead )
			Die();
	}

	public bool IsAlive()
    {
		return isAlive;
	}

	public bool CanDeposit()
    {
		return health < GameSettings.Instance.turret_maxHealth;
	}

	public void Subscribe_OnDeath( UnityMessage onDeathDelegate )
    {
		onDeath += onDeathDelegate;
	}

	public void UnSubscribe_OnDeath( UnityMessage onDeathDelegate )
	{
		onDeath -= onDeathDelegate;
	}
#endregion

#region Implementation
	private void SeekAndShoot()
	{
		var   position = transform.position;
		float distance = float.MaxValue;

		Enemy closestEnemy = null;
		Vector3 enemyPosition = Vector3.zero;

		foreach( var enemy in enemySet.itemDictionary.Values )
		{
			    enemyPosition   = enemy.transform.position;
			var distanceToEnemy = Vector3.Distance( enemyPosition, position );

			if( distanceToEnemy < distance )
			{
				distance     = distanceToEnemy;
				closestEnemy = enemy;
			}
		}

		if( closestEnemy != null )
		{
			rotateObject.LookAtAxis( enemyPosition, Vector3.up );
			Shoot( enemyPosition );
		}
	}

	private void Shoot( Vector3 targetPosition )
	{
		if( lastShoot > Time.time ) return; // Return if Cooldown is not met

		lastShoot = Time.time + GameSettings.Instance.turret_fireRate; // Add cooldown

		var shootOriginPosition = shootOrigin.position;
		var direction           = ( targetPosition - shootOriginPosition );
		var bullet              = bulletPool.GiveEntity();

		bullet.Spawn( shootOriginPosition, direction );
	}

    [ Button() ]
    protected override void Die()
    {
		colliderListener_Health_Enter.SetColliderActive( false );
		colliderListener_Seek_Stay.SetColliderActive( false );
		isAlive = false;
		health  = 0;

		updateMethod = ExtensionMethods.EmptyMethod;

		aliveMesh.SetActive( false );
		deadMesh.SetActive( true );

		InvokeOnDeath();
		ClearOnDeath();
	}

    [ Button() ]
    protected override void Revive()
    {
        // Enable healt collider since it can take damage
		colliderListener_Health_Enter.SetColliderActive( true );
		colliderListener_Seek_Stay.SetColliderActive( true );
		isAlive = true;

		updateMethod = SeekAndShoot;

		aliveMesh.SetActive( true );
		deadMesh.SetActive( false );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
