using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum SubmitMessageFlags
	{
		None = 0,
		Preprocess = 1,
		NeedsSpooler = 2,
		IgnoreSendAsRight = 4
	}
}
