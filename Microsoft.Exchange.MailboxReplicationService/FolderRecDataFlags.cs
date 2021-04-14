using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum FolderRecDataFlags
	{
		None = 0,
		PromotedProperties = 1,
		SecurityDescriptors = 2,
		Rules = 4,
		SearchCriteria = 8,
		FolderAcls = 16,
		Views = 32,
		Restrictions = 64,
		ExtendedAclInformation = 128,
		ExtendedData = 97
	}
}
