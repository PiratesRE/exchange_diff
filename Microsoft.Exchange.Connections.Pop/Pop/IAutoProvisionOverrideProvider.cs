using System;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IAutoProvisionOverrideProvider
	{
		bool TryGetOverrides(string domain, ConnectionType type, out string[] overrideHosts, out bool trustForSendAs);
	}
}
