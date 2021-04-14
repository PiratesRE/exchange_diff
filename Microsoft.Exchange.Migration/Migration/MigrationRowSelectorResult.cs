using System;

namespace Microsoft.Exchange.Migration
{
	internal enum MigrationRowSelectorResult
	{
		AcceptRow = 1,
		RejectRowContinueProcessing,
		RejectRowStopProcessing
	}
}
