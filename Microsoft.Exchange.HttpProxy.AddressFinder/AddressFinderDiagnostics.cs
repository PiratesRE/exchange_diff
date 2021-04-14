using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.Serialization;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class AddressFinderDiagnostics : IAddressFinderDiagnostics
	{
		public AddressFinderDiagnostics(HttpContextBase httpContext)
		{
			this.logger = RequestLogger.GetLogger(httpContext);
		}

		public void AddErrorInfo(object value)
		{
			this.logger.AppendErrorInfo("AddressFinder", value);
		}

		public void AddRoutingkey(IRoutingKey routingKey, string routingHint)
		{
			this.routingKeyLogs.Add(new Tuple<IRoutingKey, string>(routingKey, routingHint));
		}

		public void LogRoutingKeys()
		{
			if (this.routingKeyLogs.Count == 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (Tuple<IRoutingKey, string> tuple in this.routingKeyLogs)
			{
				stringBuilder.Append(RoutingEntryHeaderSerializer.RoutingTypeToString(tuple.Item1.RoutingItemType) + "-" + tuple.Item1.Value + "|");
				stringBuilder2.Append(tuple.Item2 + "|");
			}
			this.logger.LogField(LogKey.AnchorMailbox, stringBuilder.ToString());
			this.logger.LogField(LogKey.RoutingHint, stringBuilder2.ToString());
		}

		public void LogUnhandledException(Exception ex)
		{
			this.logger.LastChanceExceptionHandler(ex);
		}

		private const string EntrySeparator = "|";

		private const string RoutingKeyValueSeparator = "-";

		private RequestLogger logger;

		private List<Tuple<IRoutingKey, string>> routingKeyLogs = new List<Tuple<IRoutingKey, string>>();
	}
}
