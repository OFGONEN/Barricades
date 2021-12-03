/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class Window : MonoBehaviour, IInteractable
{
#region Fields
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public Vector3 GiveDepositPoint()
    {
		return Vector3.zero;
	}

    public void Deposit( int count )     
    {

    }

    public void Damage( int count )
    {
        FFLogger.Log( "Damage: " + count );
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}