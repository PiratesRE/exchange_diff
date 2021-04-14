using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal interface ITimedEventHandler
	{
		void Invoke(Context context, TimedEventEntry timedEvent);
	}
}
