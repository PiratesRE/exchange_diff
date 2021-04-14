using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox.Escalation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IGroupEscalationFlightInfo
	{
		bool IsGroupEscalationFooterEnabled();
	}
}
