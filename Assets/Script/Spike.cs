/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Spike : Entity, IInteractable
{
#region Fields
	[ BoxGroup( "Setup" ) ] public MeshRenderer aliveMesh;
	[ BoxGroup( "Setup" ) ] public GameObject deadMesh;
	[ BoxGroup( "Setup" ) ] public bool startDead;
	[ BoxGroup( "Setup" ), HideIf( "startDead" ) ] public int inital_health;
#endregion

#region Properties
#endregion

#region Unity API
	private void Awake()
	{
		ui_deposit = GetComponentInChildren< UIWorld_Deposit >();
	}

	private void Start()
	{
		health_max = GameSettings.Instance.spike_maxHealth;

		if( startDead )
			Die();
		else
		{
			health = inital_health;
			Revive();
		}

		ui_deposit.Init( health, health_max );
	}
#endregion

#region API
    public Collider GiveHealthCollider()
    {
		return colliderListener_Health_Enter.AttachedCollider;
	}

    public Transform GiveDepositOrigin()
    {
		return origin_deposit;
	}

    public void GetDeposit( int count, DepositType type, Collectable collectable = null )     
    {
		var deposit_value    = ( int )type + 1;
		    health           = Mathf.Min( health + count * ( deposit_value ), GameSettings.Instance.spike_maxHealth );
		    incomingDeposit -= deposit_value;

		UpdateHealthRatio();

		if( !isAlive )
		{
			Revive();
			particle_spawn.Raise( "explosion_green", origin_deposit.position );
		}
	}

    public void GetDamage( int count )
    {
		//TODO(OFG): spawn damage particle effect
		health = Mathf.Max( health - 1, 0 );

		UpdateHealthRatio();

		if( health <= 0 )
		{
			Die();
			particle_spawn.Raise( "explosion_red", origin_deposit.position );
		}
	}

	public void UpdateHealthRatio()
	{
		ui_deposit.SetValue( health );
	}

	public bool IsAlive()
    {
        return false; // Since we don't want enemies to attack spike
    }

    public int CanDeposit()
    {
		return GameSettings.Instance.turret_maxHealth - health - incomingDeposit;
	}

    public void IncomingDeposit( int count )
    {
		incomingDeposit += count;
	}

	public void Subscribe_OnDeath( UnityMessage onDeathDelegate )
    {
        //! DO onDeath = null for clearing the invoke list
		onDeath += onDeathDelegate;
	}

	public void UnSubscribe_OnDeath( UnityMessage onDeathDelegate )
	{
		onDeath -= onDeathDelegate;
	}
#endregion

#region Implementation
    [ Button() ]
    protected override void Die()
    {
		colliderListener_Health_Enter.SetColliderActive( false );
		colliderListener_Health_Enter.ClearEventList();
		isAlive = false;
		health  = 0;


		aliveMesh.enabled = false;
		deadMesh.SetActive( true );

		InvokeOnDeath();
		ClearOnDeath();
	}

    [ Button() ]
    protected override void Revive()
    {
        // Enable healt collider since it can dake damage
		colliderListener_Health_Enter.SetColliderActive( true );
		colliderListener_Health_Enter.triggerEvent += DamageEnemy;
		isAlive = true;


		aliveMesh.enabled = true;
		deadMesh.SetActive( false );
	}

	private void DamageEnemy( Collider other )
	{
		//Enemy will kill itself after colliding with spike
		if( other.gameObject.layer != 28 )
		{
			GetDamage( 1 );
			particle_spawn.Raise( "enemy_damage", origin_deposit.position );
		}
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
