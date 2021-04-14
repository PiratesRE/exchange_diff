using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class Pair<T, U>
	{
		public Pair(T first, U second)
		{
			this.first = first;
			this.second = second;
		}

		public T First
		{
			get
			{
				return this.first;
			}
		}

		public U Second
		{
			get
			{
				return this.second;
			}
		}

		public override int GetHashCode()
		{
			T t = this.first;
			int hashCode = t.GetHashCode();
			U u = this.second;
			return hashCode ^ u.GetHashCode();
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

		private readonly T first;

		private readonly U second;
	}
}
