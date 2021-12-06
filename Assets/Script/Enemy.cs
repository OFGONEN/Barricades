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

	// Editor Hidden \\
	[ HideInInspector ] public bool isAttacking = false;

	// Private \\
	private bool isInside = false;
	private SharedReferenceNotifier currentDestination;

	// Components \\
	private Animator animator;
    private NavMeshAgent navMeshAgent;
	private Collider attackCollider;

	// Delegates \\
	private UnityMessage updateMethod;
	private UnityMessage onInteractableDeath;
	private Sequence vaultSequence;

	// Properties
	public bool IsInside => isInside;
#endregion

#region Properties
#endregion

#region Unity API
	private void OnEnable()
	{
		event_collide_hitbox.triggerEvent += OnCollision_HitBox;
		event_collide_seek.triggerEvent   += OnCollision_Seek;
		event_collide_damage.triggerEvent += OnCollision_Damage;
	}

	private void OnDisable()
	{
		event_collide_hitbox.triggerEvent -= OnCollision_HitBox;
		event_collide_seek.triggerEvent   -= OnCollision_Seek;
		event_collide_damage.triggerEvent -= OnCollision_Damage;
	}

    private void Awake()
    {
        animator     = GetComponent< Animator >();
        navMeshAgent = GetComponent< NavMeshAgent >();
		updateMethod = ExtensionMethods.EmptyMethod;

		currentDestination = destinationOutside;
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
		navMeshAgent.destination = ( currentDestination.SharedValue as Transform ).position;

		updateMethod = CheckNavMeshAgent;
	}

    public void Vault( Vector3 position )
    {
		isInside = true;
		event_collide_seek.AttachedCollider.enabled = false;

		animator.SetLayerWeight( 1, 0 );
		animator.SetTrigger( "vault" );

		vaultSequence = DOTween.Sequence();
		vaultSequence.Append( transform.DOMove( position, GameSettings.Instance.enemy_animation_vault_duration ) );
		vaultSequence.Join( transform.DOLookAt( position, GameSettings.Instance.enemy_animation_vault_duration ) );
		vaultSequence.OnComplete( OnVaultComplete );
	}
#endregion

#region Implementation
    private void OnVaultComplete()
    {
		updateMethod  = CheckNavMeshAgent;
		vaultSequence = null;

		navMeshAgent.Warp( transform.position );
		currentDestination       = destinationInside;
		navMeshAgent.destination = ( currentDestination.SharedValue as Transform ).position;

		event_collide_seek.AttachedCollider.enabled = true;
	}

    private void CheckNavMeshAgent() 
    {
		CheckIfRunning();

		if( attackCollider != null )
		{
			transform.LookAtOverTimeAxis( attackCollider.transform.position, Vector3.up, navMeshAgent.angularSpeed );

			if( !isAttacking )
				animator.SetTrigger( "attack" );
		}
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

	private void OnCollision_Seek( Collider other )
	{
		var interactable = other.GetComponentInParent< IInteractable >();

		if( interactable.IsAlive() ) 
		{
			attackCollider           = interactable.GiveHealthCollider();
			navMeshAgent.destination = attackCollider.transform.position;

			interactable.Subscribe_OnDeath( OnInteractableDeath );
		}
	}

	private void OnCollision_Damage( Collider other )
	{
		var interactable = other.GetComponentInParent< IInteractable >();
		interactable.GetDamage( GameSettings.Instance.enemy_damage );
	}

	private void OnInteractableDeath()
	{
		attackCollider = null;
		navMeshAgent.Warp( transform.position );
		navMeshAgent.destination = ( currentDestination.SharedValue as Transform ).position;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
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