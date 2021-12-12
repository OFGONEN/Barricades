/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class Table : Entity, IInteractable
{
#region Fields
#endregion

#region Properties
#endregion

#region Unity API
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

		if( !isAlive )
			Revive();
	}

    public void GetDamage( int count )
    {
		//TODO(OFG): spawn damage particle effect
		health = Mathf.Max( health - 1, 0 );

		if( health <= 0 )
			Die();
	}

	public bool IsAlive()
    {
        return false; // Since we don't want enemies to attack spike
    }

    public int CanDeposit()
    {
		return GameSettings.Instance.turret_maxHealth - health;
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
    protected override void Die()
    {

    }

    protected override void Revive()
    {

    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
