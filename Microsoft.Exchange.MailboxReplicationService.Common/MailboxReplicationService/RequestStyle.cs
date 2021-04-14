using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public enum RequestStyle
	{
		[LocDescription(MrsStrings.IDs.MoveRequestTypeIntraOrg)]
		IntraOrg = 1,
		[LocDescription(MrsStrings.IDs.MoveRequestTypeCrossOrg)]
		CrossOrg
	}
}
