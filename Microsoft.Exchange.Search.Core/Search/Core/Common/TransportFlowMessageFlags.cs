using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	[Flags]
	public enum TransportFlowMessageFlags
	{
		None = 0,
		ShouldBypassNlg = 1,
		SkipTokenInfoGeneration = 2,
		SkipMdmGeneration = 4,
		ShouldDiscardToken = 6
	}
}
