using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Pair<T, U>
	{
		internal Pair(T first, U second)
		{
			this.First = first;
			this.Second = second;
		}

		public override int GetHashCode()
		{
			T first = this.First;
			int hashCode = first.GetHashCode();
			U second = this.Second;
			return hashCode ^ second.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Pair<T, U>)
			{
				Pair<T, U> pair = (Pair<T, U>)obj;
				return object.Equals(pair.First, this.First) && object.Equals(pair.Second, this.Second);
			}
			return false;
		}

		internal readonly T First;

		internal readonly U Second;
	}
}
