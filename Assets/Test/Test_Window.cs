/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Test_Window : MonoBehaviour
{
#region Fields
    public Window targetWindow;
	public DepositType type;

    public int damageCount;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void Test_Deposit()
    {
		targetWindow.GetDeposit( 1, type );
	}

    [ Button() ]
    public void Test_Damage()
    {
		targetWindow.GetDamage( damageCount );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
