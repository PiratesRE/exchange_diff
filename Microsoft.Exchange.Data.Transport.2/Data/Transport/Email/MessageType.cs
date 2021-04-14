using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal enum MessageType
	{
		Undefined,
		Unknown,
		SingleAttachment,
		MultipleAttachments,
		Normal,
		NormalWithRegularAttachments,
		SummaryTnef,
		LegacyTnef,
		SuperLegacyTnef,
		SuperLegacyTnefWithRegularAttachments,
		Voice,
		Fax,
		Journal,
		Dsn,
		Mdn,
		MsRightsProtected,
		Quota,
		AdReplicationMessage,
		PgpEncrypted,
		SmimeSignedNormal,
		SmimeSignedUnknown,
		SmimeSignedEncrypted,
		SmimeOpaqueSigned,
		SmimeEncrypted,
		ApprovalInitiation,
		UMPartner
	}
}
