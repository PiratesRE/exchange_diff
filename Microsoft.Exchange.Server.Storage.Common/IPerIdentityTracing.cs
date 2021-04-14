using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public interface IPerIdentityTracing<T>
	{
		bool IsConfigured { get; }

		bool IsTurnedOn { get; }

		bool IsEnabled(T identity);

		void TurnOn();

		void TurnOff();
	}
}
