using System;

namespace Microsoft.Exchange.LogUploader
{
	internal enum ProcessingStatus
	{
		NeedProcessing,
		InProcessing,
		ReadyToWriteToDatabase,
		CompletedProcessing,
		Unknown
	}
}
