/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;

/* This class holds references to ScriptableObject assets. These ScriptableObjects are singletons, so they need to load before a Scene does.
 * Using this class ensures at least one script from a scene holds a reference to these important ScriptableObjects. */
public class AppAssetHolder : MonoBehaviour
{
#region Fields
	public GameSettings gameSettings;
	public CurrentLevelData currentLevelData;

	[ BoxGroup( "Pools" ) ] public EnemyRagdollPool enemyRagdollPool;
	[ BoxGroup( "Pools" ) ] public EnemyPool enemyPool;
	[ BoxGroup( "Pools" ) ] public BulletPool bulletPool;
	[ BoxGroup( "Pools" ) ] public CollectablePool collectable_wood_Pool;
	[ BoxGroup( "Pools" ) ] public CollectablePool collectable_metal_Pool;
	[ BoxGroup( "Pools" ) ] public CollectablePool collectable_gold_Pool;

	[ BoxGroup( "Material" ) ] public Material material_transparent;

	private void Awake()
	{
		//Init Pools
		enemyRagdollPool      .InitPool( transform, false );
		enemyPool             .InitPool( transform, false );
		bulletPool            .InitPool( transform, false );
		collectable_wood_Pool .InitPool( transform, false );
		collectable_metal_Pool.InitPool( transform, false );
		collectable_gold_Pool .InitPool( transform, false );

		material_transparent.color = material_transparent.color.SetAlpha( GameSettings.Instance.asset_material_transparent_start );
		material_transparent.DOFade( GameSettings.Instance.asset_material_transparent_end, GameSettings.Instance.asset_material_transparent_duration ).SetLoops( -1, LoopType.Yoyo );
	}
#endregion
}
