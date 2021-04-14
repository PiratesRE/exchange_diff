using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum AttachmentType
	{
		NoAttachment,
		Stream,
		EmbeddedMessage,
		Ole,
		Reference
	}
}
