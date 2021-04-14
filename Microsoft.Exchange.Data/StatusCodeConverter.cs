using System;
using System.Globalization;

namespace Microsoft.Exchange.Data
{
	internal static class StatusCodeConverter
	{
		internal static string UnreachableReasonToString(UnreachableReason reasons)
		{
			return StatusCodeConverter.UnreachableReasonToString(reasons, CultureInfo.CurrentCulture, "\n");
		}

		internal static string UnreachableReasonToString(UnreachableReason reasons, CultureInfo cultureInfo, string separator)
		{
			string text = null;
			if ((reasons & UnreachableReason.NoMdb) != UnreachableReason.None)
			{
				text = StatusCodeConverter.AddReasonString(text, DataStrings.RoutingNoMdb.ToString(cultureInfo), separator);
			}
			if ((reasons & UnreachableReason.NoRouteToMdb) != UnreachableReason.None)
			{
				text = StatusCodeConverter.AddReasonString(text, DataStrings.RoutingNoRouteToMdb.ToString(cultureInfo), separator);
			}
			if ((reasons & UnreachableReason.NoRouteToMta) != UnreachableReason.None)
			{
				text = StatusCodeConverter.AddReasonString(text, DataStrings.RoutingNoRouteToMta.ToString(cultureInfo), separator);
			}
			if ((reasons & UnreachableReason.NonBHExpansionServer) != UnreachableReason.None)
			{
				text = StatusCodeConverter.AddReasonString(text, DataStrings.RoutingNonBHExpansionServer.ToString(cultureInfo), separator);
			}
			if ((reasons & UnreachableReason.NoMatchingConnector) != UnreachableReason.None)
			{
				text = StatusCodeConverter.AddReasonString(text, DataStrings.RoutingNoMatchingConnector.ToString(cultureInfo), separator);
			}
			if ((reasons & UnreachableReason.IncompatibleDeliveryDomain) != UnreachableReason.None)
			{
				text = StatusCodeConverter.AddReasonString(text, DataStrings.RoutingIncompatibleDeliveryDomain.ToString(cultureInfo), separator);
			}
			return text;
		}

		private static string AddReasonString(string allReasons, string reasonString, string separator)
		{
			if (allReasons != null)
			{
				allReasons += separator;
			}
			return allReasons + reasonString;
		}

		internal static string DeferReasonToString(DeferReason deferReason)
		{
			switch (deferReason)
			{
			case DeferReason.None:
				return null;
			case DeferReason.ADTransientFailureDuringResolve:
				return DataStrings.DeferReasonADTransientFailureDuringResolve;
			case DeferReason.ADTransientFailureDuringContentConversion:
				return DataStrings.DeferReasonADTransientFailureDuringContentConversion;
			case DeferReason.Agent:
				return DataStrings.DeferReasonAgent;
			case DeferReason.LoopDetected:
				return DataStrings.DeferReasonLoopDetected;
			case DeferReason.ReroutedByStoreDriver:
				return DataStrings.DeferReasonReroutedByStoreDriver;
			case DeferReason.StorageTransientFailureDuringContentConversion:
				return DataStrings.StorageTransientFailureDuringContentConversion;
			case DeferReason.MarkedAsRetryDeliveryIfRejected:
				return DataStrings.MarkedAsRetryDeliveryIfRejected;
			case DeferReason.TransientFailure:
				return DataStrings.TransientFailure;
			case DeferReason.AmbiguousRecipient:
				return DataStrings.AmbiguousRecipient;
			case DeferReason.ConcurrencyLimitInStoreDriver:
				return DataStrings.DeferReasonConcurrencyLimitInStoreDriver;
			case DeferReason.TargetSiteInboundMailDisabled:
				return DataStrings.DeferReasonTargetSiteInboundMailDisabled;
			case DeferReason.RecipientThreadLimitExceeded:
				return DataStrings.DeferReasonRecipientThreadLimitExceeded;
			case DeferReason.TransientAttributionFailure:
				return DataStrings.DeferReasonTransientAttributionFailure;
			case DeferReason.TransientAcceptedDomainsLoadFailure:
				return DataStrings.DeferReasonTransientAcceptedDomainsLoadFailure;
			case DeferReason.ConfigUpdate:
				return DataStrings.DeferReasonConfigUpdate;
			}
			return deferReason.ToString();
		}
	}
}
