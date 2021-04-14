using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public interface IPendingRequestNotifier
	{
		event DataAvailableEventHandler DataAvailable;

		bool ShouldThrottle { get; }

		string ReadDataAndResetState();

		void ConnectionAliveTimer();
	}
}
