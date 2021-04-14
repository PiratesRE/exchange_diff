using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.FfoReporting
{
	public enum MessageDirection
	{
		[LocDescription(CoreStrings.IDs.MessageDirectionAll)]
		All,
		[LocDescription(CoreStrings.IDs.MessageDirectionSent)]
		Sent,
		[LocDescription(CoreStrings.IDs.MessageDirectionReceived)]
		Received
	}
}
