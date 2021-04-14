using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface IServerCapabilities
	{
		IEnumerable<string> Capabilities { get; }

		IServerCapabilities Add(string capability);

		IServerCapabilities Remove(string capability);

		bool Supports(string capability);

		bool Supports(IEnumerable<string> desiredCapabilitiesList);

		bool Supports(IServerCapabilities desiredCapabilities);

		IEnumerable<string> NotIn(IServerCapabilities desiredCapabilities);
	}
}
