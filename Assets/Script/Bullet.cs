/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Bullet : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Event Listeners" ) ] public MultipleEventListenerDelegateResponse listener_level_finished;
    [ BoxGroup( "Fired Events" ) ] public ParticleSpawnEvent particle_spawn;
    [ BoxGroup( "Shared Variables" ) ] public BulletPool bulletPool;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser colliderListener_AllyDamage_Enter;

    // Private Fields \\
    private Rigidbody bulletRigidbody;
	private TrailRenderer trail_renderer;
#endregion

#region Properties
#endregion

#region Unity API
	private void OnEnable()
	{
		listener_level_finished.OnEnable();
	}

	private void OnDisable()
	{
		listener_level_finished.OnDisable();
	}

    private void Awake()
    {
		listener_level_finished.response = ReturnToPool;
        bulletRigidbody = GetComponent< Rigidbody >();
		trail_renderer = GetComponentInChildren< TrailRenderer >();

		trail_renderer.enabled = false;
	}
#endregion

#region API
    public void Spawn( Vector3 position, Vector3 direction, float speed )
    {
		gameObject.SetActive( true );

		trail_renderer.enabled = true;

		transform.position       = position;
		transform.forward        = direction;
		bulletRigidbody.velocity = direction * speed;

		colliderListener_AllyDamage_Enter.triggerEvent += OnTrigger;
	}
#endregion

#region Implementation
    private void OnTrigger( Collider other )
    {
		particle_spawn.Raise( "bullet", transform.position );
		ReturnToPool();
	}
    
    private void ReturnToPool()
    {
		gameObject.SetActive( false );
		trail_renderer.enabled = false;
		bulletPool.ReturnEntity( this );

		colliderListener_AllyDamage_Enter.triggerEvent -= OnTrigger;
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
