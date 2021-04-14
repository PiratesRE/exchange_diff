using System;

namespace Microsoft.Exchange.Transport
{
	internal interface ISystemCheckComponent : ITransportComponent
	{
		bool Enabled { get; }

		void SetLoadTimeDependencies(SystemCheckConfig systemCheckConfig, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration);
	}
}
