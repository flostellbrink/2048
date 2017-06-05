using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace _2048.Core
{
	internal enum Direction { Up, Down, Left, Right }

	/// <summary>
	/// Enum.GetValues is slow. Use this to cache values
	/// </summary>
	internal static class EnumHelper
	{
		private static Dictionary<Type, object> _enumMap = new Dictionary<Type, object>();

		internal static T[] GetValues<T>()
		{
			Debug.Assert(typeof(T).IsEnum);
			if (_enumMap.TryGetValue(typeof(T), out var values))
				return (T[]) values;
			var newValues = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
			_enumMap.Add(typeof(T), newValues);
			return newValues;
		}
	}
}