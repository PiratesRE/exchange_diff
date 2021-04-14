using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal abstract class ShadowHubPickerBase
	{
		public ShadowHubPickerBase(IShadowRedundancyConfigurationSource configurationSource)
		{
			this.configurationSource = configurationSource;
		}

		public abstract bool TrySelectShadowServers(out IEnumerable<INextHopServer> shadowServers);

		protected IShadowRedundancyConfigurationSource configurationSource;
	}
}
