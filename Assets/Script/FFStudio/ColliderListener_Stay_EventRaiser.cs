/* Created by and for usage of FF Studios (2022). */

using UnityEngine;

namespace FFStudio
{
    public class ColliderListener_Stay_EventRaiser : ColliderListener
    {
#region UnityAPI
        private void OnTriggerStay( Collider other )
        {
			InvokeEvent( other );
		}
#endregion
    }
}