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
	[ BoxGroup( "Setup" ) ] public MeshRenderer deadMesh;
	[ BoxGroup( "Setup" ) ] public bool startDead;
#endregion

#region Properties
#endregion

#region Unity API
	private void Awake()
	{
		if( startDead )
			Die();
		else
		{
			health = GameSettings.Instance.spike_maxHealth;
			Revive();
		}
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
		return health < GameSettings.Instance.spike_maxHealth;
	}

	public void Subscribe_OnDeath( UnityMessage onDeathDelegate )
    {
        //! DO onDeath = null for clearing the invoke list
		onDeath += onDeathDelegate;
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
		deadMesh.enabled  = true;

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
		deadMesh.enabled  = false;
	}

	private void DamageEnemy( Collider other )
	{
		//Enemy will kill itself after colliding with spike
		GetDamage( 1 );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
