using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IGroupMembershipReader<T>
	{
		IEnumerable<T> GetJoinedGroups();
	}
}
