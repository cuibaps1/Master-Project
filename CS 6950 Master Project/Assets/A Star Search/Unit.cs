﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


public class Unit : MonoBehaviour
{


	public Transform target, target1;
	float speed = 20;
	Vector3[] path;
	int targetIndex;

	void Start()
	{
		StartCoroutine(DoMoving());
		//PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
		//PathRequestManager.RequestPath(transform.position, target1.position, OnPathFound);
	}

	IEnumerator DoMoving()
	{
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
		yield return new WaitForSeconds(5f);

		PathRequestManager.RequestPath(transform.position, target1.position, OnPathFound);
		yield return new WaitForSeconds(2f);

	}


	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{
		if (pathSuccessful)
		{
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0];
		while (true)
		{
			if (transform.position == currentWaypoint)
			{
				targetIndex++;
				if (targetIndex >= path.Length)
				{
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;

		}
	}

	public void OnDrawGizmos()
	{
		if (path != null)
		{
			for (int i = targetIndex; i < path.Length; i++)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if (i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
				{
					Gizmos.DrawLine(path[i - 1], path[i]);
				}
			}
		}
	}
}