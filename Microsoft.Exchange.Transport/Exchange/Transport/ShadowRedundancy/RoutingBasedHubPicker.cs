using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class RoutingBasedHubPicker : ShadowHubPickerBase
	{
		public RoutingBasedHubPicker(IShadowRedundancyConfigurationSource configurationSource, IMailRouter mailRouter) : base(configurationSource)
		{
			if (mailRouter == null)
			{
				throw new ArgumentNullException("mailRouter");
			}
			this.mailRouter = mailRouter;
		}

		public override bool TrySelectShadowServers(out IEnumerable<INextHopServer> shadowServers)
		{
			return this.mailRouter.TrySelectHubServersForShadow(new ShadowRoutingConfiguration(this.configurationSource.ShadowMessagePreference, this.configurationSource.MaxRemoteShadowAttempts, this.configurationSource.MaxLocalShadowAttempts), out shadowServers);
		}

		private IMailRouter mailRouter;
	}
}
