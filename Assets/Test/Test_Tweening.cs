/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;
using DG.Tweening;

public class Test_Tweening : MonoBehaviour
{
#region Fields
    public AnimationCurve movement_Y_curve;

    public float value = 1;
    public float duration = 1;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        // FFLogger.Log( "Parent: " + transform.parent.name, transform.parent );
    }
#endregion

#region API
    [ Button() ]
    public void Tween_Y()
    {
		transform.DOMoveY( value, duration ).SetEase( movement_Y_curve );
	}

    [ Button() ]
    public void Tween()
    {
		transform.DOMove( value * Vector3.one, duration ).SetEase( movement_Y_curve );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}