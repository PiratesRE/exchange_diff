using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IStoreDriver
	{
		void Start(bool initiallyPaused);

		void Retire();

		void Stop();

		void Pause();

		void Continue();

		void DoLocalDelivery(NextHopConnection connection);

		void ExpireOldSubmissionConnections();

		string CurrentState { get; }
	}
}
