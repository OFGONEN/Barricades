/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class UIText_Int : UIText
{
#region Fields
    public SharedIntNotifier notifier_int;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		notifier_int.changeEvent += OnValueChange;
	}

    private void OnDisable()
    {
		notifier_int.changeEvent -= OnValueChange;
    }
#endregion

#region API
#endregion

#region Implementation
    private void OnValueChange()
    {
		textRenderer.text = notifier_int.SharedValue.ToString();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}