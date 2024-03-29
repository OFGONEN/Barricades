/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using FFStudio;
using NaughtyAttributes;

public class Window : Entity, IInteractable
{
#region Fields
    [ BoxGroup( "Setup" ) ] public NavMeshLink navMeshLink;
    [ BoxGroup( "Setup" ) ] public MeshFilter[] stackMeshFilters;
    [ BoxGroup( "Setup" ) ] public GameObject deadMesh;

	// Private Fields \\
	private float lastVaultTime;

	// Deposit
	private int[] stackHealths = { 0, 0 , 0};
#endregion

#region Properties
#endregion

#region Unity API
    private void OnDisable()
    {
		colliderListener_Seek_Stay.ClearEventList();
	}

	private void Awake()
	{
		ui_deposit = GetComponentInChildren< UIWorld_Deposit >();
	}

    private void Start()
    {
		ui_deposit.Init( 0, stackHealths.Length );
		Die();
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
		int emptyIndex = 0;

        for( var i = 0; i < stackHealths.Length; i++ )
        {
            if( stackHealths[ i ] <= 0 )
			{
				emptyIndex = i;
			}
		}

		incomingDeposit--;
		stackHealths[ emptyIndex ] = count * ( ( int )type + 1 );
		stackMeshFilters[ emptyIndex ].mesh = GameSettings.Instance.window_meshes[ ( int )type ];

		UpdateHealthRatio();

		if( !isAlive )
			Revive();
	}

    public void GetDamage( int count )
    {
		int index = 0;

        for( var i = 0; i < stackHealths.Length; i++ )
        {
            if( stackHealths[ i ] > 0 )
				index = i;
		}

		stackHealths[ index ] -= count;

        if( stackHealths[ index ] <= 0 )
			stackMeshFilters[ index ].mesh = null;

		bool isDead = true;

		for( var i = 0; i < stackHealths.Length; i++ )
        {
            if( stackHealths[ i ] > 0 )
				isDead = false;
		}


		UpdateHealthRatio();

        if( isDead )
			Die();
	}

	public void UpdateHealthRatio()
	{
		int currentHealth = 0;

		for( var i = 0; i < stackHealths.Length; i++ )
		{
			if( stackHealths[ i ] != 0 )
				currentHealth++;
		}

		ui_deposit.SetValue( currentHealth );
	}

	public bool IsAlive()
    {
        return isAlive;
    }

    public int CanDeposit()
    {
		int depositCount = 0;
		for( var i = 0; i < stackHealths.Length; i++ )
        {
            if( stackHealths[ i ] <= 0 )
				depositCount++;
		}

		return depositCount - incomingDeposit;
	}

    public void IncomingDeposit( int count )
    {
		incomingDeposit++;
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
    [ Button() ]
    protected override void Die()
    {
		colliderListener_Health_Enter.SetColliderActive( false );
		isAlive                = false;
		deadMesh.SetActive( true );

		InvokeOnDeath();
		ClearOnDeath();

		colliderListener_Seek_Stay.triggerEvent += VaultInEnemies;
	}

    [ Button() ]
    protected override void Revive()
    {
		// Toggle for re-activating OnTriggerEnter events
		colliderListener_Seek_Stay.SetColliderActive( false );
		colliderListener_Seek_Stay.SetColliderActive( true );

        // Enable healt collider since it can dake damage
		colliderListener_Health_Enter.SetColliderActive( true );
		isAlive = true;
		deadMesh.SetActive( false );

		// 
		colliderListener_Seek_Stay.triggerEvent -= VaultInEnemies;
	}

    private void VaultInEnemies( Collider other )
    {
		// Vault enemies in
		var enemy = other.GetComponentInParent< Enemy >();

		var onCoolDown = lastVaultTime + GameSettings.Instance.window_cooldown_vault > Time.time;

		if( enemy == null || onCoolDown || enemy.IsInside ) return; 

		//Find vault position
		var position = new Vector3
		(
			Random.Range( navMeshLink.endPoint.x - navMeshLink.width, navMeshLink.endPoint.x + navMeshLink.width ),
			0,
			Random.Range( navMeshLink.endPoint.z / 2, navMeshLink.endPoint.z )
		);

		enemy.Vault( transform.TransformPoint( position ) );

		lastVaultTime = Time.time;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}