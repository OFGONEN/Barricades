/* Created by and for usage of FF Studios (2021). */

using NaughtyAttributes;
using UnityEngine;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
#region Singleton Related
        private static GameSettings instance;

        private delegate GameSettings ReturnGameSettings();
        private static ReturnGameSettings returnInstance = LoadInstance;

		public static GameSettings Instance => returnInstance();
#endregion
        
#region Fields
        [ BoxGroup( "Remote Config" ) ] public bool useRemoveConfig_GameSettings;
        [ BoxGroup( "Remote Config" ) ] public bool useRemoveConfig_Components;

        public int maxLevelCount;
        [ Foldout( "UI Settings" ), Tooltip( "Duration of the movement for ui element"          ) ] public float ui_Entity_Move_TweenDuration;
        [ Foldout( "UI Settings" ), Tooltip( "Duration of the fading for ui element"            ) ] public float ui_Entity_Fade_TweenDuration;
		[ Foldout( "UI Settings" ), Tooltip( "Duration of the scaling for ui element"           ) ] public float ui_Entity_Scale_TweenDuration;
		[ Foldout( "UI Settings" ), Tooltip( "Duration of the movement for floating ui element" ) ] public float ui_Entity_FloatingMove_TweenDuration;
		[ Foldout( "UI Settings" ), Tooltip( "Joy Stick"                                        ) ] public float ui_Entity_JoyStick_Gap;
        [ Foldout( "UI Settings" ), Tooltip( "Percentage of the screen to register a swipe"     ) ] public int swipeThreshold;

        [ Foldout( "Enemy" ) ] public int enemy_animation_idle_count;
        [ Foldout( "Enemy" ) ] public int enemy_animation_run_count;
        [ Foldout( "Enemy" ) ] public int enemy_animation_attack_count;
        [ Foldout( "Enemy" ) ] public float enemy_animation_vault_duration;
        [ Foldout( "Enemy" ) ] public float enemy_animation_run_speed;
        [ Foldout( "Enemy" ) ] public float enemy_ragdoll_duration;
        [ Foldout( "Enemy" ) ] public Vector2 enemy_death_velocity_range;
        [ Foldout( "Enemy" ) ] public float enemy_distance_targetFollow = 0.25f;
        [ Foldout( "Enemy" ) ] public int enemy_damage = 1;

        // Window Entity
        [ Foldout( "Window" ) ] public float window_cooldown_vault = 0.25f;
        [ Foldout( "Window" ) ] public Mesh[] window_meshes;

        // Spike Entity
        [ Foldout( "Spike" ) ] public int spike_maxHealth = 6;

        // Bullet Entity

        // Turret Entity
        [ Foldout( "Turret" ) ] public int turret_maxHealth = 6;
        [ Foldout( "Turret" ) ] public float turret_bullet_fireRate = 0.15f;
        [ Foldout( "Turret" ) ] public float turret_bullet_speed = 6;

        // Player Entity
        [ Foldout( "Player" ) ] public int player_max_health = 6;
        [ Foldout( "Player" ) ] public int player_max_collectable = 12;
        [ Foldout( "Player" ) ] public float player_duration_deposit = 0.1f;

        // Collectable Entity
        [ Foldout( "Collectable" ) ] public AnimationCurve collectable_ease;
        [ Foldout( "Collectable" ) ] public AnimationCurve collectable_ease_reverse;
        [ Foldout( "Collectable" ) ] public int collectable_stack_height = 5;
        [ Foldout( "Collectable" ) ] public float collectable_duration_deposit = 0.25f;
        [ Foldout( "Collectable" ) ] public float collectable_delay_deposit = 0.2f;
        [ Foldout( "Collectable" ) ] public float collectable_random_deposit = 1f;

        // Guard
        [ Foldout( "Guard" ) ] public float guard_shoot_height = 1;
        [ Foldout( "Guard" ) ] public float guard_shoot_rate = 0.25f;
        [ Foldout( "Guard" ) ] public float guard_bullet_speed = 6;
#endregion

#region Implementation
        private static GameSettings LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< GameSettings >( "game_settings" );

			returnInstance = ReturnInstance;

			return instance;
		}

		private static GameSettings ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}
