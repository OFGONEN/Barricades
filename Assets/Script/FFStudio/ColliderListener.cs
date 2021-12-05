/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public abstract class ColliderListener : MonoBehaviour
	{
#region Fields
        public Component attachedComponent;
		public event ColliderTrigger triggerEvent;
#endregion

#region UnityAPI
#endregion

#region API
		public void ClearEventList()
		{
			triggerEvent = null;
		}
#endregion

#region Implementation
        protected void InvokeEvent( Collider other )
        {
			triggerEvent?.Invoke( other );
		}
#endregion
	}
}
