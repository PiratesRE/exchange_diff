using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public sealed class CopyStreamResult
	{
		public CopyStreamResult(TimeSpan timeReadingInput, TimeSpan timeWritingOutput, long TotalBytesCopied)
		{
			this.TimeReadingInput = timeReadingInput;
			this.TimeWritingOutput = timeWritingOutput;
			this.TotalBytesCopied = TotalBytesCopied;
		}

		public TimeSpan TimeReadingInput { get; private set; }

		public TimeSpan TimeWritingOutput { get; private set; }

		public long TotalBytesCopied { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"TimeReadingInput=",
				this.TimeReadingInput.TotalMilliseconds,
				"ms, TimeWritingOutput=",
				this.TimeWritingOutput.TotalMilliseconds,
				"ms, TotalBytesCopied=",
				this.TotalBytesCopied
			});
		}
	}
}
