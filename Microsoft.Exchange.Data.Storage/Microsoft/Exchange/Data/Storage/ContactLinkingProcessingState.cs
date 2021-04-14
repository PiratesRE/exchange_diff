using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ContactLinkingProcessingState
	{
		Unknown,
		DoNotProcess,
		ProcessBeforeSave,
		ProcessAfterSave,
		Processed
	}
}
