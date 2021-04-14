using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum MessageSecureSubmitFlags
	{
		None = 0,
		ClientSubmittedSecurely = 1,
		ServerSubmittedSecurely = 2
	}
}
