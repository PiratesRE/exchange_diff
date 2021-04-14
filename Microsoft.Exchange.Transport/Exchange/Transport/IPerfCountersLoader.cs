using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IPerfCountersLoader
	{
		void AddCounterToGetExchangeDiagnostics(Type counterType, string counterName);
	}
}
