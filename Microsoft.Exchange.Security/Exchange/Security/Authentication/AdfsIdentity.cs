using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Security.Authentication
{
	[Serializable]
	public class AdfsIdentity : SidBasedIdentity
	{
		public AdfsIdentity(string userPrincipalName, string userSid, OrganizationId orgId, string PartitionId, IEnumerable<string> groupSidIds, bool isPublicSession) : base(userPrincipalName, userSid, userPrincipalName, "ADFS", null)
		{
			if (groupSidIds == null)
			{
				throw new ArgumentNullException("groupSidIds");
			}
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaServer.ShouldSkipAdfsGroupReadOnFrontend.Enabled)
			{
				this.prepopulatedGroupSidIds = groupSidIds;
			}
			base.UserOrganizationId = orgId;
			this.IsPublicSession = isPublicSession;
		}

		public bool IsPublicSession { get; private set; }

		private const string AdfsAuthType = "ADFS";
	}
}
