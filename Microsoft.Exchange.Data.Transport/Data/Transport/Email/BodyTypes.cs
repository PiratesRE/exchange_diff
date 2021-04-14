using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	[Flags]
	internal enum BodyTypes
	{
		None = 0,
		Text = 1,
		Enriched = 2,
		Html = 4,
		Calendar = 8
	}
}
