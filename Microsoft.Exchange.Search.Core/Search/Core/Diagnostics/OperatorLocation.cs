using System;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal enum OperatorLocation
	{
		None,
		DiagnosticsStarted,
		EndOfFlow,
		BeginProcessRecord = 10,
		EndProcessRecord,
		EndProcessRecordException,
		BeginWrite = 20,
		EndWrite,
		EndWriteException
	}
}
