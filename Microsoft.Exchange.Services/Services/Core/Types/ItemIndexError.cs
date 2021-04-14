using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ItemIndexErrorType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum ItemIndexError
	{
		None,
		GenericError,
		Timeout,
		StaleEvent,
		MailboxOffline,
		AttachmentLimitReached,
		MarsWriterTruncation,
		DocumentParserFailure
	}
}
