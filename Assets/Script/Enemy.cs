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
    [ BoxGroup( "Shared Variables" ) ] public EnemySet enemySet;
    [ BoxGroup( "Shared Variables" ) ] public EnemyPool enemyPool;
    [ BoxGroup( "Shared Variables" ) ] public EnemyRagdollPool enemyRagdollPool;
    [ BoxGroup( "Shared Variables" ) ] public SharedReferenceNotifier destinationInside;
    [ BoxGroup( "Shared Variables" ) ] public SharedReferenceNotifier destinationOutside;

    [ BoxGroup( "Event Listeners" ) ] public MultipleEventListenerDelegateResponse listener_level_complete;
    [ BoxGroup( "Fired Events" ) ] public GameEvent enemyDiedEvent;
    [ BoxGroup( "Fired Events" ) ] public ParticleSpawnEvent particle_spawn;

    [ BoxGroup( "Setup" ) ] public Transform rootBone;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser event_collide_hitbox;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser event_collide_seek;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser event_collide_damage;

	// Editor Hidden \\
	[ HideInInspector ] public bool isAttacking = false;

	// Private \\
	private bool isInside = false;
	private bool isAlive = false;
	private bool isRunning = false;
	private Vector3 vaultPosition;
	private SharedReferenceNotifier currentDestination;
	private Transform currentDestinationTransform;

	// Components \\
	private Animator animator;
    private NavMeshAgent navMeshAgent;
	private Collider currentAttackCollider;
	public IInteractable currentInteractable;

	// Delegates \\
	private UnityMessage updateMethod;
	private UnityMessage onInteractableDeath;
	private Sequence vaultSequence;

	// Properties
	public bool IsInside => isInside;
	public bool IsAlive => isAlive;
#endregion

#region Properties
#endregion

#region Unity API
	private void OnEnable()
	{
		event_collide_hitbox.triggerEvent += OnCollision_HitBox;
		event_collide_seek.triggerEvent   += OnCollision_Seek;
		event_collide_damage.triggerEvent += OnCollision_Damage;

		listener_level_complete.OnEnable();
	}

	private void OnDisable()
	{
		event_collide_hitbox.triggerEvent -= OnCollision_HitBox;
		event_collide_seek.triggerEvent   -= OnCollision_Seek;
		event_collide_damage.triggerEvent -= OnCollision_Damage;

		event_collide_damage.AttachedCollider.enabled = false;
		event_collide_seek.AttachedCollider.enabled   = false;
		event_collide_hitbox.AttachedCollider.enabled = false;

		listener_level_complete.OnDisable();

		// Remove from enemy set
		enemySet.RemoveDictionary( GetInstanceID() );
	}

    private void Awake()
    {
        animator     = GetComponent< Animator >();
        navMeshAgent = GetComponent< NavMeshAgent >();
		updateMethod = ExtensionMethods.EmptyMethod;

		currentDestination = destinationOutside;

		listener_level_complete.response = LevelFinishedResponse;
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
		isAlive = true;

		// Enable Colliders
		event_collide_seek.AttachedCollider.enabled   = true;
		event_collide_hitbox.AttachedCollider.enabled = true;

		ConfigureRandomValues();

		var randomIdle = Random.Range( 1, GameSettings.Instance.enemy_animation_idle_count + 1 );
		animator.Play( "idle_" + randomIdle , 0, Random.Range( 0f, 1f ) );
		animator.Play( "idle_" + randomIdle , 1, Random.Range( 0f, 1f ) );

		navMeshAgent.Warp( position );

		currentDestinationTransform = currentDestination.SharedValue as Transform;
		navMeshAgent.destination = currentDestinationTransform.position;
		updateMethod = CheckNavMeshAgent;
	}

    public void Vault( Vector3 position )
    {
		isInside      = true;
		vaultPosition = position;
		event_collide_seek.AttachedCollider.enabled = false;

		animator.SetLayerWeight( 1, 0 );
		animator.SetTrigger( "vault" );

		vaultSequence = DOTween.Sequence();
		vaultSequence.Append( transform.DOMove( position, GameSettings.Instance.enemy_animation_vault_duration ) );
		vaultSequence.Join( transform.DOLookAt( position, GameSettings.Instance.enemy_animation_vault_duration ) );
		vaultSequence.OnComplete( OnVaultComplete );
	}

	public Vector3 GiveShootPosition()
	{
		Vector3 offSet;

		if( isRunning )
			offSet = GameSettings.Instance.guard_shoot_offset;
		else
			offSet = Vector3.up * GameSettings.Instance.guard_shoot_offset.y;

		return transform.TransformPoint( offSet );
	}
#endregion

#region Implementation
    private void OnVaultComplete()
    {
		enemySet.AddDictionary( GetInstanceID(), this );

		updateMethod  = CheckNavMeshAgent;
		vaultSequence = vaultSequence.KillProper();

		navMeshAgent.CompleteOffMeshLink();
		navMeshAgent.Warp( vaultPosition );

		currentDestination          = destinationInside;
		currentDestinationTransform = currentDestination.SharedValue as Transform;
		navMeshAgent.destination    = currentDestinationTransform.position;

		event_collide_seek.AttachedCollider.enabled = true;
	}

    private void CheckNavMeshAgent() 
    {
		CheckIfRunning();

		var targetPosition = currentDestinationTransform.transform.position;

		if( Vector3.Distance( transform.position, targetPosition ) >= GameSettings.Instance.enemy_distance_targetFollow )
			navMeshAgent.destination = targetPosition;

		if( currentAttackCollider != null )
		{
			transform.LookAtOverTimeAxis( targetPosition, Vector3.up, navMeshAgent.angularSpeed );

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
		// UnSub 
		currentInteractable?.UnSubscribe_OnDeath( OnInteractableDeath );

		// Default variables
		isInside              = false;
		isAlive               = false;
		isAttacking           = false;
		currentDestination    = destinationOutside;
		currentAttackCollider = null;
		currentInteractable   = null;
		vaultSequence = vaultSequence.KillProper(); //Kill if vault sequence is active

		gameObject.SetActive( false ); // Disable the object
		enemyPool.ReturnEntity( this ); // Return to pool

		// Get a ragdoll from pool and replace with the current object
		var ragdoll = enemyRagdollPool.GiveEntity();
        
		rootBone.ReplaceHumanoidModel( ragdoll.RootBone );
		ragdoll.gameObject.SetActive( true );
		ragdoll.RootRigidbody.AddForce( direction * GameSettings.Instance.enemy_death_velocity_range.RandomRange(), ForceMode.Impulse );
		ragdoll.Spawn();

		enemyDiedEvent.Raise();
	}

    private void CheckIfRunning()
    {
		isRunning = navMeshAgent.velocity.magnitude >= GameSettings.Instance.enemy_animation_run_speed;
		animator.SetBool( "run", isRunning );
	}

	private void OnCollision_HitBox( Collider other )
	{
		var direction = other.transform.position - transform.position;
		Die( direction.SetY( 0 ) );
	}

	private void OnCollision_Seek( Collider other )
	{
		var interactable = other.GetComponentInParent< IInteractable >();

		if( interactable != null && interactable.IsAlive() && ( isInside != ( interactable is Window ) ) ) 
		{
			currentInteractable?.UnSubscribe_OnDeath( OnInteractableDeath );
			currentInteractable         = interactable;
			currentAttackCollider       = interactable.GiveHealthCollider();
			currentDestinationTransform = currentAttackCollider.transform;
			navMeshAgent.destination    = currentDestinationTransform.position;

			interactable.Subscribe_OnDeath( OnInteractableDeath );
		}
	}

	private void OnCollision_Damage( Collider other )
	{
		var interactable = other.GetComponentInParent< IInteractable >();
		interactable.GetDamage( GameSettings.Instance.enemy_damage );

		particle_spawn.Raise( "enemy_damage", GiveShootPosition() );
	}

	private void OnInteractableDeath()
	{
		currentInteractable   = null;
		currentAttackCollider = null;

		navMeshAgent.Warp( transform.position );
		currentDestinationTransform = currentDestination.SharedValue as Transform;
		navMeshAgent.destination    = currentDestinationTransform.position;
	}

	private void LevelFinishedResponse()
	{
		// UnSub 
		currentInteractable?.UnSubscribe_OnDeath( OnInteractableDeath );

		// Default variables
		isInside              = false;
		isAlive               = false;
		isAttacking           = false;
		currentDestination    = destinationOutside;
		currentAttackCollider = null;
		currentInteractable   = null;
		vaultSequence = vaultSequence.KillProper(); //Kill if vault sequence is active

		gameObject.SetActive( false ); // Disable the object
		enemyPool.ReturnEntity( this ); // Return to pool
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
	[ Button() ]
	public void Test_Die()
	{
		Die( Random.onUnitSphere );
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