using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class CollectCmdletLogEntriesModule : TaskIOPipelineBase, ITaskModule, ICriticalFeature
	{
		public CollectCmdletLogEntriesModule(TaskContext context)
		{
			this.context = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			bool? flag = (bool?)this.context.InvocationInfo.Fields["CmdletLogEntriesEnabled"];
			if (flag != null && flag.Value)
			{
				ExchangePropertyContainer.EnableCmdletLog(this.context.SessionState);
			}
			if ((flag != null && flag.Value) || ExchangePropertyContainer.IsCmdletLogEnabled(this.context.SessionState))
			{
				task.PreInit += this.Task_PreInit;
				task.Stop += this.DisableCmdletLog;
				task.Release += this.DisableCmdletLog;
				this.context.CommandShell.PrependTaskIOPipelineHandler(this);
			}
		}

		private void Task_PreInit(object sender, EventArgs e)
		{
			ExchangePropertyContainer.EnableCmdletLog(this.context.SessionState);
			this.cmdletLogEntries = ExchangePropertyContainer.GetCmdletLogEntries(this.context.SessionState);
			this.context.InvocationInfo.IsVerboseOn = true;
		}

		private void DisableCmdletLog(object sender, EventArgs e)
		{
			if (this.context == null)
			{
				return;
			}
			if (ExchangePropertyContainer.IsCmdletLogEnabled(this.context.SessionState))
			{
				ExchangePropertyContainer.DisableCmdletLog(this.context.SessionState);
			}
		}

		public void Dispose()
		{
		}

		public override bool WriteVerbose(LocalizedString input, out LocalizedString output)
		{
			if (this.cmdletLogEntries != null)
			{
				this.cmdletLogEntries.AddEntry(input);
			}
			output = input;
			return true;
		}

		public override bool WriteWarning(LocalizedString input, string helpUrl, out LocalizedString output)
		{
			if (this.cmdletLogEntries != null)
			{
				this.cmdletLogEntries.AddEntry(new LocalizedString(string.Format("{0} ({1})", input, Strings.LogHelpUrl(helpUrl))));
			}
			output = input;
			return true;
		}

		private TaskContext context;

		private CmdletLogEntries cmdletLogEntries;
	}
}
