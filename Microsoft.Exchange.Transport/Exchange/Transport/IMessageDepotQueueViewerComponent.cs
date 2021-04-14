using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport
{
	internal interface IMessageDepotQueueViewerComponent : ITransportComponent, IDiagnosable
	{
		IMessageDepotQueueViewer MessageDepotQueueViewer { get; }

		bool Enabled { get; }

		void SetLoadTimeDependencies(IMessageDepotComponent messageDepotComponent, TransportAppConfig.ILegacyQueueConfig queueConfig);
	}
}
