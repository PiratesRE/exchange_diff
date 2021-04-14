using System;

namespace Microsoft.Exchange.HttpProxy
{
	internal enum TargetCalculationCallbackState
	{
		TargetResolved,
		MailboxServerResolved,
		LocatorCallback
	}
}
