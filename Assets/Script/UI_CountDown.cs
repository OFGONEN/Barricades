/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

public class UI_CountDown : UIText
{
#region Fields
    [ BoxGroup( "Event Listeners" ) ] public EventListenerDelegateResponse listener_level_start;
    [ BoxGroup( "Setup" ) ] public TextMeshProUGUI countdown_renderer;

    // Private \\
    private int countdown;
    private Tween countdown_tween;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		listener_level_start.OnEnable();
	}

    private void OnDisable()
    {
		listener_level_start.OnDisable();
    }

    private void Awake()
    {
		listener_level_start.response = LevelStartResponse;

		textRenderer.enabled       = false;
		countdown_renderer.enabled = false;
	}
#endregion

#region API
#endregion

#region Implementation
    private void LevelStartResponse()
    {
		uiTransform.localScale = Vector3.zero;

		countdown = CurrentLevelData.Instance.levelData.GetSpawnCountdown();
		countdown_renderer.text = countdown.ToString();

		textRenderer.enabled       = true;
		countdown_renderer.enabled = true;

		Appear().OnComplete( OnAppearComplete );
	}

    private void OnAppearComplete()
    {
		countdown_tween = DOTween.To( () => countdown, x => countdown = x, 0, countdown );
		countdown_tween.SetEase( Ease.Linear );
		countdown_tween.OnUpdate( OnCountDownUpdate );
		countdown_tween.OnComplete( OnCountDownComplete );
    }


    private void OnCountDownUpdate()
    {
		countdown_renderer.text = countdown.ToString();
    }

    private void OnCountDownComplete()
    {
		countdown_tween = countdown_tween.KillProper();

		textRenderer.enabled       = true;
		countdown_renderer.enabled = true;

		Disappear();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}