using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal enum RtfRunKind : ushort
	{
		Ignore,
		EndOfFile = 4096,
		Begin = 8192,
		End = 12288,
		Binary = 16384,
		SingleRunLast = 16384,
		Keyword = 20480,
		Text = 28672,
		Escape = 32768,
		Unicode = 36864,
		Zero = 40960,
		Mask = 61440
	}
}
