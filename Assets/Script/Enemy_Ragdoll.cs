/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Enemy_Ragdoll : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared Variables" ), SerializeField ] private EnemyRagdollPool enemyRagdollPool;
    [ BoxGroup( "Setup" ), SerializeField ] private Transform rootBone;
    [ BoxGroup( "Setup" ), SerializeField ] private Rigidbody rootRigidbody;

    // Public Properties
    public Transform RootBone => rootBone;
    public Rigidbody RootRigidbody => rootRigidbody;
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