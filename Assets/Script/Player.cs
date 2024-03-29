/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FFStudio;
using NaughtyAttributes;

public class Player : Entity, IInteractable
{
#region Fields
	[ BoxGroup( "Shared Variable" ) ] public SharedReferenceNotifier notifier_shared_camera;
	[ BoxGroup( "Shared Variable" ) ] public SharedFloatNotifier notifier_health_ratio;
	[ BoxGroup( "Shared Variable" ) ] public SharedInput_JoyStick input_JoyStick;

	[ BoxGroup( "Setup" ) ] public Transform origin_deposit_wait;
	[ BoxGroup( "Setup" ) ] public Animator animator_dog;
	[ BoxGroup( "Setup" ) ] public ParticleSystem particle_hit;
	[ BoxGroup( "Fired Events" ) ] public GameEvent levelFailed;

	[ HideInInspector ] public bool onDamageCooldown;

	// Private \\
	private Stack< Collectable > collectables;
	private float lastDeposit;

	// Component 
	private Animator animator;
	private NavMeshAgent navMeshAgent;
	private Transform transform_camera;

	// Delegate
	private UnityMessage updateMethod;

	// Properties
	private bool DepositCooldown => lastDeposit > Time.time;
#endregion

#region Properties
#endregion

#region Unity API

	private void Awake()
    {
		updateMethod = ExtensionMethods.EmptyMethod;
		navMeshAgent = GetComponent< NavMeshAgent >();
		animator     = GetComponentInChildren< Animator >();

		health                      = GameSettings.Instance.player_max_health;
		collectables                = new Stack< Collectable >( GameSettings.Instance.player_max_collectable );
		navMeshAgent.updateRotation = false;
	}

	private void Start()
	{
		Revive();
	}

	private void Update()
	{
		updateMethod();
	}
#endregion

#region API
    public Collider GiveHealthCollider()
    {
		return colliderListener_Health_Enter.AttachedCollider;
	}

    public Transform GiveDepositOrigin()
    {
		return origin_deposit;
	}

    public void GetDeposit( int count, DepositType type, Collectable collectable = null )     
    {
		collectables.Push( collectable );
	}

    public void GetDamage( int count )
    {
		if( onDamageCooldown ) return;

		health = Mathf.Max( health - 1, 0 );
		notifier_health_ratio.SharedValue = health / ( float )GameSettings.Instance.player_max_health;

		particle_hit.transform.forward = Random.onUnitSphere;
		particle_hit.Play(); // Particle to play when get it 

		if( health <= 0 )
		{
			Die();
		}
		else
			animator.SetTrigger( "hit" );

		UpdateHealthRatio();
	}

	public void UpdateHealthRatio()
	{
		health_ratio = health / ( float ) GameSettings.Instance.player_max_health;
		//TODO Update Health Bar
	}

	public bool IsAlive()
    {
        return isAlive;
    }

    public int CanDeposit()
    {
		return GameSettings.Instance.player_max_collectable - collectables.Count;
	}

    public void IncomingDeposit( int count )
    {
	}

	public void Subscribe_OnDeath( UnityMessage onDeathDelegate )
    {
        //! DO onDeath = null for clearing the invoke list
		onDeath += onDeathDelegate;
	}

	public void UnSubscribe_OnDeath( UnityMessage onDeathDelegate )
	{
		onDeath -= onDeathDelegate;
	}
#endregion

#region Implementation
    protected override void Die()
    {
		updateMethod = ExtensionMethods.EmptyMethod;
		isAlive      = false;

		InvokeOnDeath();
		ClearOnDeath();

		colliderListener_Seek_Stay.triggerEvent -= OnAllySeekEnter;

		animator.SetTrigger( "death" );
		animator_dog.SetBool( "run", false );

		//! Raise Level Fail Event
		levelFailed.Raise();
	}

    protected override void Revive()
    {
		updateMethod = Move;
		isAlive      = true;

		colliderListener_Seek_Stay.triggerEvent += OnAllySeekEnter;

		transform_camera = notifier_shared_camera.SharedValue as Transform;
	}

	private void Move()
	{
		var direction = input_JoyStick.SharedValue.ConvertV3();

		// direction = ( transform_camera.TransformPoint( direction ) - transform_camera.position ).normalized.SetY( 0 );
		direction = transform_camera.TransformVector( direction ).SetY( 0 ).normalized;
		navMeshAgent.Move( direction * navMeshAgent.speed * Time.deltaTime );

		if( direction != Vector3.zero )
		{
			transform.forward = direction;
			animator.SetBool( "running", true );
			animator_dog.SetBool( "run", true );
		}
		else
		{
			animator.SetBool( "running", false );
			animator_dog.SetBool( "run", false );
		}
	}

	private void OnAllySeekEnter( Collider other )
	{
		var interactable = ( other.GetComponent< ColliderListener >() ).AttachedComponent as IInteractable;

		if( DepositCooldown || interactable == null || !( interactable.CanDeposit() > 0 ) || collectables.Count <= 0 ) return;

		lastDeposit = Time.time + GameSettings.Instance.player_cooldown_deposit;

		var collectable = collectables.Pop();
		collectable.transform.SetParent( origin_deposit_wait );
		interactable.IncomingDeposit( ( int )collectable.depositType + 1 );
		collectable.DepositToInteractable( interactable, GameSettings.Instance.collectable_delay_deposit );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
