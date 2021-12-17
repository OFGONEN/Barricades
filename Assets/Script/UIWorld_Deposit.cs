/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

public class UIWorld_Deposit : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public TextMeshProUGUI deposit_text;
    [ BoxGroup( "Setup" ) ] public Image deposit_image;

    private Tween deposit_tween;
    private int deposit_maxValue;
	private float deposit_ratio;
	private float deposit_duration;

	private StringBuilder stringBuilder;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		stringBuilder = new StringBuilder( 8 );
		deposit_duration = GameSettings.Instance.ui_Entity_Move_TweenDuration;
	}
#endregion

#region API
    public void Init( int curentValue, int maxValue )
    {
		deposit_maxValue         = maxValue;
		deposit_text.text        = GiveDepositText( curentValue );
		deposit_ratio            = curentValue / ( float )maxValue;
		deposit_image.fillAmount = deposit_ratio;
	}

    public void SetText( string text )
    {
		deposit_text.text = text;
	}

    public void SetValue( int newValue )
    {
		deposit_tween = deposit_tween.KillProper();
		deposit_tween = DOTween.To( () => deposit_ratio, x => deposit_ratio = x, newValue / ( float )deposit_maxValue, deposit_duration );

		deposit_tween.OnUpdate( OnDepositRatioUpdate );
		deposit_tween.OnComplete( OnDepositRatioComplete );
	}
#endregion

#region Implementation
    private string GiveDepositText( int currentValue )
    {
		stringBuilder.Clear();
		stringBuilder.Append( currentValue );
		stringBuilder.Append( '/' );
		stringBuilder.Append( deposit_maxValue );

		return stringBuilder.ToString();
	}

    private void OnDepositRatioUpdate()
    {
		deposit_image.fillAmount = deposit_ratio;
	}

    private void OnDepositRatioComplete()
    {
		deposit_tween = deposit_tween.KillProper();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}