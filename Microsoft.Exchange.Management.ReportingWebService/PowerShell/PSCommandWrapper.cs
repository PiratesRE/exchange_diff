using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.ReportingWebService.PowerShell
{
	internal class PSCommandWrapper : IPSCommandWrapper
	{
		public PSCommandWrapper() : this(new PSCommand())
		{
		}

		public PSCommandWrapper(PSCommand command)
		{
			this.command = command;
		}

		public CommandCollection Commands
		{
			get
			{
				return this.command.Commands;
			}
		}

		public PowerShellResults Invoke(RunspaceMediator runspaceMediator)
		{
			return this.command.Invoke(runspaceMediator);
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
			return this.command.ToString();
		}

		private PSCommand command;
	}
}
