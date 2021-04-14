using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class Triplet<T, U, V>
	{
		internal Triplet(T first, U second, V third)
		{
			this.First = first;
			this.Second = second;
			this.Third = third;
		}

		public override int GetHashCode()
		{
			T first = this.First;
			int hashCode = first.GetHashCode();
			U second = this.Second;
			int num = hashCode ^ second.GetHashCode();
			V third = this.Third;
			return num ^ third.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Triplet<T, U, V>)
			{
				Triplet<T, U, V> triplet = (Triplet<T, U, V>)obj;
				return object.Equals(triplet.First, this.First) && object.Equals(triplet.Second, this.Second) && object.Equals(triplet.Third, this.Third);
			}
			return false;
		}

		internal readonly T First;

		internal readonly U Second;

		internal readonly V Third;
	}
}
