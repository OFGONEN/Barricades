/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Window : MonoBehaviour, IInteractable
{
#region Fields
    [ BoxGroup( "Setup" ) ] public BoxCollider colliderHealth;

    // Private Fields \\
    [ SerializeField ] private bool isAlive = true;

    // Delegates \\
    private event UnityMessage onDeath;
#endregion

#region Properties
#endregion

#region Unity API
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

    public void Deposit( int count )     
    {

    }

    public void Damage( int count )
    {
        FFLogger.Log( "Damage: " + count );
    }

	public bool IsAlive()
    {
        return isAlive;
    }

	public void Subscribe_OnDeath( UnityMessage onDeathDelegate )
    {
        //! DO onDeath = null for clearin the invoke list
		onDeath += onDeathDelegate;
	}

#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
    private void Test_Death()
    {
		var center = colliderHealth.center;
		center.y = -10f;

		colliderHealth.center = center;

		isAlive = false;
		colliderHealth.enabled = false;

		onDeath();
		onDeath = null;
	}

    [ Button() ]
    private void Test_Alive()
    {
		isAlive = true;
		colliderHealth.enabled = true;

		var center = colliderHealth.center;
		center.y = 1.25f;
		colliderHealth.center = center;
	}
#endif
#endregion
}