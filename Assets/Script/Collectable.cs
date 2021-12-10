/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Collectable : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared Variables" ) ] public CollectablePool collectablePool;

    [ BoxGroup( "Setup" ) ] public DepositType depositType;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser colliderListener_Seek_Enter;
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
    public void OnAllySeekEnter( Collider other )
    {
        var interactable = other.GetComponentInParent< IInteractable >();
        
        if( interactable == null || !interactable.CanDeposit() ) return;

		var origin_deposit = interactable.GiveDepositOrigin();

		interactable.GetDeposit( 1, depositType, this );

		transform.SetParent( origin_deposit );
		transform.localPosition    = Vector3.up * transform.GetSiblingIndex() * GameSettings.Instance.collectable_stack_height;
		transform.localEulerAngles = Vector3.zero;

		colliderListener_Seek_Enter.AttachedCollider.enabled = false;
		colliderListener_Seek_Enter.triggerEvent -= OnAllySeekEnter;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
