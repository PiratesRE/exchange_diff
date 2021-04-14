using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal sealed class ServerLogger : ExtensibleLogger
	{
		private ServerLogger() : base(new ServerLogConfiguration())
		{
		}

		internal static readonly ServerLogger Instance = new ServerLogger();
	}
}
