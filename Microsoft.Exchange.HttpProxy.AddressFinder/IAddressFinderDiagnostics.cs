using System;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal interface IAddressFinderDiagnostics
	{
		void AddErrorInfo(object value);

		void AddRoutingkey(IRoutingKey routingKey, string routingHint);

		void LogRoutingKeys();

		void LogUnhandledException(Exception ex);
	}
}
