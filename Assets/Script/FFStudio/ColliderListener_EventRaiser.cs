/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class ColliderListener_EventRaiser : MonoBehaviour
	{
#region Fields
		public event ColliderTrigger triggerEnter;
#endregion

#region UnityAPI
		private void OnTriggerEnter( Collider other )
		{
			triggerEnter?.Invoke( other );
		}
#endregion
	}
}