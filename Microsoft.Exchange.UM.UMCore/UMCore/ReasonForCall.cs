using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal enum ReasonForCall
	{
		None,
		Direct,
		DivertNoAnswer,
		DivertBusy,
		DivertForward,
		Outbound
	}
}
