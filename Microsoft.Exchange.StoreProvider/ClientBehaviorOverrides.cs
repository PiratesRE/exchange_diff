using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ClientBehaviorOverrides : IClientBehaviorOverrides
	{
		internal static IClientBehaviorOverrides Instance
		{
			get
			{
				return ClientBehaviorOverrides.instance;
			}
		}

		public bool TryGetClientSpecificOverrides(string clientId, CrossServerBehavior crossServerBehaviorDefaults, out CrossServerBehavior crossServerBehaviorOverrides)
		{
			return CrossServerConnectionRegistryParameters.TryGetClientSpecificOverrides(clientId, crossServerBehaviorDefaults, out crossServerBehaviorOverrides);
		}

		private static IClientBehaviorOverrides instance = new ClientBehaviorOverrides();
	}
}
