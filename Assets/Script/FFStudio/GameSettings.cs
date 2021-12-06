﻿/* Created by and for usage of FF Studios (2021). */

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
        [ Foldout( "UI Settings" ), Tooltip( "Percentage of the screen to register a swipe"     ) ] public int swipeThreshold;

        [ Foldout( "Enemy" ) ] public int enemy_animation_idle_count;
        [ Foldout( "Enemy" ) ] public int enemy_animation_run_count;
        [ Foldout( "Enemy" ) ] public int enemy_animation_attack_count;
        [ Foldout( "Enemy" ) ] public float enemy_animation_vault_duration;
        [ Foldout( "Enemy" ) ] public float enemy_animation_run_speed;
        [ Foldout( "Enemy" ) ] public float enemy_ragdoll_duration;
        [ Foldout( "Enemy" ) ] public Vector2 enemy_death_velocity_range;
        [ Foldout( "Enemy" ) ] public int enemy_damage = 1;


        [ Foldout( "Window" ) ] public float window_cooldown_vault = 0.25f;
        [ Foldout( "Window" ) ] public Mesh[] window_meshes;
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
