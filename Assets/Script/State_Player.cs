/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class State_Player : StateMachineBehaviour
{
	private Player player;

	override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
		if( player == null )
			player = animator.GetComponentInParent< Player >();

		player.onDamageCooldown = true; 
	}

	override public void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
		if( player == null )
			player = animator.GetComponentInParent< Player >();

		player.onDamageCooldown = false; 
	}
}