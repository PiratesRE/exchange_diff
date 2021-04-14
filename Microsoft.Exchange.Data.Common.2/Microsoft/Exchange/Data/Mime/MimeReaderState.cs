using System;

namespace Microsoft.Exchange.Data.Mime
{
	[Flags]
	internal enum MimeReaderState
	{
		Start = 1,
		PartStart = 2,
		HeaderStart = 4,
		HeaderIncomplete = 8,
		HeaderComplete = 16,
		EndOfHeaders = 32,
		PartPrologue = 64,
		PartBody = 128,
		PartEpilogue = 256,
		PartEnd = 512,
		InlineStart = 1024,
		InlineBody = 2048,
		InlineEnd = 4096,
		InlineJunk = 8192,
		Embedded = 16384,
		EmbeddedEnd = 32768,
		End = 65536
	}
}
