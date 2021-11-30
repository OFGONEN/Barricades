/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "RagdollPool", menuName = "FF/Data/Pool/Enemy_Ragdoll" ) ]
public class EnemyRagdollPool : ComponentPool< Enemy_Ragdoll >
{
#region API
	public void InitPool( Transform parent, bool active )
	{
		InitPool();

		foreach( var element in stack )
		{
			element.transform.parent = parent;
			element.gameObject.SetActive( active );
		}
	}
#endregion
}