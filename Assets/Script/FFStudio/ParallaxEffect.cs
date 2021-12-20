/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class ParallaxEffect : MonoBehaviour
	{
#region Fields
		public EventListenerDelegateResponse level_start_listener;

		public SharedReferenceNotifier targetReference;
		// public Vector3 parallax_ratio;
		public Vector3 parallax_offset;
		public float parallax_speed;

		/* Private Fields */
		private Transform targetTransform;
		private Vector3 startPosition;
		private Vector3 target_StartPosition;

		private UnityMessage updateMethod;
#endregion

#region Unity API
		private void OnEnable()
		{
			level_start_listener.OnEnable();
		}

		private void OnDisable()
		{
			level_start_listener.OnDisable();

			updateMethod = ExtensionMethods.EmptyMethod;
		}

		private void Awake()
		{
			updateMethod = ExtensionMethods.EmptyMethod;

			level_start_listener.response = LevelStartResponse;
		}

		private void Start()
		{
			targetTransform = targetReference.SharedValue as Transform;
		}

		private void Update()
		{
			updateMethod();
		}
#endregion

#region API
#endregion

#region Implementation
		private void OnUpdate()
		{
			transform.position = Vector3.MoveTowards( transform.position, targetTransform.position + parallax_offset, Time.deltaTime * parallax_speed );
		}

		private void LevelStartResponse()
		{
			updateMethod = OnUpdate;
		}
#endregion
	}
}