using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal sealed class UserEwsCallingContext : UserServiceCallingContext<ExchangeServiceBinding>
	{
		public UserEwsCallingContext(ICredentialHandler credentialHandler, string userRole, IDictionary<string, ICredentials> cachedCredentials = null) : base(credentialHandler, cachedCredentials)
		{
			this.userRole = userRole;
		}

		public override void SetServiceApiContext(ExchangeServiceBinding binding, string mailboxEmailAddress)
		{
			base.SetServiceApiContext(binding, mailboxEmailAddress);
			if (!string.IsNullOrEmpty(this.userRole))
			{
				binding.ManagementRole = new ManagementRoleType
				{
					UserRoles = new string[]
					{
						this.userRole
					}
				};
			}
		}

		private readonly string userRole;
	}
}
