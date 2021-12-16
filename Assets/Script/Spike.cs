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
#endregion

#region Properties
#endregion

#region Unity API
	private void Start()
	{
		if( startDead )
			Die();
		else
		{
			health = GameSettings.Instance.spike_maxHealth;
			Revive();
		}

		UpdateHealthRatio();
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
		//TODO(OFG): spawn deposited particle effect
		health = Mathf.Min( health + count * ( ( int )type + 1 ), GameSettings.Instance.spike_maxHealth );
		incomingDeposit--;

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
		health_ratio = health / ( float ) GameSettings.Instance.turret_maxHealth;
		health_ratio_image.fillAmount = health_ratio;
	}

	public bool IsAlive()
    {
        return false; // Since we don't want enemies to attack spike
    }

    public int CanDeposit()
    {
		return GameSettings.Instance.turret_maxHealth - health - incomingDeposit;
	}

    public void IncomingDeposit()
    {
		incomingDeposit++;
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
			GetDamage( 1 );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
