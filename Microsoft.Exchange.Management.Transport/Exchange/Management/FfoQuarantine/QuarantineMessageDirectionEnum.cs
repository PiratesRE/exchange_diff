using System;
using Microsoft.Exchange.Core;

namespace Microsoft.Exchange.Management.FfoQuarantine
{
	public enum QuarantineMessageDirectionEnum
	{
		[LocDescription(CoreStrings.IDs.QuarantineInbound)]
		Inbound,
		[LocDescription(CoreStrings.IDs.QuarantineOutbound)]
		Outbound
	}
}
