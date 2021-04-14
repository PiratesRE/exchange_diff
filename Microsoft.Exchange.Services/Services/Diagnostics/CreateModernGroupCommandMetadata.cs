using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum CreateModernGroupCommandMetadata
	{
		[DisplayName("CMG", "SMTP")]
		GroupSmtpAddress,
		[DisplayName("CMG", "GCT")]
		GroupCreationTime,
		[DisplayName("CMG", "AADT")]
		AADIdentityCreationTime,
		[DisplayName("CMG", "AADCCT")]
		AADCompleteCallbackTime,
		[DisplayName("CMG", "SPNT")]
		SharePointNotificationTime,
		[DisplayName("CMG", "MBT")]
		MailboxCreationTime,
		[DisplayName("CMG", "TPT")]
		TotalProcessingTime,
		[DisplayName("CMG", "DES")]
		DescriptionSpecified,
		[DisplayName("CMG", "MC")]
		MemberCount,
		[DisplayName("CMG", "OC")]
		OwnerCount,
		[DisplayName("CMG", "ERTP")]
		ExceptionType,
		[DisplayName("CMG", "ER")]
		Exception,
		[DisplayName("CMG", "ERLOC")]
		ExceptionLocation,
		[DisplayName("CMG", "CID")]
		CmdletCorrelationId,
		[DisplayName("CMG", "EA")]
		ErrorAction,
		[DisplayName("CMG", "EC")]
		ErrorCode,
		[DisplayName("CMG", "AADAQT")]
		AADAliasQueryTime,
		[DisplayName("CMG", "ASDV")]
		AutoSubscribeOptionDefault,
		[DisplayName("CMG", "ASRV")]
		AutoSubscribeOptionReceived
	}
}
