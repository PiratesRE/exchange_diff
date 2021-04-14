using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	[Flags]
	internal enum MessageFlags
	{
		None = 0,
		Normal = 1,
		System = 2,
		KnownApplication = 4,
		Tnef = 8
	}
}
