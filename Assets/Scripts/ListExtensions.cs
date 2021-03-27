using System.Collections.Generic;

namespace System.Collections.Generic
{
	public static class ListExtensions
	{
		//Gets a random element from a list
		public static T GetRandom<T>(this List<T> list)
		{
			//Return a random element
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
	}
}

