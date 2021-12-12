/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

[CreateAssetMenu( fileName = "pool_collectable", menuName = "FF/Data/Pool/Collectable_Pool" )]
public class CollectablePool : ComponentPool< Collectable >
{
	private Transform initialParent;
	private bool initialActive;
	[ SerializeField, ReadOnly] private int activeCount;

	public Transform InitialParent => initialParent;
	public int ActiveCount => activeCount;

#region API
	public void InitPool( Transform parent, bool active )
	{
		initialParent = parent;
		initialActive = active;

		activeCount = 0;

		InitPool();

		foreach( var element in stack )
		{
			element.transform.parent = parent;
			element.gameObject.SetActive( active );
		}
	}

	public override Collectable GiveEntity()
	{
		Collectable entity;

		if( stack.Count > 0 )
			entity = stack.Pop();
		else
		{
			entity = GameObject.Instantiate( poolEntity );
			entity.transform.parent = initialParent;
			entity.gameObject.SetActive( initialActive );
		}

		activeCount++;
		return entity;
	}

	public override void ReturnEntity( Collectable entity )
	{
		entity.transform.parent = initialParent;
		stack.Push( entity );
		activeCount--;
	}
#endregion
}
