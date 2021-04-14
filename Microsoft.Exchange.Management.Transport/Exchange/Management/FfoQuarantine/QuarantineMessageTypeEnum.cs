using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.FfoQuarantine
{
	public enum QuarantineMessageTypeEnum
	{
		[LocDescription(CoreStrings.IDs.QuarantineSpam)]
		Spam,
		[LocDescription(CoreStrings.IDs.QuarantineTransportRule)]
		TransportRule
	}
}
