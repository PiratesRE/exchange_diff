using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal enum RtfBorderKind : byte
	{
		None,
		Dot,
		Dash,
		Solid,
		Double,
		Groove,
		Ridge,
		Inset,
		Outset,
		Default = 255
	}
}
