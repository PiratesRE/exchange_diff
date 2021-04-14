using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	internal enum RecipientFilterableObjectFlags
	{
		None = 0,
		FilterApplied = 1,
		IsDefault = 2
	}
}
