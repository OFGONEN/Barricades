/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[CreateAssetMenu( fileName = "pool_enemy", menuName = "FF/Data/Pool/Enemy_Pool" )]
public class EnemyPool : ComponentPool< Enemy >
{
	private Transform initialParent;
	private bool initialActive;
	#region API
	public void InitPool( Transform parent, bool active )
	{
		initialParent = parent;
		initialActive = active;

		InitPool();

		foreach( var element in stack )
		{
			element.transform.parent = parent;
			element.gameObject.SetActive( active );
		}
	}

	public override Enemy GiveEntity()
	{
		Enemy entity;

		if( stack.Count > 0 )
			entity = stack.Pop();
		else
		{
			entity = GameObject.Instantiate( poolEntity );
			entity.transform.parent = initialParent;
			entity.gameObject.SetActive( initialActive );
		}

		return entity;
	}

	public override void ReturnEntity( Enemy entity )
	{
		entity.transform.parent = initialParent;
		stack.Push( entity );
	}
#endregion
}