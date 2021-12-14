/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Table : Entity, IInteractable
{
#region Fields
    [ BoxGroup( "Setup" ) ] public Transform[] origin_deposit_array;
    [ BoxGroup( "Setup" ) ] public CollectablePool[] collectable_pool_array;

#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public Collider GiveHealthCollider()
    {
		return null;
	}

    public Transform GiveDepositOrigin()
    {
		return origin_deposit;
	}

    public void GetDeposit( int count, DepositType type, Collectable collectable = null )     
    {
		//TODO(OFG): spawn deposited particle effect

		var origin_deposit_random = origin_deposit_array[ Random.Range( 0, origin_deposit_array.Length ) ];
		var pool = collectable_pool_array[ Mathf.Min( collectable_pool_array.Length - 1, (int)type + 1 ) ];

		var collectable_upgraded = pool.GiveEntity();
		var deposit_position     = origin_deposit_random.position + Random.insideUnitCircle.ConvertV3() * GameSettings.Instance.collectable_random_deposit;

		collectable_upgraded.transform.position = origin_deposit.position;
		collectable_upgraded.DepositToGround( deposit_position, Random.Range( 0, 360 ) );
	}

    public void GetDamage( int count )
    {
	}

	public bool IsAlive()
    {
        return false; // Since we don't want enemies to attack spike
    }

    public int CanDeposit()
    {
		return GameSettings.Instance.player_max_collectable;
	}

    public void IncomingDeposit()
    {

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
