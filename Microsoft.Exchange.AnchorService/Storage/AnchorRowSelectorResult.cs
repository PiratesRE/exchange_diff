using System;

namespace Microsoft.Exchange.AnchorService.Storage
{
	internal enum AnchorRowSelectorResult
	{
		AcceptRow = 1,
		RejectRowContinueProcessing,
		RejectRowStopProcessing
	}
}
