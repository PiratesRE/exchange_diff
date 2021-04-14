using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class TaskInvocationInfo
	{
		public TaskInvocationInfo(string cmdletName, string snapinName, Dictionary<string, string> parameters)
		{
			this.CmdletName = cmdletName;
			this.SnapinName = snapinName;
			this.Parameters = parameters;
		}

		public string CmdletName { get; private set; }

		public string SnapinName { get; private set; }

		public Dictionary<string, string> Parameters { get; private set; }
	}
}
