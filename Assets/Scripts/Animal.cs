using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class Animal : MonoBehaviour
{
	[SerializeField]
	public char gender = 'M';

	[SerializeField]
	protected string foodObjectTag;
	[SerializeField]
	protected float speed = 1f;
	[SerializeField]
	protected float sensoryRadius = 6f;
	[SerializeField]
	protected float maxWeight = 5f;
	[SerializeField]
	protected float maxLifetime = 500f; //lifetime is measured in seconds
	[SerializeField]
	protected float stamina = 80f;
	[SerializeField]
	protected float hungerModifier = 1f;
	[SerializeField]
	protected float thirstModifier = 1f;
	[SerializeField]
	protected float reproductiveUrgeRate = 1f;

	protected float birthWeight = 1f;
	protected float finalWeight = 5f;
	protected float growingTime = 60f;
	protected float currentWeight = 1f;
	protected float currentHunger = 0f;
	protected float currentThirst = 0f;
	protected float currentLifetime = 300f;
	protected float currentReproductiveUrge = 0f;
	protected Vector3 foodLocation = Vector3.down * 10f;
	protected Vector3 waterLocation;
	protected Seeker seeker;
	protected float age = 0f;

	private bool hasTarget = false;
	private Target target = Target.None;
	private Path path;
	private int currentWaypoint = 0;
	private float sqDistToNextWaypoint = 100f;
	private Vector3 pathTarget = Vector3.down * 10f;

	public virtual void Start()
	{
		seeker = gameObject.GetComponent<Seeker>();
	}

	public virtual void Update()
	{
		#region Urges and growing
		age += Time.deltaTime;

		if (age > currentLifetime) {
			DoDeath();
		}

		currentHunger += currentWeight * speed * speed * hungerModifier * Time.deltaTime;
		currentThirst += currentWeight * speed * speed * thirstModifier * Time.deltaTime;
		if (currentWeight >= finalWeight) {
			currentReproductiveUrge += reproductiveUrgeRate * Time.deltaTime;
		}
		currentWeight += (finalWeight - birthWeight) * Time.deltaTime / growingTime;
		if (currentWeight >= finalWeight) {
			currentWeight = finalWeight;
		}
		#endregion

		#region Vision
		DoVision();
		#endregion

		#region Motion
		if (!HasMotionOverrides()) {
			if (hasTarget) {
				if (target == Target.Food) {
					DoEating();
				}
				else if (target == Target.Water) {
					DoDrinking();
				}
				else if (target == Target.Mate) {
					DoFindMate();
				}
			}
			else {
				if (currentReproductiveUrge > currentHunger && currentReproductiveUrge > currentThirst) {
					target = Target.Mate;
				}
				else if (currentHunger > currentThirst) {
					target = Target.Food;
				}
				else {
					target = Target.Water;
				}
				hasTarget = true;
			}
		}
		#endregion
	}

	public virtual void DoFindMate()
	{

	}

	public virtual void DoEating()
	{
		Debug.Log("EATING");
		if (foodLocation.y >= 0f) {
			if (path == null) {
				seeker.StartPath(transform.position, foodLocation, OnPathComplete);
			}

			if (path.error) {
				return;
			}
			else {
				currentWaypoint = 0;
			}

			bool reachedEndOfPath = false;
			while (true) {
				float sqDistToWaypoint = (transform.position - path.vectorPath[currentWaypoint]).sqrMagnitude;
				if (sqDistToWaypoint < sqDistToNextWaypoint) {
					if (currentWaypoint + 1 < path.vectorPath.Count) {
						currentWaypoint++;
					}
					else {
						reachedEndOfPath = true;
						break;
					}
				}
				else {
					break;
				}
			}

			if (reachedEndOfPath) {
				path = null;
			}
		}
		currentHunger = 0f;
	}

	public virtual void DoDrinking()
	{
		Debug.Log("DRINKING");
		currentHunger = 0f;
	}

	public virtual void DoOutOfStamina()
	{

	}

	public virtual bool HasMotionOverrides()
	{
		return false;
	}

	public virtual void DoVision()
	{
		if (foodLocation.y > 0 && (foodLocation - transform.position).sqrMagnitude > sensoryRadius * sensoryRadius) {
			foodLocation.y = -10f;
		}
		if (waterLocation.y > 0 && (waterLocation - transform.position).sqrMagnitude > sensoryRadius * sensoryRadius) {
			waterLocation.y = -10f;
		}

		Collider[] objectsInSight = Physics.OverlapSphere(transform.position, sensoryRadius);

		foreach (Collider col in objectsInSight) {
			if (col.CompareTag(foodObjectTag)) {
				if (foodLocation.y < 0 || (foodLocation - transform.position).sqrMagnitude > (col.transform.position - transform.position).sqrMagnitude) {
					foodLocation = col.transform.position;
				}
			}
			else if (col.CompareTag("Water")) {
				if (waterLocation.y < 0 || (waterLocation - transform.position).sqrMagnitude > (col.transform.position + Vector3.up - transform.position).sqrMagnitude) {
					waterLocation = col.transform.position + Vector3.up;
				}
			}
		}
	}

	public virtual void DoDeath()
	{
		Debug.Log("DEAD");
		Destroy(gameObject);
	}

	

	private void OnPathComplete(Path p)
	{
		if (p.error) {
			Debug.Log("Path had error!");
			path = null;
		}
		else {
			path = p;
		}
	}
}

[Serializable]
public enum Target
{
	Food,
	Water,
	Mate,
	None,
}