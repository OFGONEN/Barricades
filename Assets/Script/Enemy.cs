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
    [ BoxGroup( "Shared Variables" ) ] public EnemyPool enemyPool;
    [ BoxGroup( "Shared Variables" ) ] public EnemyRagdollPool enemyRagdollPool;
    [ BoxGroup( "Shared Variables" ) ] public SharedReferenceNotifier destinationInside;
    [ BoxGroup( "Shared Variables" ) ] public SharedReferenceNotifier destinationOutside;
    [ BoxGroup( "Setup" ) ] public Transform rootBone;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser event_collide_hitbox;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser event_collide_seek;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser event_collide_damage;

	// 
	[ HideInInspector ] public bool isAttacking = false;

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
	private void OnEnable()
	{
		event_collide_hitbox.triggerEnter += OnCollision_HitBox;
		event_collide_seek.triggerEnter += OnCollision_Seek_Outside;
	}

	private void OnDisable()
	{
		event_collide_hitbox.triggerEnter -= OnCollision_HitBox;
		event_collide_seek.triggerEnter -= OnCollision_Seek_Outside;
	}

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
		gameObject.SetActive( true );

		ConfigureRandomValues();

		var randomIdle = Random.Range( 1, GameSettings.Instance.enemy_animation_idle_count + 1 );
		animator.Play( "idle_" + randomIdle , 0, Random.Range( 0f, 1f ) );
		animator.Play( "idle_" + randomIdle , 1, Random.Range( 0f, 1f ) );

		navMeshAgent.Warp( position );
		navMeshAgent.destination = ( destinationOutside.SharedValue as Transform ).position;

		updateMethod = CheckNavMeshAgent_Outside;
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

		navMeshAgent.destination = ( destinationInside.SharedValue as Transform ).position;
		updateMethod = CheckNavMeshAgent_Inside;
	}

    private void CheckNavMeshAgent_Outside() 
    {
		CheckIfRunning();

        // if( navMeshAgent.isOnOffMeshLink )
        // {
		// 	updateMethod = ExtensionMethods.EmptyMethod;
		// 	Vault( navMeshAgent.currentOffMeshLinkData );
        // }
	}

    private void CheckNavMeshAgent_Inside() 
    {
		CheckIfRunning();
	}

    private void ConfigureRandomValues()
    {
		animator.SetInteger( "random_idle", Random.Range( 1, GameSettings.Instance.enemy_animation_idle_count + 1 ) );
		animator.SetInteger( "random_run", Random.Range( 0, GameSettings.Instance.enemy_animation_run_count ) );
		animator.SetInteger( "random_attack", Random.Range( 0, GameSettings.Instance.enemy_animation_attack_count ) );
	}

    private void Die( Vector3 direction )
    {
		vaultSequence.KillProper(); //Kill if vault sequence is active

		gameObject.SetActive( false ); // Disable the object
		enemyPool.ReturnEntity( this ); // Return to pool

		// Get a ragdoll from pool and replace with the current object
		var ragdoll = enemyRagdollPool.GiveEntity();
        
		rootBone.ReplaceHumanoidModel( ragdoll.RootBone );
		ragdoll.gameObject.SetActive( true );
		ragdoll.RootRigidbody.AddForce( direction * GameSettings.Instance.enemy_death_velocity_range.RandomRange(), ForceMode.Impulse );
		ragdoll.Spawn();
	}

    private void CheckIfRunning()
    {
		var isRunning = navMeshAgent.velocity.magnitude >= GameSettings.Instance.enemy_animation_run_speed;
		animator.SetBool( "run", isRunning );
    }

	private void OnCollision_HitBox( Collider other )
	{
		var direction = transform.position - other.transform.position;
		Die( direction.SetY( 0 ) );
	}

	private void OnCollision_Seek_Outside( Collider other )
	{
		FFLogger.Log( "Seeked: " + other.name, other.gameObject );

		var interactable = other.GetComponentInParent< IInteractable >();
		interactable.Damage( 1 );
	}

	private void OnCollision_Damage( Collider other )
	{

	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
    private void Test_Start()
    {
        //For Testing
		Spawn( transform.position );
    }

    [ Button() ]
    private void Test_Vault()
    {
		Vault( navMeshAgent.currentOffMeshLinkData );
	}

    [ Button() ]
    private void Test_Die()
    {
		Die( Random.onUnitSphere );
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