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

	// Component \\
	private Animator animator;
	private NavMeshAgent navMeshAgent;

	// Delegate \\ 
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
		return null;
	}

    public Vector3 GiveDepositPoint()
    {
		return Vector3.zero;
	}

    public void GetDeposit( int count, DepositType type )     
    {
	}

    public void GetDamage( int count )
    {
	}

	public bool IsAlive()
    {
        return isAlive;
    }

    public bool CanDeposit()
    {
		return false;
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
	}

    protected override void Revive()
    {
		updateMethod = Move;
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
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
