/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "RagdollPool", menuName = "FF/Data/Pool/Enemy_Ragdoll" ) ]
public class EnemyRagdollPool : FixedComponentPool< Enemy_Ragdoll >
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

	public override void ReturnEntity( Enemy_Ragdoll entity )
	{
		entity.transform.parent = initialParent;
		base.ReturnEntity( entity);
	}


	public override Enemy_Ragdoll GiveEntity()
	{
		Enemy_Ragdoll entity;

		if( stack.Count > 0 )
		{
			entity = stack.Pop();
			activeEntities.Add( entity );
		}
		else if( activeEntities.Count > 0 )
		{
			entity = activeEntities[ 0 ];
			activeEntities.RemoveAt( 0 );
			activeEntities.Add( entity );
		}
		else
		{
			entity = GameObject.Instantiate( poolEntity );
			entity.transform.parent = initialParent;
			entity.gameObject.SetActive( initialActive );
		}

		return entity;
	}
#endregion
}