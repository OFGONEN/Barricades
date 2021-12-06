/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using FFStudio;
using NaughtyAttributes;

public class Window : MonoBehaviour, IInteractable
{
#region Fields
    [ BoxGroup( "Setup" ) ] public NavMeshLink navMeshLink;
    [ BoxGroup( "Setup" ) ] public BoxCollider colliderHealth;
    [ BoxGroup( "Setup" ) ] public BoxCollider colliderSeek;
    [ BoxGroup( "Setup" ) ] public ColliderListener_Stay_EventRaiser colliderListener_Seek_Stay;

    // Private Fields \\
    private bool isAlive = true;
	private float lastVaultTime;

	// Delegates \\
	private event UnityMessage onDeath;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnDisable()
    {
		colliderListener_Seek_Stay.ClearEventList();
	}
#endregion

#region API
    public Collider GiveHealthCollider()
    {
		return colliderHealth;
	}

    public Vector3 GiveDepositPoint()
    {
		return Vector3.zero;
	}

    public void GetDeposit( int count, DepositType type )     
    {

    }

    public void GetDamage( int count )
    {
        FFLogger.Log( "Damage: " + count );
    }

	public bool IsAlive()
    {
        return isAlive;
    }

	public void Subscribe_OnDeath( UnityMessage onDeathDelegate )
    {
        //! DO onDeath = null for clearing the invoke list
		onDeath += onDeathDelegate;
	}
#endregion

#region Implementation
    [ Button() ]
    private void Die()
    {
        colliderHealth.enabled = false;
        isAlive                = false;

		onDeath();
		onDeath = null;

		colliderListener_Seek_Stay.triggerEvent += VaultInEnemies;
	}

    [ Button() ]
    private void Revive()
    {
        // Toggle for re-activating OnTriggerEnter events
        colliderSeek.enabled = false;
        colliderSeek.enabled = true;

        // Enable healt collider since it can dake damage
        colliderHealth.enabled = true;
		isAlive = true;

		// 
		colliderListener_Seek_Stay.triggerEvent -= VaultInEnemies;
	}

    private void VaultInEnemies( Collider other )
    {
		// Vault enemies in
		var enemy = other.GetComponentInParent< Enemy >();

		var onCoolDown = lastVaultTime + GameSettings.Instance.window_cooldown_vault > Time.time;

		if( onCoolDown || enemy.IsInside ) return; 

		//Find vault position
		var position = new Vector3
		(
			Random.Range( navMeshLink.endPoint.x - navMeshLink.width, navMeshLink.endPoint.x + navMeshLink.width ),
			0,
			Random.Range( navMeshLink.endPoint.z / 2, navMeshLink.endPoint.z )
		);

		enemy.Vault( transform.TransformPoint( position ) );

		lastVaultTime = Time.time;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}