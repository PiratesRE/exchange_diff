using System;

namespace System.Collections.Generic
{
	[Serializable]
	internal class ObjectComparer<T> : Comparer<T>
	{
		public override int Compare(T x, T y)
		{
			return Comparer.Default.Compare(x, y);
		}

		public override bool Equals(object obj)
		{
			ObjectComparer<T> objectComparer = obj as ObjectComparer<T>;
			return objectComparer != null;
		}

		public override int GetHashCode()
		{
			return base.GetType().Name.GetHashCode();
		}
	}
}
