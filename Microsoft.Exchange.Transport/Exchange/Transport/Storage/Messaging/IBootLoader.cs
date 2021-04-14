using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.RemoteDelivery;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal interface IBootLoader : IStartableTransportComponent, ITransportComponent
	{
		void SetLoadTimeDependencies(ExEventLog eventLogger, IMessagingDatabase database, ShadowRedundancyComponent shadowRedundancyComponent, PoisonMessage poisonComponent, ICategorizer categorizerComponent, QueueManager queueManagerComponent, IBootLoaderConfig bootLoaderConfiguration);

		event Action OnBootLoadCompleted;
	}
}
