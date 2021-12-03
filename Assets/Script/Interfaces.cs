/* Created by and for usage of FF Studios (2021). */
using UnityEngine;

public interface IInteractable
{
	public abstract Collider GiveHealthCollider();
	public abstract Vector3 GiveDepositPoint();
	public abstract void Deposit( int count );
	public abstract void Damage( int count );
}