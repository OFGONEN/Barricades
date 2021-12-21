/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
    public class ColliderListener_Exit_EventRaiser : ColliderListener
    {
#region UnityAPI
		private void OnTriggerExit( Collider other )
		{
			InvokeEvent( other );
		}
#endregion
    }   
}

