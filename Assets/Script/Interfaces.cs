/* Created by and for usage of FF Studios (2021). */
using UnityEngine;
using FFStudio;

public interface IInteractable
{
	public abstract Collider GiveHealthCollider();
	public abstract Vector3 GiveDepositPoint();
	public abstract void Deposit( int count );
	public abstract void Damage( int count );
	public abstract bool IsAlive();
	public abstract void Subscribe_OnDeath( UnityMessage onDeathDelegate );
}