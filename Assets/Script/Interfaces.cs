/* Created by and for usage of FF Studios (2021). */
using UnityEngine;
using FFStudio;

public interface IInteractable
{
	public abstract Collider GiveHealthCollider();
	public abstract Transform GiveDepositOrigin();
	public abstract void GetDeposit( int count, DepositType type, Collectable collectable = null );
	public abstract void GetDamage( int count );
	public abstract void UpdateHealthRatio();
	public abstract bool IsAlive();
	public abstract void IncomingDeposit();
	public abstract int CanDeposit();
	public abstract void Subscribe_OnDeath( UnityMessage onDeathDelegate );
	public abstract void UnSubscribe_OnDeath( UnityMessage onDeathDelegate );
}