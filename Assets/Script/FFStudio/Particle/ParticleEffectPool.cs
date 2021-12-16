/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "ParticlePool", menuName = "FF/Data/Pool/ParticlePool" ) ]
	public class ParticleEffectPool : ComponentPool< ParticleEffect >
	{
		private Transform initial_Parent;
		private bool initial_Active;
		private ParticleEffectStopped initial_Delegate;
#region API
		public void InitPool( Transform parent, bool active, ParticleEffectStopped effectStoppedDelegate )
		{
			initial_Parent   = parent;
			initial_Active   = active;
			initial_Delegate = effectStoppedDelegate;

			InitPool();

			foreach( var element in stack )
			{
				element.InitIntoPool( parent, active, effectStoppedDelegate );
			}
		}

		public override ParticleEffect GiveEntity()
		{
			ParticleEffect entity;

			if( stack.Count > 0 )
				entity = stack.Pop();
			else
			{
				entity = GameObject.Instantiate( poolEntity );
				entity.InitIntoPool( initial_Parent, initial_Active, initial_Delegate );
			}

			return entity;
		}

		public override void ReturnEntity( ParticleEffect entity )
		{
			entity.transform.parent = initial_Parent;
			stack.Push( entity );
		}
#endregion
	}
}