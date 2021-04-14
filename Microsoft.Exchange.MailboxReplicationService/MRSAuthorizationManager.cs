using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class MRSAuthorizationManager : AuthorizationManagerBase
	{
		internal override AdminRoleDefinition[] ComputeAdminRoles(IRootOrganizationRecipientSession recipientSession, ITopologyConfigurationSession configSession)
		{
			string containerDN = configSession.ConfigurationNamingContext.ToDNString();
			ADGroup adgroup = recipientSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.EmaWkGuid, containerDN);
			return new AdminRoleDefinition[]
			{
				new AdminRoleDefinition(adgroup.Sid, "RecipientAdmins"),
				new AdminRoleDefinition(recipientSession.GetExchangeServersUsgSid(), "ExchangeServers"),
				new AdminRoleDefinition(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), "LocalSystem"),
				new AdminRoleDefinition(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), "BuiltinAdmins")
			};
		}
	}
}
