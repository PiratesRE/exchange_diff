using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum DelegateRuleType
	{
		ForwardAndSetAsInformationalUpdate = 1,
		Forward,
		ForwardAndDelete,
		NoForward
	}
}
