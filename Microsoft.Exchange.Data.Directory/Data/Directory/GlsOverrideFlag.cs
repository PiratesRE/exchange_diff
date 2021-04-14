using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	internal enum GlsOverrideFlag
	{
		None = 0,
		OverrideIsSet = 1,
		GlsRecordMismatch = 2,
		ResourceForestMismatch = 4,
		AccountForestMismatch = 8
	}
}
