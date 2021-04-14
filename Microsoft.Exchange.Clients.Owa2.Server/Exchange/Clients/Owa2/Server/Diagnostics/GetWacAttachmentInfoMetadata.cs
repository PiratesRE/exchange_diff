using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal enum GetWacAttachmentInfoMetadata
	{
		[DisplayName("GWAI", "LSA")]
		LogonSmtpAddress,
		[DisplayName("GWAI", "MSA")]
		MailboxSmtpAddress,
		[DisplayName("GWAI", "E")]
		Edit,
		[DisplayName("GWAI", "EXT")]
		Extension,
		[DisplayName("GWAI", "D")]
		DraftProvided,
		[DisplayName("GWAI", "URL")]
		WacUrl,
		[DisplayName("GWAI", "HE")]
		HandledException,
		[DisplayName("GWAI", "Status")]
		Status,
		[DisplayName("GWAI", "ATO")]
		OriginalAttachmentType,
		[DisplayName("GWAI", "ORU")]
		OriginalReferenceAttachmentUrl,
		[DisplayName("GWAI", "ORSU")]
		OriginalReferenceAttachmentServiceUrl,
		[DisplayName("GWAI", "ATR")]
		ResultAttachmentType,
		[DisplayName("GWAI", "RAC")]
		ResultAttachmentCreation,
		[DisplayName("GWAI", "RRU")]
		ResultReferenceAttachmentUrl,
		[DisplayName("GWAI", "RRSU")]
		ResultReferenceAttachmentServiceUrl,
		[DisplayName("GWAI", "ADP")]
		AttachmentDataProvider,
		[DisplayName("GWAI", "ESID")]
		ExchangeSessionId
	}
}
