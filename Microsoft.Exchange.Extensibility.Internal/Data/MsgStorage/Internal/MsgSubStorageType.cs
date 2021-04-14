using System;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal enum MsgSubStorageType
	{
		TopLevelMessage,
		AttachedMessage,
		Recipient,
		Attachment
	}
}
