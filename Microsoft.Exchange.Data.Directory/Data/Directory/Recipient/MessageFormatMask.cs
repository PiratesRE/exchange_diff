using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal enum MessageFormatMask
	{
		None,
		PreferenceMask = 131072,
		MessageFormatMask = 262144,
		MessageBodyFormatMask = 1572864,
		MacAttachmentFormatMask = 6291456
	}
}
