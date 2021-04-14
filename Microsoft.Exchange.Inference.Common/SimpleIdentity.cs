using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	[Serializable]
	internal class SimpleIdentity<TIdentity> : IIdentity, IEquatable<IIdentity>
	{
		public SimpleIdentity(TIdentity identity)
		{
			this.identity = identity;
		}

		public TIdentity Identity
		{
			get
			{
				return this.identity;
			}
		}

		public override int GetHashCode()
		{
			TIdentity tidentity = this.identity;
			return tidentity.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as SimpleIdentity<TIdentity>);
		}

		public virtual bool Equals(IIdentity other)
		{
			return this.Equals(other as SimpleIdentity<TIdentity>);
		}

		public override string ToString()
		{
			TIdentity tidentity = this.identity;
			return tidentity.ToString();
		}

		private bool Equals(SimpleIdentity<TIdentity> other)
		{
			if (other == null)
			{
				return false;
			}
			TIdentity tidentity = this.identity;
			return tidentity.Equals(other.identity);
		}

		private readonly TIdentity identity;
	}
}
