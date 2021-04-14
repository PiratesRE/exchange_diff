using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	internal class CmdletExecutionRequest<TCmdletResult> : BaseRequest
	{
		protected CmdletExecutionRequest(string cmdletName, CmdletExecutionPool cmdletPool, ILogger logger)
		{
			this.cmdletPool = cmdletPool;
			this.logger = logger;
			this.Command = new PSCommand();
			this.Command.AddCommand(cmdletName);
		}

		public override Exception Exception
		{
			get
			{
				Exception result;
				if ((result = base.Exception) == null)
				{
					if (this.Error != null)
					{
						return this.Error.Exception;
					}
					result = null;
				}
				return result;
			}
		}

		public override bool IsBlocked
		{
			get
			{
				return !this.cmdletPool.HasRunspacesAvailable;
			}
		}

		public ErrorRecord Error
		{
			get
			{
				return this.error;
			}
		}

		public TCmdletResult Result { get; private set; }

		private protected PSCommand Command { protected get; private set; }

		protected string CommandString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Command command in this.Command.Commands)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append("; ");
					}
					stringBuilder.Append(command.CommandText);
					foreach (CommandParameter commandParameter in command.Parameters)
					{
						stringBuilder.AppendFormat(" -{0}", commandParameter.Name);
					}
				}
				return stringBuilder.ToString();
			}
		}

		public override RequestDiagnosticData GetDiagnosticData(bool verbose)
		{
			CmdletRequestDiagnosticData cmdletRequestDiagnosticData = (CmdletRequestDiagnosticData)base.GetDiagnosticData(verbose);
			cmdletRequestDiagnosticData.ErrorRecord = this.error;
			cmdletRequestDiagnosticData.Exception = this.Exception;
			cmdletRequestDiagnosticData.Command = PowershellCommandDiagnosticData.FromPSCommand(this.Command, verbose);
			return cmdletRequestDiagnosticData;
		}

		protected override RequestDiagnosticData CreateDiagnosticData()
		{
			return new CmdletRequestDiagnosticData();
		}

		protected override void ProcessRequest()
		{
			using (RunspaceReservation runspaceReservation = this.cmdletPool.AcquireRunspace())
			{
				this.logger.Log(MigrationEventType.Verbose, "About to execute powershell command {0}", new object[]
				{
					this.CommandString
				});
				this.Result = runspaceReservation.Runspace.RunPSCommandSingleOrDefault<TCmdletResult>(this.Command, out this.error);
				if (this.error != null && this.error.Exception != null)
				{
					this.error.Exception.PreserveExceptionStack();
				}
				this.logger.Log(MigrationEventType.Verbose, "Finished executing powershell request. Error: {0}", new object[]
				{
					this.error
				});
			}
		}

		private readonly CmdletExecutionPool cmdletPool;

		private readonly ILogger logger;

		private ErrorRecord error;
	}
}
