using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	[Serializable]
	internal class SimpleIdentityWithComparer<TIdentity, TComparer> : IIdentity, IEquatable<IIdentity> where TComparer : IComparer<TIdentity>
	{
		public SimpleIdentityWithComparer(TIdentity identity, TComparer comparer)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity is Null");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("Comparer is Null");
			}
			this.identity = identity;
			this.comparer = comparer;
		}

		public override string ToString()
		{
			TIdentity tidentity = this.identity;
			return tidentity.ToString();
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as SimpleIdentityWithComparer<TIdentity, TComparer>);
		}

		public bool Equals(IIdentity other)
		{
			return this.Equals(other as SimpleIdentityWithComparer<TIdentity, TComparer>);
		}

		public override int GetHashCode()
		{
			if (this.identity is string && this.comparer is StringComparer)
			{
				int hashCode = (this.identity as string).ToLower().GetHashCode();
				TComparer tcomparer = this.comparer;
				return hashCode ^ tcomparer.GetHashCode();
			}
			TIdentity tidentity = this.identity;
			int hashCode2 = tidentity.GetHashCode();
			TComparer tcomparer2 = this.comparer;
			return hashCode2 ^ tcomparer2.GetHashCode();
		}

		private bool Equals(SimpleIdentityWithComparer<TIdentity, TComparer> other)
		{
			if (other == null)
			{
				return false;
			}
			TComparer tcomparer = this.comparer;
			if (tcomparer.Compare(this.identity, other.identity) == 0)
			{
				TComparer tcomparer2 = other.comparer;
				return tcomparer2.Compare(other.identity, this.identity) == 0;
			}
			return false;
		}

		private readonly TIdentity identity;

		private readonly TComparer comparer;
	}
}
