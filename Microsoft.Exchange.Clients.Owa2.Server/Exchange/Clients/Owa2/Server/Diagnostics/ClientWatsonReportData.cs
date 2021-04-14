using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal struct ClientWatsonReportData
	{
		public string TraceComponent;

		public string FunctionName;

		public string PackageName;

		public string ExceptionMessage;

		public string ExceptionType;

		public string NormalizedCallStack;

		public string OriginalCallStack;

		public int CallStackHash;

		public bool? IsUnhandledException;
	}
}
