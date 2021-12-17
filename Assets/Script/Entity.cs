/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using NaughtyAttributes;

public abstract class Entity : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Fired Events" ) ] public ParticleSpawnEvent particle_spawn;
    [ BoxGroup( "Setup" ) ] public Transform origin_deposit;
    [ BoxGroup( "Setup" ) ] public ColliderListener colliderListener_Health_Enter;
    [ BoxGroup( "Setup" ) ] public ColliderListener colliderListener_Seek_Stay;
    [ BoxGroup( "Setup" ) ] public Image health_ratio_image;

    // Private Fields \\
    protected int health;
    protected float health_ratio; 
    protected int incomingDeposit;
    protected bool isAlive;

    // Delegates \\
	protected event UnityMessage onDeath;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
    protected abstract void Die();
    protected abstract void Revive();

    protected void InvokeOnDeath()
    {
		onDeath?.Invoke();
	}

    protected void ClearOnDeath()
    {
		onDeath = null;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
