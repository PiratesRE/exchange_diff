using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.HttpProxy
{
	internal class UserADRawEntryAnchorMailbox : ArchiveSupportedAnchorMailbox
	{
		public UserADRawEntryAnchorMailbox(ADRawEntry activeDirectoryRawEntry, IRequestContext requestContext) : base(AnchorSource.UserADRawEntry, (activeDirectoryRawEntry != null && activeDirectoryRawEntry.Id != null) ? activeDirectoryRawEntry.Id.DistinguishedName : null, requestContext)
		{
			this.activeDirectoryRawEntry = activeDirectoryRawEntry;
		}

		public override string GetOrganizationNameForLogging()
		{
			return ((OrganizationId)this.activeDirectoryRawEntry[ADObjectSchema.OrganizationId]).GetFriendlyName();
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			return this.activeDirectoryRawEntry;
		}

		private ADRawEntry activeDirectoryRawEntry;
	}
}
