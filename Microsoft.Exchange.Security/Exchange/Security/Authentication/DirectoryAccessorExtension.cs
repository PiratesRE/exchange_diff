using System;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Security.Authentication
{
	internal static class DirectoryAccessorExtension
	{
		public static MiniRecipient[] GetSlaveAccounts(this IExecuter directoryAccessor, SecurityIdentifier masterAccountSid, OrganizationId tenantOrganizationId, PropertyDefinition[] miniRecipientProperties)
		{
			MiniRecipient[] miniRecipients = null;
			directoryAccessor.Execute(delegate
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(tenantOrganizationId), 40, "GetSlaveAccounts", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\TokenMunger\\DirectoryAccessorExtension.cs");
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, MiniRecipientSchema.MasterAccountSid, masterAccountSid);
				miniRecipients = tenantOrRootOrgRecipientSession.FindMiniRecipient(null, QueryScope.SubTree, filter, null, 2, miniRecipientProperties);
			});
			return miniRecipients;
		}
	}
}
