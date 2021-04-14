using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Flags]
	public enum SpamScanFlags
	{
		IsOutbound = 1,
		AllowUserOptOut = 2,
		CsfmTestXHeader = 4,
		CsfmTestSubjectMod = 8
	}
}
