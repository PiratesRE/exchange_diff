using System;
using System.Collections;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IPSCommandWrapper
	{
		CommandCollection Commands { get; }

		PowerShellResults<O> Invoke<O>(RunspaceMediator runspaceMediator, IEnumerable pipelineInput, WebServiceParameters parameters, CmdletActivity activity, bool isGetListAsync);

		IPSCommandWrapper AddCommand(string commandText);

		IPSCommandWrapper AddCommand(Command command);

		IPSCommandWrapper AddParameter(string name);

		IPSCommandWrapper AddParameter(string name, object value);
	}
}
