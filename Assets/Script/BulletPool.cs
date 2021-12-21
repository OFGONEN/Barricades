/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[CreateAssetMenu( fileName = "pool_bullet", menuName = "FF/Data/Pool/Bullet_Pool" )]
public class BulletPool : ComponentPool< Bullet >
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

	public override Bullet GiveEntity()
	{
		Bullet entity;

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

	public override void ReturnEntity( Bullet entity )
	{
		entity.transform.parent = initialParent;
		stack.Push( entity );
	}
#endregion
}
