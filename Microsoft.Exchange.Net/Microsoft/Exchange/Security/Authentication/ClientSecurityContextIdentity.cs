using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	[Serializable]
	public abstract class ClientSecurityContextIdentity : GenericIdentity
	{
		protected ClientSecurityContextIdentity(string name, string type) : base(name, type)
		{
		}

		public abstract SecurityIdentifier Sid { get; }

		internal abstract ClientSecurityContext CreateClientSecurityContext();
	}
}
