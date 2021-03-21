using System;
using UnityEngine;

/// <summary>
/// Represents a configured drop
/// </summary>
[Serializable]
public struct DropChance
{
	/// <summary>
	/// The drop to be instantiated
	/// </summary>
	public GameObject Drop;
	/// <summary>
	/// How often the drop can occur
	/// </summary>
	public float Chance;
}

