using System;

namespace Microsoft.Exchange.Net.XropService
{
	internal interface IServerDiagnosticsHandler
	{
		void AnalyseException(ref Exception exception);

		void LogException(Exception exception);

		void LogMessage(string message);
	}
}
