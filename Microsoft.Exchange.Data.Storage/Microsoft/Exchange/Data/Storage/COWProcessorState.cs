using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum COWProcessorState
	{
		Unknown,
		DoNotProcess,
		ProcessAfterSave,
		Processed
	}
}
