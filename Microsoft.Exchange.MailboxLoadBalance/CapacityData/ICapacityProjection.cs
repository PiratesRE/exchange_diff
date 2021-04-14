using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.CapacityData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ICapacityProjection
	{
		BatchCapacityDatum GetCapacity();
	}
}
