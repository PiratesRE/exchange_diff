using System;

namespace Microsoft.Mapi
{
	internal enum SubmitMessageExFlags
	{
		None,
		Preprocess,
		NeedsSpooler,
		IgnoreSendAsRight = 4
	}
}
