using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Search.Mdb
{
	internal interface INotificationsEventSource : IDisposable
	{
		MapiEvent[] ReadEvents(long startCounter, int eventCountWanted, ReadEventsFlags flags, out long endCounter);

		MapiEvent ReadLastEvent();

		long ReadFirstEventCounter();

		long GetNetworkLatency(int samples);
	}
}
