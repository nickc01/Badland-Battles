using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Used for starting coroutines that aren't bound to a specific object. This means the routines will continue to run even if the object is destroyed
/// </summary>
public class UnboundRoutine : MonoBehaviour
{
	//The object for running the unbound routines
	static UnboundRoutine _routineRunner;

	/// <summary>
	/// The object for running the routines. Creates one if one does not exist already
	/// </summary>
	public static UnboundRoutine RoutineRunner
	{
		get
		{
			if (_routineRunner == null)
			{
				_routineRunner = new GameObject("Unbound Routine Runner").AddComponent<UnboundRoutine>();
				_routineRunner.gameObject.hideFlags = HideFlags.HideInHierarchy;
				GameObject.DontDestroyOnLoad(_routineRunner.gameObject);
			}
			return _routineRunner;
		}
	}

	/// <summary>
	/// Starts an unbound routine
	/// </summary>
	/// <param name="routine">The routine to run</param>
	/// <returns>The coroutine that represents the routine being run</returns>
	public static Coroutine StartUnboundRoutine(IEnumerator routine)
	{
		return RoutineRunner.StartCoroutine(routine);
	}

	/// <summary>
	/// Stops an unbound routine
	/// </summary>
	/// <param name="routine">The routine to stop</param>
	public static void StopUnboundRoutine(Coroutine routine)
	{
		RoutineRunner.StopCoroutine(routine);
	}
}
