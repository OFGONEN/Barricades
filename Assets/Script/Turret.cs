/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Turret : Entity, IInteractable
{
#region Fields
    [ BoxGroup( "Setup" ) ] public GameObject aliveMesh;
	[ BoxGroup( "Setup" ) ] public GameObject deadMesh;
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
			health = GameSettings.Instance.turret_maxHealth;
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
    [ Button() ]
    protected override void Die()
    {
		colliderListener_Health_Enter.SetColliderActive( false );
		colliderListener_Seek_Stay.SetColliderActive( false );
		isAlive = false;
		health  = 0;

		aliveMesh.SetActive( false );
		deadMesh.SetActive( true );

		InvokeOnDeath();
		ClearOnDeath();
	}

    [ Button() ]
    protected override void Revive()
    {
        // Enable healt collider since it can dake damage
		colliderListener_Health_Enter.SetColliderActive( true );
		colliderListener_Seek_Stay.SetColliderActive( true );
		isAlive = true;

		aliveMesh.SetActive( true );
		deadMesh.SetActive( false );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
