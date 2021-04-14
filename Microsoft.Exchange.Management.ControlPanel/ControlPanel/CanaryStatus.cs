using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[Flags]
	internal enum CanaryStatus
	{
		None = 0,
		IsCanaryRenewed = 16,
		IsCanaryValid = 64,
		IsCanaryAboutToExpire = 128,
		IsCanaryNeeded = 256
	}
}
