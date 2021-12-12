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

		[ Header( "Fired Events" ) ]
        public GameEvent levelFailedEvent;
        public GameEvent levelCompleted;

        [ Header( "Level Releated" ) ]
        public SharedFloatNotifier levelProgress;
        public SharedReferenceNotifier level_destination_outside;

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
		}

        private void OnDisable()
        {
            levelLoadedListener.OnDisable();
            levelRevealedListener.OnDisable();
            levelStartedListener.OnDisable();
			listener_level_finished.OnDisable();
        }

        private void Awake()
        {
            levelLoadedListener.response     = LevelLoadedResponse;
            levelRevealedListener.response   = LevelRevealedResponse;
            levelStartedListener.response    = LevelStartedResponse;
            listener_level_finished.response = LevelFinishedResponse;

			updateMethod = ExtensionMethods.EmptyMethod;
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

        }

        private void LevelStartedResponse()
        {
			level_volume = ( level_destination_outside.SharedValue as Transform ).GetComponent< LevelVolume >();
			collectable_spawn_max = CurrentLevelData.Instance.levelData.collectable_spawn_max;
			updateMethod = SpawnCollectable;
		}
        
        private void LevelFinishedResponse()
        {
			updateMethod = ExtensionMethods.EmptyMethod;
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
			collectable.Spawn( position );
		}
#endregion
    }
}