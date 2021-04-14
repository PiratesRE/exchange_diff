using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class Directory : IDirectory
	{
		public SecurityIdentifier GetExchangeServersUsgSid()
		{
			SecurityIdentifier exchangeServersSid = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 31, "GetExchangeServersUsgSid", "f:\\15.00.1497\\sources\\dev\\mapimt\\src\\RpcHttpModules\\Directory.cs");
				exchangeServersSid = rootOrganizationRecipientSession.GetExchangeServersUsgSid();
			});
			return exchangeServersSid;
		}

		public bool AllowsTokenSerializationBy(WindowsIdentity windowsIdentity)
		{
			bool result;
			using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(windowsIdentity))
			{
				result = LocalServer.AllowsTokenSerializationBy(clientSecurityContext);
			}
			return result;
		}
	}
}
