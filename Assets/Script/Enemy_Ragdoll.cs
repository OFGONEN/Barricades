/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
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

    // Private Fields
    private Tween returnToPoolTween;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public void Spawn()
    {
		returnToPoolTween.KillProper();
		returnToPoolTween = DOVirtual.DelayedCall( GameSettings.Instance.enemy_ragdoll_duration, ReturnToPool );
	}
#endregion

#region Implementation
    private void ReturnToPool()
    {
		gameObject.SetActive( false );
		enemyRagdollPool.ReturnEntity( this );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}