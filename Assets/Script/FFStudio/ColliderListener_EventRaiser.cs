/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class ColliderListener_EventRaiser : ColliderListener
	{
#region UnityAPI
		private void OnTriggerEnter( Collider other )
		{
			InvokeEvent( other );
		}
#endregion
	}
}