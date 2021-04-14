using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal enum MimeWriteState
	{
		Initial,
		Complete,
		StartPart,
		Headers,
		Parameters,
		Recipients,
		GroupRecipients,
		PartContent,
		EndPart
	}
}
