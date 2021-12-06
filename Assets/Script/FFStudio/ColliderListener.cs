/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public abstract class ColliderListener : MonoBehaviour
	{
#region Fields
		public event ColliderTrigger triggerEvent;

		// Private
        [ SerializeField ] private Component attachedComponent;
		private Collider attachedCollider;

		// Public Properties
		public Component AttachedComponent => attachedComponent;
		public Collider AttachedCollider   => attachedCollider;
#endregion

#region UnityAPI
		private void Awake()
		{
			attachedCollider = GetComponent< Collider >();
		}
#endregion

#region API
		public void ClearEventList()
		{
			triggerEvent = null;
		}

		public void SetColliderActive( bool active )
		{
			attachedCollider.enabled = active;
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
