/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using FFStudio;
using DG.Tweening;
using UnityEditor;
using NaughtyAttributes;

public class Enemy : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared Variables" ) ] public EnemyRagdollPool enemyRagdollPool;
    [ BoxGroup( "Setup" ) ] public Transform rootBone;

    // Components \\
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    // Delegates \\
    private UnityMessage updateMethod;
    private Sequence vaultSequence;
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

    private void Update()
    {
		updateMethod();
	}
#endregion

#region API
    public void Spawn( Vector3 position )
    {
		ConfigureRandomValues();

		animator.Play( "idle_" + Random.Range( 1, GameSettings.Instance.enemy_animation_idle_count + 1 ), 0, Random.Range( 0f, 1f ) );
		navMeshAgent.Warp( position );

		updateMethod = CheckNavMeshAgent;
	}
#endregion

#region Implementation
    private void Vault( OffMeshLinkData linkData )
    {
		animator.SetTrigger( "vault" );

		vaultSequence = DOTween.Sequence();

		vaultSequence.Append( transform.DOMove( linkData.endPos, GameSettings.Instance.enemy_animation_vault_duration ) );
		vaultSequence.Join( transform.DOLookAt( linkData.endPos, GameSettings.Instance.enemy_animation_vault_duration ) );
		vaultSequence.OnComplete( OnVaultComplete );
	}

    private void OnVaultComplete()
    {
		vaultSequence = null;
		navMeshAgent.CompleteOffMeshLink();

		updateMethod = CheckNavMeshAgent;
	}

    private void CheckNavMeshAgent() 
    {
		var isRunning = navMeshAgent.velocity.magnitude >= GameSettings.Instance.enemy_animation_run_speed;
		animator.SetBool( "run", isRunning );

        if( navMeshAgent.isOnOffMeshLink )
        {
			updateMethod = ExtensionMethods.EmptyMethod;
			Vault( navMeshAgent.currentOffMeshLinkData );
        }
	}

    private void ConfigureRandomValues()
    {
		animator.SetInteger( "random_idle", Random.Range( 1, GameSettings.Instance.enemy_animation_idle_count + 1 ) );
		animator.SetInteger( "random_run", Random.Range( 0, GameSettings.Instance.enemy_animation_run_count ) );
		animator.SetInteger( "random_attack", Random.Range( 0, GameSettings.Instance.enemy_animation_attack_count ) );
	}

    private void Die()
    {
		vaultSequence.KillProper(); //Kill if vault sequence is active

		gameObject.SetActive( false ); // Disable the object

        // Get a ragdoll from pool and replace with the current object
		var ragdoll = enemyRagdollPool.GiveEntity();
        
		rootBone.ReplaceHumanoidModel( ragdoll.RootBone );
		ragdoll.gameObject.SetActive( true );
		ragdoll.RootRigidbody.AddForce( Random.insideUnitSphere * GameSettings.Instance.enemy_death_velocity_range.RandomRange(), ForceMode.Impulse );
		ragdoll.Spawn();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
    private void Test_Start()
    {
        //For Testing
		Spawn( transform.position );
		navMeshAgent.destination = Vector3.zero;
    }

    [ Button() ]
    private void Test_Vault()
    {
		Vault( navMeshAgent.currentOffMeshLinkData );
	}

    [ Button() ]
    private void Test_Die()
    {
		Die();
	}

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