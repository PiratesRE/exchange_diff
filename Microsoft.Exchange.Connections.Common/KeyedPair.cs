using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class KeyedPair<T, U> : Pair<T, U>
	{
		public KeyedPair(T first, U second) : base(first, second)
		{
		}

		public override int GetHashCode()
		{
			T first = base.First;
			return first.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			KeyedPair<T, U> keyedPair = obj as KeyedPair<T, U>;
			return keyedPair != null && object.Equals(keyedPair.First, base.First);
		}
	}
}
