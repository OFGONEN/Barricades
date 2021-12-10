/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Collectable : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared Variables" ) ] public CollectablePool collectablePool;

    [ BoxGroup( "Setup" ) ] public DepositType depositType;
    [ BoxGroup( "Setup" ) ] public ColliderListener_EventRaiser colliderListener_Seek_Enter;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
