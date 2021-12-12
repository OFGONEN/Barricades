/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Guard : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared Variables" ) ] public BulletPool bulletPool;

    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser[] colliderListener_Enter_array;
    [ BoxGroup( "Setup" ) ] public ColliderListener_Exit_EventRaiser[] colliderListener_Exit_array;
    [ BoxGroup( "Setup" ) ] public Animator animator;

    private Dictionary< int, Transform > target_dictionary = new Dictionary<int, Transform>( 64 );
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
        for( var i = 0; i < colliderListener_Enter_array.Length; i++ )
        {
			colliderListener_Enter_array[ i ].triggerEvent += OnEntityEnter;
		}

        for( var i = 0; i < colliderListener_Exit_array.Length; i++ )
        {
			colliderListener_Exit_array[ i ].triggerEvent += OnEntityExit;
        }
    }

    private void OnDisable()
    {
        for( var i = 0; i < colliderListener_Enter_array.Length; i++ )
        {
			colliderListener_Enter_array[ i ].triggerEvent -= OnEntityEnter;
		}

        for( var i = 0; i < colliderListener_Exit_array.Length; i++ )
        {
			colliderListener_Exit_array[ i ].triggerEvent -= OnEntityExit;
        }       
    }
#endregion

#region API
#endregion

#region Implementation
    private void OnEntityEnter( Collider other )
    {

    }

    private void OnEntityExit( Collider other )
    {

    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
