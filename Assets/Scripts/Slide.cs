using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The name for the slide")]
	string slideName;

	/// <summary>
	/// The name for the slide
	/// </summary>
	public string SlideName => slideName;
}
