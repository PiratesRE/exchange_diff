using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServerCapabilities : IServerCapabilities
	{
		internal ServerCapabilities()
		{
			this.Capabilities = new List<string>();
		}

		internal ServerCapabilities(IEnumerable<string> capabilities)
		{
			this.Capabilities = from s in capabilities
			select s;
		}

		internal ServerCapabilities(IServerCapabilities capabilities)
		{
			this.Capabilities = new List<string>(capabilities.Capabilities);
		}

		public IEnumerable<string> Capabilities { get; private set; }

		public IServerCapabilities Add(string capability)
		{
			((IList<string>)this.Capabilities).Add(capability);
			return this;
		}

		public IServerCapabilities Remove(string capability)
		{
			if (!this.Capabilities.Contains(capability, StringComparer.OrdinalIgnoreCase))
			{
				throw new MissingCapabilitiesException(capability);
			}
			((IList<string>)this.Capabilities).Remove(capability);
			return this;
		}

		public bool Supports(string capability)
		{
			return this.Capabilities.Contains(capability, StringComparer.OrdinalIgnoreCase);
		}

		public bool Supports(IEnumerable<string> desiredCapabilitiesList)
		{
			ServerCapabilities desiredCapabilities = new ServerCapabilities(desiredCapabilitiesList);
			return this.Supports(desiredCapabilities);
		}

		public bool Supports(IServerCapabilities desiredCapabilities)
		{
			IEnumerable<string> source = desiredCapabilities.NotIn(this);
			return !source.Any<string>();
		}

		public IEnumerable<string> NotIn(IServerCapabilities desiredCapabilities)
		{
			return this.Capabilities.Except(desiredCapabilities.Capabilities, StringComparer.OrdinalIgnoreCase);
		}
	}
}
