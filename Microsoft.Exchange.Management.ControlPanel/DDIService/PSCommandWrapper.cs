using System;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	public class PSCommandWrapper : IPSCommandWrapper
	{
		public PSCommandWrapper(PSCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			this.command = command;
		}

		public CommandCollection Commands
		{
			get
			{
				return this.command.Commands;
			}
		}

		public PowerShellResults<O> Invoke<O>(RunspaceMediator runspaceMediator, IEnumerable pipelineInput, WebServiceParameters parameters, CmdletActivity activity, bool isGetListAsync)
		{
			return this.command.Invoke(runspaceMediator, pipelineInput, parameters, activity, isGetListAsync);
		}

		public IPSCommandWrapper AddCommand(string commandText)
		{
			return new PSCommandWrapper(this.command.AddCommand(commandText));
		}

		public IPSCommandWrapper AddCommand(Command command)
		{
			return new PSCommandWrapper(this.command.AddCommand(command));
		}

		public IPSCommandWrapper AddParameter(string name)
		{
			return new PSCommandWrapper(this.command.AddParameter(name));
		}

		public IPSCommandWrapper AddParameter(string name, object value)
		{
			return new PSCommandWrapper(this.command.AddParameter(name, value));
		}

		public override string ToString()
		{
			return this.command.ToTraceString();
		}

		private PSCommand command;
	}
}
