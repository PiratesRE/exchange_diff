using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	public enum EdgeBlockMode : byte
	{
		None,
		Reject,
		PassThrough,
		Test,
		Grouping,
		Disabled
	}
}
