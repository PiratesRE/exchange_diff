using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public enum UpdateAttachmentPermissionsMetadata
	{
		[DisplayName("UAP", "UID")]
		NumberOfUserIDs,
		[DisplayName("UAP", "DL")]
		NumberOfDLs,
		[DisplayName("UAP", "LDL")]
		NumberOfLargeDLs,
		[DisplayName("UAP", "RDL")]
		NumberOfRecipientsInDLs,
		[DisplayName("UAP", "RSDL")]
		NumberOfRecipientsInSmallestDL,
		[DisplayName("UAP", "RLDL")]
		NumberOfRecipientsInLargestDL,
		[DisplayName("UAP", "ADP")]
		NumberOfAttachmentDataProviders,
		[DisplayName("UAP", "UTSP")]
		NumberOfUsersToSetPermissions,
		[DisplayName("UAP", "GMTT")]
		GetMailTipsTime,
		[DisplayName("UAP", "DLET")]
		DLExpandTime
	}
}
