using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	[Flags]
	internal enum MatchOptions
	{
		None = 0,
		MultiLevelCertWildcards = 1
	}
}
