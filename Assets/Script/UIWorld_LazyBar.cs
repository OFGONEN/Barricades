/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;

public class UIWorld_LazyBar : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public Image[] image_renderer_array;
    [ BoxGroup( "Setup" ) ] public Image image_background; 
    [ BoxGroup( "Setup" ) ] public Image image_foreground; 
    [ BoxGroup( "Shared Variables" ) ] public SharedReferenceNotifier shared_reference_camera;
    [ BoxGroup( "Shared Variables" ) ] public SharedFloatNotifier shared_ratio;

    // Private \\
    private Sequence fillSequence;
    private Sequence fadeSequence;

	private Transform camera_transform;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		shared_ratio.changeEvent += OnRatioChange;
	}

    private void OnDisable()
    {
		shared_ratio.changeEvent -= OnRatioChange;
	}

	private void Awake()
	{
		SetAlphaAmount( 0 );
	}

	private void Start()
	{
		camera_transform = shared_reference_camera.SharedValue as Transform;
	}

	private void Update()
	{
		transform.LookAtAxis( camera_transform.position, Vector3.up );
	}
#endregion

#region API
#endregion

#region Implementation
    private void OnRatioChange()
    {
		fadeSequence = fadeSequence.KillProper();
		fillSequence = fillSequence.KillProper();

		SetAlphaAmount( 1 );

		fillSequence = DOTween.Sequence();
		fillSequence.Append( image_foreground.DOFillAmount( 
            shared_ratio.SharedValue, 
            GameSettings.Instance.ui_Entity_Fade_TweenDuration ) );
		fillSequence.Append( image_background.DOFillAmount( 
            shared_ratio.SharedValue, 
            GameSettings.Instance.ui_Entity_Fade_TweenDuration ) );
		fillSequence.OnComplete( OnFillComplete );
	}

    private void OnFillComplete()
    {
		fillSequence = fillSequence.KillProper();

		var duration = GameSettings.Instance.ui_Entity_Fade_TweenDuration;
		fadeSequence = DOTween.Sequence();

		for( var i = 0; i < image_renderer_array.Length; i++ )
        {
			var image = image_renderer_array[ i ];
			fadeSequence.Join( image.DOFade( 0, duration ) );
		}

		fadeSequence.OnComplete( OnFadeComplete );
	}

    private void SetAlphaAmount( float amount )
    {
        for( var i = 0; i < image_renderer_array.Length; i++ )
        {
			image_renderer_array[ i ].SetAlpha( amount );
		}
    }

    private void OnFadeComplete()
    {
		fadeSequence = fadeSequence.KillProper();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}