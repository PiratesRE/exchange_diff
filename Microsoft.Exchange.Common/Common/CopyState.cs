using System;

namespace Microsoft.Exchange.Common
{
	public enum CopyState
	{
		SuccessfulCopy,
		OpeningInputFile,
		ReadingInputFile,
		CompletingRead,
		OpeningOutputFile,
		WritingOutputFile,
		CompletingWrite,
		FlushingOutputFile,
		CopyStopped,
		InvalidOperation
	}
}
