using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IClientBehaviorOverrides
	{
		bool TryGetClientSpecificOverrides(string clientId, CrossServerBehavior crossServerBehaviorDefaults, out CrossServerBehavior crossServerBehaviorOverrides);
	}
}
