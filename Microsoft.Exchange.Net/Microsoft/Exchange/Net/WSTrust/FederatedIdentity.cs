using System;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class FederatedIdentity
	{
		public string Identity { get; private set; }

		public IdentityType Type { get; private set; }

		public FederatedIdentity(string identity, IdentityType type)
		{
			this.Identity = identity;
			this.Type = type;
		}

		public override string ToString()
		{
			return this.Type + ":" + this.Identity;
		}
	}
}
