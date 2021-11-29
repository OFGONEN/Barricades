/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using FFStudio;
using UnityEditor;
using NaughtyAttributes;

public class Enemy : MonoBehaviour
{
#region Fields
    // Components \\
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    // Delegates \\
    private UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        animator     = GetComponent< Animator >();
        navMeshAgent = GetComponent< NavMeshAgent >();
		updateMethod = ExtensionMethods.EmptyMethod;
	}

    private void Start()
    {
		Spawn( transform.position );
		navMeshAgent.destination = Vector3.zero;
	}

    private void Update()
    {
		updateMethod();
	}
#endregion

#region API
    private void Spawn( Vector3 position )
    {
		ConfigureRandomValues();

		animator.Play( "idle_" + Random.Range( 1, GameSettings.Instance.enemy_animation_idle_count + 1 ), 0, Random.Range( 0f, 1f ) );
		navMeshAgent.Warp( position );

		updateMethod = CheckNavMeshAgent;
	}
#endregion

#region Implementation
    private void CheckNavMeshAgent() 
    {
		var isRunning = navMeshAgent.velocity.magnitude >= GameSettings.Instance.enemy_animation_run_speed;
		animator.SetBool( "run", isRunning );
	}

    private void ConfigureRandomValues()
    {
		animator.SetInteger( "random_idle", Random.Range( 1, GameSettings.Instance.enemy_animation_idle_count + 1 ) );
		animator.SetInteger( "random_run", Random.Range( 0, GameSettings.Instance.enemy_animation_run_count ) );
		animator.SetInteger( "random_attack", Random.Range( 0, GameSettings.Instance.enemy_animation_attack_count ) );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
    private void LogOffMeshLinkData()
    {
		var data = navMeshAgent.currentOffMeshLinkData;
		FFLogger.Log( "Start Point: " + data.startPos );
		FFLogger.Log( "End Point: " + data.endPos );
		FFLogger.Log( "LinkType: " + data.linkType );

        if( data.offMeshLink == null )
            FFLogger.Log( "Link is NULL" );
        else
            FFLogger.Log( "Link", data.offMeshLink.gameObject );
	}

    private void OnDrawGizmos()
    {
        if( !Application.isPlaying )
			return;

		// Handles.Label( transform.position + Vector3.up * 1.25f, "Speed: " + navMeshAgent.velocity.magnitude );
		Handles.Label( transform.position + Vector3.up, "OnLink: " + navMeshAgent.isOnOffMeshLink );
	}
#endif
#endregion
}