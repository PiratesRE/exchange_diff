using System;

namespace Microsoft.Exchange.Configuration.PswsProxy
{
	internal class ResponseContent
	{
		internal string Id { get; set; }

		internal string Command { get; set; }

		internal ExecutionStatus Status { get; set; }

		internal string OutputFormat { get; set; }

		internal ResponseErrorRecord Error { get; set; }

		internal DateTime ExpirationTime { get; set; }

		internal int WaitMsec { get; set; }

		internal string OutputXml { get; set; }
	}
}
