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
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}