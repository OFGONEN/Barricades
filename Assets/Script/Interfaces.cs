/* Created by and for usage of FF Studios (2021). */
using UnityEngine;
using FFStudio;

public interface IInteractable
{
	public abstract Collider GiveHealthCollider();
	public abstract Vector3 GiveDepositPoint();
	public abstract void GetDeposit( int count, DepositType type );
	public abstract void GetDamage( int count );
	public abstract bool IsAlive();
	public abstract void Subscribe_OnDeath( UnityMessage onDeathDelegate );
}