using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IWatson
	{
		void SendReport(Exception ex);

		void SendReportOnUnhandledException(Action action);
	}
}
