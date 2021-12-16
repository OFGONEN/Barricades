/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
#region Fields
        [ Header( "Event Listeners" ) ]
        public EventListenerDelegateResponse levelLoadedListener;
        public EventListenerDelegateResponse levelRevealedListener;
        public EventListenerDelegateResponse levelStartedListener;
		public MultipleEventListenerDelegateResponse listener_level_finished;
		public EventListenerDelegateResponse listener_enemy_death;

		[ Header( "Fired Events" ) ]
        public GameEvent levelFailedEvent;
        public GameEvent levelCompleted;

        [ Header( "Level Releated" ) ]
        public SharedFloatNotifier levelProgress;
        public SharedReferenceNotifier level_destination_outside;
		public SharedInt enemy_count;
		public SharedIntNotifier enemy_count_remaining;

		[ BoxGroup( "Pool" ) ] public CollectablePool collectable_pool;


        // Private \\
        private UnityMessage updateMethod;

		private LevelVolume level_volume;
		private float collectable_spawn_cooldown;
		private int collectable_spawn_max;
		private int collectable_spawn_area = 1 << 3;
#endregion

#region UnityAPI
        private void OnEnable()
        {
            levelLoadedListener.OnEnable();
            levelRevealedListener.OnEnable();
            levelStartedListener.OnEnable();
			listener_level_finished.OnEnable();
			listener_enemy_death.OnEnable();
		}

        private void OnDisable()
        {
            levelLoadedListener.OnDisable();
            levelRevealedListener.OnDisable();
            levelStartedListener.OnDisable();
			listener_level_finished.OnDisable();
			listener_enemy_death.OnDisable();
        }

        private void Awake()
        {
            levelLoadedListener.response     = LevelLoadedResponse;
            levelRevealedListener.response   = LevelRevealedResponse;
            levelStartedListener.response    = LevelStartedResponse;
            listener_level_finished.response = LevelFinishedResponse;
            listener_enemy_death.response    = EnemyDeathResponse;

			updateMethod = ExtensionMethods.EmptyMethod;

			enemy_count.sharedValue           = 0;
			enemy_count_remaining.SetValue_DontNotify( 0 );
		}

        private void Update()
        {
			updateMethod();
		}
#endregion

#region Implementation
        private void LevelLoadedResponse()
        {
			levelProgress.SharedValue = 0;

			var levelData = CurrentLevelData.Instance.levelData;

            // Set Active Scene
			if( levelData.overrideAsActiveScene )
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 1 ) );
            else
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 0 ) );
		}

        private void LevelRevealedResponse()
        {
			levelStartedListener.gameEvent.Raise();
		}

        private void LevelStartedResponse()
        {
			// Enemy Count
			enemy_count_remaining.SetValue_NotifyAlways( enemy_count.sharedValue );
			levelProgress.SharedValue         = 1;


			// Configure Spawning Collectable
			level_volume          = ( level_destination_outside.SharedValue as Transform ).GetComponentInParent< LevelVolume >();
			collectable_spawn_max = CurrentLevelData.Instance.levelData.collectable_spawn_max;
			updateMethod          = SpawnCollectable;
		}
        
        private void LevelFinishedResponse()
        {
			enemy_count.sharedValue           = 0;

			updateMethod = ExtensionMethods.EmptyMethod;
		}

        private void EnemyDeathResponse()
        {
			enemy_count_remaining.SetValue_NotifyAlways( enemy_count_remaining.SharedValue - 1);
			levelProgress.SharedValue = enemy_count_remaining.SharedValue / (float)enemy_count.sharedValue;

			if( enemy_count_remaining.SharedValue <= 0 )
				levelCompleted.Raise();
		}

        private void SpawnCollectable()
        {
            if( collectable_spawn_cooldown > Time.time || collectable_pool.ActiveCount >= collectable_spawn_max ) return;

			NavMeshHit navMeshHit;
			Vector3 position = Vector3.zero;
			bool loop = true;
			int  area = 1 << 3;

			while( loop )
            {
				NavMesh.SamplePosition( level_volume.GiveRandomPosition(), out navMeshHit, 0.5f, area );
				loop     = !navMeshHit.hit;
				position = navMeshHit.position;
			}

			collectable_spawn_cooldown = Time.time + CurrentLevelData.Instance.levelData.collectable_spawn_rate;
			var collectable = collectable_pool.GiveEntity();
			position.x = position.x.RoundTo( GameSettings.Instance.collectable_spawn_grid );
			position.z = position.z.RoundTo( GameSettings.Instance.collectable_spawn_grid );
			collectable.Spawn( position );
		}
#endregion
    }
}