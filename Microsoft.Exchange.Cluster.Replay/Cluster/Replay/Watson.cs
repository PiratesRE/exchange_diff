using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class Watson : IWatson
	{
		public void SendReport(Exception ex)
		{
			ExWatson.SendReport(ex);
		}

		public void SendReportOnUnhandledException(Action action)
		{
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				action();
			});
		}
	}
}
