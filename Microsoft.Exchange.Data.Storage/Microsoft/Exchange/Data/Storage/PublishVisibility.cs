using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum PublishVisibility
	{
		None = 0,
		PrivateDetail = 8,
		PrivateFreeBusy = 16
	}
}
