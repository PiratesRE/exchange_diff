using System;
using Microsoft.Exchange.Transport.Scheduler.Contracts;
using Microsoft.Exchange.Transport.Scheduler.Processing;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Transport
{
	internal interface IProcessingSchedulerComponent : ITransportComponent
	{
		IProcessingScheduler ProcessingScheduler { get; }

		IProcessingSchedulerAdmin ProcessingSchedulerAdmin { get; }

		void SetLoadTimeDependencies(ITransportConfiguration transportConfiguration, IMessageDepotComponent messageDepotComponent, IMessageProcessor messageProcessor, IMessagingDatabaseComponent messagingDatabaseComponent);

		void Pause();

		void Resume();
	}
}
