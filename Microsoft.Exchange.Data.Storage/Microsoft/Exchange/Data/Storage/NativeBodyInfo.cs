using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum NativeBodyInfo
	{
		Undefined,
		PlainTextBody,
		RtfCompressedBody,
		HtmlBody,
		ClearSignedBody
	}
}
