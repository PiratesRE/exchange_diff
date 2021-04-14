using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport
{
	internal interface IMessageDepotComponent : ITransportComponent, IDiagnosable
	{
		IMessageDepot MessageDepot { get; }

		bool Enabled { get; }

		void SetLoadTimeDependencies(MessageDepotConfig messageDepotConfig);
	}
}
