using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICrossServerDiagnostics
	{
		void BlockCrossServerCall(ExRpcConnectionInfo connectionInfo);

		void BlockCrossServerCall(ExRpcConnectionInfo connectionInfo, string mailboxDescription);

		void BlockMonitoringCrossServerCall(ExRpcConnectionInfo connectionInfo);

		void LogInfoWatson(ExRpcConnectionInfo connectionInfo);

		void TraceCrossServerCall(string serverDn);
	}
}
