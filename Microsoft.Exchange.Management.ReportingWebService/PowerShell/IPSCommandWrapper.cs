using System;
using System.Management.Automation.Runspaces;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.ReportingWebService.PowerShell
{
	internal interface IPSCommandWrapper
	{
		CommandCollection Commands { get; }

		PowerShellResults Invoke(RunspaceMediator runspaceMediator);

		IPSCommandWrapper AddCommand(string commandText);

		IPSCommandWrapper AddCommand(Command command);

		IPSCommandWrapper AddParameter(string name);

		IPSCommandWrapper AddParameter(string name, object value);
	}
}
