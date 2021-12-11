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
	[ BoxGroup( "Shared Variable" ) ] public SharedInput_JoyStick input_JoyStick;
	[ BoxGroup( "Fired Events" ) ] public GameEvent levelFailed;

	[ HideInInspector ] public bool onDamageCooldown;

	// Private \\
	private Stack< Collectable > collectables;

	// Component 
	private Animator animator;
	private NavMeshAgent navMeshAgent;

	// Delegate
	private UnityMessage updateMethod;
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

		if( health <= 0 )
		{
			Die();
		}
		else
			animator.SetTrigger( "hit" );

	}

	public bool IsAlive()
    {
        return isAlive;
    }

    public bool CanDeposit()
    {
		return collectables.Count < GameSettings.Instance.player_max_collectable;
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

		//! Raise Level Fail Event
		levelFailed.Raise();
	}

    protected override void Revive()
    {
		updateMethod = Move;
		isAlive      = true;

		colliderListener_Seek_Stay.triggerEvent += OnAllySeekEnter;
	}

	private void Move()
	{
		var direction = input_JoyStick.SharedValue.ConvertV3();
		navMeshAgent.Move( direction * navMeshAgent.speed * Time.deltaTime );

		if( direction != Vector3.zero )
		{
			transform.forward = direction;
			animator.SetBool( "running", true );
		}
		else
			animator.SetBool( "running", false );
	}

	private void OnAllySeekEnter( Collider other )
	{
		var interactable = ( other.GetComponent< ColliderListener >() ).AttachedComponent as IInteractable;

		if( interactable == null || !interactable.CanDeposit() || collectables.Count <= 0 ) return;

		var collectable = collectables.Pop();

		//TODO make this work with collectable pool
		collectable.transform.SetParent( transform );
		collectable.gameObject.SetActive( false );

		interactable.GetDeposit( 1, collectable.depositType, collectable );
		FFLogger.Log( "Interactable:" + Time.frameCount , interactable.GiveDepositOrigin() );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
