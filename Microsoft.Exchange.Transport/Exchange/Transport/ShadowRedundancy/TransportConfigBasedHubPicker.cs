using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class TransportConfigBasedHubPicker : ShadowHubPickerBase
	{
		public TransportConfigBasedHubPicker(IShadowRedundancyConfigurationSource configurationSource) : base(configurationSource)
		{
			this.enabled = (Components.TransportAppConfig.ShadowRedundancy.ShadowHubList.Count > 0);
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		public override bool TrySelectShadowServers(out IEnumerable<INextHopServer> shadowServers)
		{
			if (!this.enabled)
			{
				throw new InvalidOperationException("Configuration switch is not enabled");
			}
			shadowServers = Components.TransportAppConfig.ShadowRedundancy.ShadowHubList;
			return true;
		}

		private readonly bool enabled;
	}
}
