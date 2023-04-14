// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global


using System;


namespace BlockPuzzle.Scripts.Runtime.extensions {
	public static class ArrayExtensions {
		public static T[] Fill<T> (this T[] self, T with) where T : struct {
			if (self == null)
				throw new NullReferenceException(nameof(self));

			for (int i = 0; i < self.Length; i++)
				self[i] = with;

			return self;
		}
	}
}
