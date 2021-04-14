using System;

namespace Microsoft.Exchange.LogUploader
{
	public enum Actions
	{
		LetThrough = 1,
		Skip,
		SkipAndLogEvent
	}
}
