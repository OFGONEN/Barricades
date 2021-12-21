/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Test_ZombieAnimation : MonoBehaviour
{
#region Fields
    Animator animator;
    public int idleStateCount;

    // public Transform baseBone;
    // public Transform targetBone;

    // public Transform target;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        animator = GetComponent< Animator >();
		//Play random state called idle_X with randomized normalized Time. 
		animator.Play( "idle_" + Random.Range( 0, idleStateCount + 1 ), 0, Random.Range( 0, 1f ) );
	}

	// void OnAnimatorIK( int layerIndex )
    // {
	// 	animator.SetLookAtPosition( target.position );
	// 	animator.SetLookAtWeight( 1, 1, 1, 1 );
	// }

#endregion

#region API
    // [ Button() ]
    // public void Killed()
    // {
	// 	gameObject.SetActive( false );
	// 	targetBone.parent.gameObject.SetActive( true );
	// 	baseBone.ReplaceHumanoidModel( targetBone );
	// }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}