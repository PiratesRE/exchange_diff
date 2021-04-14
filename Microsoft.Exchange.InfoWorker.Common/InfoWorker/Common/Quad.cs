using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class Quad<T, U, V, W>
	{
		internal Quad(T first, U second, V third, W fourth)
		{
			this.First = first;
			this.Second = second;
			this.Third = third;
			this.Fourth = fourth;
		}

		public override int GetHashCode()
		{
			T first = this.First;
			int hashCode = first.GetHashCode();
			U second = this.Second;
			int num = hashCode ^ second.GetHashCode();
			V third = this.Third;
			int num2 = num ^ third.GetHashCode();
			W fourth = this.Fourth;
			return num2 ^ fourth.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Quad<T, U, V, W>)
			{
				Quad<T, U, V, W> quad = (Quad<T, U, V, W>)obj;
				return object.Equals(quad.First, this.First) && object.Equals(quad.Second, this.Second) && object.Equals(quad.Third, this.Third) && object.Equals(quad.Fourth, this.Fourth);
			}
			return false;
		}

		internal readonly T First;

		internal readonly U Second;

		internal readonly V Third;

		internal readonly W Fourth;
	}
}
