﻿/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Lean.Touch;

namespace FFStudio
{
    public class InputManager : MonoBehaviour
    {
#region Fields
		[ Header( "Fired Events" ) ]
		public SwipeInputEvent swipeInputEvent;
		public ScreenPressEvent screenPressEvent;
		public IntGameEvent tapInputEvent;

		[ Header( "Shared Variables" ) ]
		public SharedReferenceNotifier mainCamera_ReferenceNotifier;
		public SharedInput_JoyStick sharedInput_JoyStick;

		// Privat fields
		private int swipeThreshold;

		// Delegates
		private LeanFingerDelegate leanFingerUpdate;
		private LeanFingerDelegate leanFingerUp;

		// Components
		private Transform mainCamera_Transform;
		private Camera mainCamera;
		private LeanTouch leanTouch;
#endregion

#region Unity API
		private void OnEnable()
		{
			mainCamera_ReferenceNotifier.changeEvent += OnCameraReferenceChange;
		}

		private void OnDisable()
		{
			mainCamera_ReferenceNotifier.changeEvent -= OnCameraReferenceChange;
		}

		private void Awake()
		{
			swipeThreshold = Screen.width * GameSettings.Instance.swipeThreshold / 100;

			leanTouch         = GetComponent<LeanTouch>();
			leanTouch.enabled = false;

			leanFingerUpdate = FingerDown;
			leanFingerUp     = FingerUp;
		}
#endregion
		
#region API
		public void Swiped( Vector2 delta )
		{
			swipeInputEvent.ReceiveInput( delta );
		}
		
		public void Tapped( int count )
		{
			tapInputEvent.eventValue = count;

			tapInputEvent.Raise();
		}

		public void LeanFingerUpdate( LeanFinger finger )
		{
			leanFingerUpdate( finger );
		}

		public void LeanFingerUp( LeanFinger finger )
		{
			leanFingerUp( finger );
		}
#endregion

#region Implementation
		private void OnCameraReferenceChange()
		{
			var value = mainCamera_ReferenceNotifier.SharedValue;

			if( value == null )
			{
				mainCamera_Transform = null;
				leanTouch.enabled = false;
			}
			else 
			{
				mainCamera_Transform = value as Transform;
				mainCamera           = mainCamera_Transform.GetComponent< Camera >();
				leanTouch.enabled    = true;
			}
		}

		private void FingerDown( LeanFinger finger )
		{
			sharedInput_JoyStick.Enable( finger.ScreenPosition );
			leanFingerUpdate = FingerUpdate;
		}

		private void FingerUpdate( LeanFinger finger )
		{
			sharedInput_JoyStick.ReceiveInput( finger.ScreenPosition );
		}

		private void FingerUp( LeanFinger finger )
		{
			sharedInput_JoyStick.Disable( finger.ScreenPosition );
			leanFingerUpdate = FingerDown;
		}
#endregion
    }
}