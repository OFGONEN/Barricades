/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "ParticlePool", menuName = "FF/Data/Pool/ParticlePool" ) ]
	public class ParticleEffectPool : ComponentPool< ParticleEffect >
	{
		private Transform initialParent;
		private bool initialActive;
#region API
		public void InitPool( Transform parent, bool active, ParticleEffectStopped effectStoppedDelegate )
		{
			initialParent = parent;
			initialActive = active;

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
				entity.transform.parent = initialParent;
				entity.gameObject.SetActive( initialActive );
			}

			return entity;
		}

		public override void ReturnEntity( ParticleEffect entity )
		{
			entity.transform.parent = initialParent;
			stack.Push( entity );
		}
#endregion
	}
}