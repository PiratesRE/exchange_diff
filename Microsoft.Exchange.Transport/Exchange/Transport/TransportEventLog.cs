using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class TransportEventLog
	{
		internal static string GetEventSource()
		{
			string currentProcessName;
			switch (currentProcessName = TransportEventLog.CurrentProcessName)
			{
			case "edgetransport":
				return "MSExchangeTransport";
			case "msexchangefrontendtransport":
				return "MSExchangeFrontEndTransport";
			case "msexchangedelivery":
				return "MSExchangeTransportDelivery";
			case "msexchangesubmission":
				return "MSExchangeTransportSubmission";
			case "msexchangemailboxassistants":
				return "MSExchangeTransportMailboxAssistants";
			case "msexchangetransportlogsearch":
				return "MSExchangeTransportSearch";
			}
			return "MSExchangeTransport";
		}

		internal const string HubEventSource = "MSExchangeTransport";

		internal const string FrontEndEventSource = "MSExchangeFrontEndTransport";

		internal const string MailboxDeliveryEventSource = "MSExchangeTransportDelivery";

		internal const string MailboxSubmissionEventSource = "MSExchangeTransportSubmission";

		internal const string MailSubmissionEventSource = "MSExchangeTransportMailSubmission";

		internal const string MailboxAssistantsEventSource = "MSExchangeTransportMailboxAssistants";

		internal const string TransportLogSearchEventSource = "MSExchangeTransportSearch";

		private static readonly string CurrentProcessName = Process.GetCurrentProcess().ProcessName.ToLower();
	}
}
