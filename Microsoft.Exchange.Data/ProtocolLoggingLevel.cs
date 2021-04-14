using System;

namespace Microsoft.Exchange.Data
{
	public enum ProtocolLoggingLevel
	{
		[LocDescription(DataStrings.IDs.ProtocolLoggingLevelNone)]
		None,
		[LocDescription(DataStrings.IDs.ProtocolLoggingLevelVerbose)]
		Verbose
	}
}
