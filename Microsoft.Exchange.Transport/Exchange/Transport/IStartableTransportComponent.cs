using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport
{
	internal interface IStartableTransportComponent : ITransportComponent
	{
		string CurrentState { get; }

		void Start(bool initiallyPaused, ServiceState intendedState);

		void Stop();

		void Pause();

		void Continue();
	}
}
