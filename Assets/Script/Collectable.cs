/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;

public class Collectable : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Event Listeners" ) ] public MultipleEventListenerDelegateResponse listener_level_finished;
    [ BoxGroup( "Fired Events" ) ] public ParticleSpawnEvent particle_spawn;
    [ BoxGroup( "Shared Variables" ) ] public CollectablePool collectablePool;

    [ BoxGroup( "Setup" ) ] public DepositType depositType;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser colliderListener_Seek_Enter;

	// Private \\
	private Sequence depositSequence;
	private TrailRenderer[] renderer_trail_array;
#endregion

#region Properties
#endregion

#region Unity API
	private void OnEnable()
	{
		listener_level_finished.OnEnable();
	}

	private void OnDisable()
	{
		listener_level_finished.OnDisable();
	}

    private void Awake()
    {
		listener_level_finished.response = ReturnToPool;
		renderer_trail_array = GetComponentsInChildren< TrailRenderer >();

		ToggleTrailRenderer( false );
	}
#endregion

#region API
    public void Spawn( Vector3 position )
    {
		ToggleTrailRenderer( false );

		gameObject.SetActive( true );
		transform.SetParent( collectablePool.InitialParent );
		transform.position = position;
		transform.localEulerAngles = Vector3.forward;

		colliderListener_Seek_Enter.triggerEvent += OnAllySeekEnter;
		colliderListener_Seek_Enter.AttachedCollider.enabled = true;
	}

	public void DepositToGround( Vector3 position, float rotation_y )
	{
		gameObject.SetActive( true );

		ToggleTrailRenderer( true );

		depositSequence.KillProper();
		depositSequence = DOTween.Sequence();
		depositSequence.Append( transform.DOMoveY( 
			position.y, 
			GameSettings.Instance.collectable_duration_deposit )
			.SetEase( GameSettings.Instance.collectable_ease_reverse ) );

		depositSequence.Join( transform.DOLocalMoveX( position.x, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.Join( transform.DOLocalMoveZ( position.z, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.Join( transform.DOLocalRotate( Vector3.up * rotation_y, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.OnComplete( OnGroundDeposit );

	}

	public void DepositToInteractable( IInteractable interactable, float delay )
	{
		var deposit_position = interactable.GiveDepositOrigin().position;

		AnimationCurve collectable_ease;

		if( transform.position.y < deposit_position.y )
			collectable_ease = GameSettings.Instance.collectable_ease;
		else
			collectable_ease = GameSettings.Instance.collectable_ease_reverse;

		ToggleTrailRenderer( true );
		depositSequence.KillProper();
		depositSequence = DOTween.Sequence();

		depositSequence.AppendCallback( SetInitialParent );
		depositSequence.Append( transform.DOMoveY( 
			deposit_position.y, 
			GameSettings.Instance.collectable_duration_deposit )
			.SetEase( collectable_ease ) );

		depositSequence.Join( transform.DOLocalMoveX( deposit_position.x, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.Join( transform.DOLocalMoveZ( deposit_position.z, GameSettings.Instance.collectable_duration_deposit ) );
		// depositSequence.Join( transform.DOLocalRotate( Vector3.zero, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.SetDelay( delay );
		depositSequence.OnComplete( () => OnInteractableDeposit( interactable ) );
	}
#endregion

#region Implementation
    private void OnAllySeekEnter( Collider other ) // Only works with player
    {
		var interactable = ( other.GetComponent< ColliderListener >() ).AttachedComponent as Player;
        
        if( interactable == null || !( interactable.CanDeposit() > 0 ) ) return;
		
		transform.SetParent( interactable.GiveDepositOrigin() );
		interactable.GetDeposit( 1, depositType, this );

		ToggleTrailRenderer( true );

		depositSequence.KillProper();
		depositSequence = DOTween.Sequence();

		depositSequence.Append( transform.DOLocalMoveY( 
			transform.GetSiblingIndex() * GameSettings.Instance.collectable_stack_height, 
			GameSettings.Instance.collectable_duration_deposit )
			.SetEase( GameSettings.Instance.collectable_ease ) );

		depositSequence.Join( transform.DOLocalMoveX( 0, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.Join( transform.DOLocalMoveZ( 0, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.Join( transform.DOLocalRotate( Vector3.zero, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.OnComplete( OnPlayerDeposit );

		colliderListener_Seek_Enter.AttachedCollider.enabled = false;
		colliderListener_Seek_Enter.triggerEvent -= OnAllySeekEnter;
	}

	private void OnPlayerDeposit()
	{
		depositSequence = depositSequence.KillProper();
		transform.localScale = Vector3.Scale( transform.localScale , GameSettings.Instance.collectable_stack_size );

		ToggleTrailRenderer( false );
	}

	private void OnInteractableDeposit( IInteractable interactable )
	{
		depositSequence = depositSequence.KillProper();

		ToggleTrailRenderer( false );

		gameObject.SetActive( false );
		interactable.GetDeposit( 1, depositType );

		particle_spawn.Raise( "deposit", transform.position );

		collectablePool.ReturnEntity( this );
	}

	private void OnGroundDeposit()
	{
		depositSequence = depositSequence.KillProper();

		ToggleTrailRenderer( false );

		colliderListener_Seek_Enter.triggerEvent += OnAllySeekEnter;
		colliderListener_Seek_Enter.AttachedCollider.enabled = true;
	}

	private void SetInitialParent()
	{
		transform.SetParent( collectablePool.InitialParent );
		transform.localScale = Vector3.one;
	}

	private void ReturnToPool()
	{
		depositSequence = depositSequence.KillProper();

		ToggleTrailRenderer( false );

		transform.localScale = Vector3.one;
		gameObject.SetActive( false );
		collectablePool.ReturnEntity( this );
	}

	private void ToggleTrailRenderer( bool active )
	{
		foreach( var renderer in renderer_trail_array )
		{
			renderer.enabled = active;
		}
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
