using System;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[Flags]
	internal enum SiteMailboxInternalState
	{
		None = 0,
		WelcomeMessageSent = 1
	}
}
