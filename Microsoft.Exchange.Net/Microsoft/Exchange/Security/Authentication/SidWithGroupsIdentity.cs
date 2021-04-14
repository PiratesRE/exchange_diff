using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class SidWithGroupsIdentity : ClientSecurityContextIdentity
	{
		internal SidWithGroupsIdentity(string name, string type, ClientSecurityContext clientSecurityContext) : base(name, type)
		{
			if (clientSecurityContext == null)
			{
				throw new ArgumentNullException("clientSecurityContext");
			}
			this.clientSecurityContext = clientSecurityContext;
		}

		public override SecurityIdentifier Sid
		{
			get
			{
				return this.clientSecurityContext.UserSid;
			}
		}

		internal override ClientSecurityContext CreateClientSecurityContext()
		{
			return this.clientSecurityContext.Clone();
		}

		private ClientSecurityContext clientSecurityContext;
	}
}
