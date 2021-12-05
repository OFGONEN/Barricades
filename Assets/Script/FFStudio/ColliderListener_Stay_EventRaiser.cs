/* Created by and for usage of FF Studios (2022). */

using UnityEngine;

namespace FFStudio
{
    public class ColliderListener_Stay_EventRaiser : MonoBehaviour
    {
#region Fields
        public event ColliderTrigger triggerStay;
#endregion

#region UnityAPI
        private void OnTriggerStay( Collider other )
        {
            triggerStay?.Invoke(other);
        }
#endregion
    }
}