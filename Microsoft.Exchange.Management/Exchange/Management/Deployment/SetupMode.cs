using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal enum SetupMode
	{
		Ignore = 61952,
		Add,
		Remove,
		Reinstall,
		Forklift = 61959
	}
}
