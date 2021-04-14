using System;
using Microsoft.Exchange.HttpProxy.Routing;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	internal interface IRouteSelectorDiagnostics : IRoutingDiagnostics
	{
		void SetOrganization(string value);

		void AddRoutingEntry(string value);

		void AddErrorInfo(object value);

		void ProcessRoutingKey(IRoutingKey key);

		void ProcessRoutingEntry(IRoutingEntry entry);
	}
}
