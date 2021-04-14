using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	public enum GetParticipantOrDLFromAddressMetadata
	{
		[DisplayName("GPDA", "OT")]
		ObjectType,
		[DisplayName("GPDA", "EA")]
		EmailAddress,
		[DisplayName("GPDA", "NM")]
		Name,
		[DisplayName("GPDA", "ODN")]
		OriginalDisplayName,
		[DisplayName("GPDA", "MT")]
		MailboxType,
		[DisplayName("GPDA", "IID")]
		ItemId
	}
}
