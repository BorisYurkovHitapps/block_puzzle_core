// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global


using System;
using System.Collections.Generic;


namespace BlockPuzzle.Scripts.Runtime.extensions {
	public static class EnumerableExtensions {
		public static void ForEach<T> (this IEnumerable <T> enumerable, Action <T> action) {
			foreach (T item in enumerable)
				action(item);
		}
	}
}
