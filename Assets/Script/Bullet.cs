/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Bullet : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared Variables" ) ] public BulletPool bulletPool;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser colliderListener_AllyDamage_Enter;

    // Private Fields \\
    private Rigidbody bulletRigidbody;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        bulletRigidbody = GetComponent< Rigidbody >();
    }
#endregion

#region API
    public void Spawn( Vector3 position, Vector3 direction, float speed )
    {
		gameObject.SetActive( true );

		transform.position       = position;
		transform.forward        = direction;
		bulletRigidbody.velocity = direction * speed;

		colliderListener_AllyDamage_Enter.triggerEvent += OnTrigger;
	}
#endregion

#region Implementation
    private void OnTrigger( Collider other )
    {
        //TODO(OFG): Spawn hit particle effect

		gameObject.SetActive( false );
		bulletPool.ReturnEntity( this );

		colliderListener_AllyDamage_Enter.triggerEvent -= OnTrigger;
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
