using System;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal interface IRoutingComponent : ITransportComponent
	{
		IMailRouter MailRouter { get; }

		void SetLoadTimeDependencies(TransportAppConfig appConfig, ITransportConfiguration transportConfig);

		void SetRunTimeDependencies(ShadowRedundancyComponent shadowRedundancy, UnhealthyTargetFilterComponent unhealthyTargetFilter, CategorizerComponent categorizer);
	}
}
