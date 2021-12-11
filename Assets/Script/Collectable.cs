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
    [ BoxGroup( "Shared Variables" ) ] public CollectablePool collectablePool;

    [ BoxGroup( "Setup" ) ] public DepositType depositType;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser colliderListener_Seek_Enter;

	// Private \\
	private Sequence depositSequence;
#endregion

#region Properties
#endregion

#region Unity API
    private void Start()
    {
		Spawn( transform.position );
	}
#endregion

#region API
    public void Spawn( Vector3 position )
    {
		gameObject.SetActive( true );
		//TODO transform.SetParent( collectablePool.InitialParent );
		transform.position = position;
		transform.localEulerAngles = Vector3.forward;

		colliderListener_Seek_Enter.triggerEvent += OnAllySeekEnter;
		colliderListener_Seek_Enter.AttachedCollider.enabled = true;
	}
#endregion

#region Implementation
    private void OnAllySeekEnter( Collider other ) // Only works with player
    {
		var interactable = ( other.GetComponent< ColliderListener >() ).AttachedComponent as Player;
        
        if( interactable == null || !( interactable.CanDeposit() > 0 ) ) return;
		
		transform.SetParent( interactable.GiveDepositOrigin() );

		depositSequence = DOTween.Sequence();

		depositSequence.Append( transform.DOLocalMove( 
			Vector3.up * transform.GetSiblingIndex() * GameSettings.Instance.collectable_stack_height, 
			GameSettings.Instance.collectable_duration_deposit )
			.SetEase( GameSettings.Instance.collectable_ease ) );

		depositSequence.Join( transform.DOLocalMoveX( 0, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.Join( transform.DOLocalMoveZ( 0, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.Join( transform.DOLocalRotate( Vector3.zero, GameSettings.Instance.collectable_duration_deposit ) );
		depositSequence.OnComplete( () => OnPlayerDeposit( interactable ) );

		colliderListener_Seek_Enter.AttachedCollider.enabled = false;
		colliderListener_Seek_Enter.triggerEvent -= OnAllySeekEnter;
	}

	private void OnPlayerDeposit( IInteractable interactable )
	{
		depositSequence = depositSequence.KillProper();
		interactable.GetDeposit( 1, depositType, this );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
