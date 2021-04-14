using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum GetFolderMetadata
	{
		[DisplayName("GF", "FT")]
		FolderType,
		[DisplayName("GF", "MBXT")]
		MailboxTarget,
		[DisplayName("GF", "PRIP")]
		Principal
	}
}
