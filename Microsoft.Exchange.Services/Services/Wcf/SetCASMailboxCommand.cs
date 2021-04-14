using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetCASMailboxCommand : OptionServiceCommandBase<SetCASMailboxRequest, SetCASMailboxResponse>
	{
		public SetCASMailboxCommand(CallContext callContext, SetCASMailboxRequest request) : base(callContext, request)
		{
		}

		protected override SetCASMailboxResponse CreateTaskAndExecute()
		{
			SetCASMailboxResponse setCASMailboxResponse = new SetCASMailboxResponse();
			this.imapPropertyChanged = (this.request.Options.HasPropertyChanged("ImapForceICalForCalendarRetrievalOption") || this.request.Options.HasPropertyChanged("ImapSuppressReadReceipt"));
			this.popPropertyChanged = (this.request.Options.HasPropertyChanged("PopForceICalForCalendarRetrievalOption") || this.request.Options.HasPropertyChanged("PopSuppressReadReceipt"));
			if (this.imapPropertyChanged || this.popPropertyChanged)
			{
				this.casMailbox = this.GetCASMailboxInfo();
			}
			this.SetCASMailboxInfo();
			setCASMailboxResponse.WasSuccessful = true;
			return setCASMailboxResponse;
		}

		private Microsoft.Exchange.Data.Directory.Management.CASMailbox GetCASMailboxInfo()
		{
			PSLocalTask<GetCASMailbox, Microsoft.Exchange.Data.Directory.Management.CASMailbox> pslocalTask = CmdletTaskFactory.Instance.CreateGetCASMailboxTask(base.CallContext.AccessingPrincipal);
			CmdletRunner<GetCASMailbox, Microsoft.Exchange.Data.Directory.Management.CASMailbox> cmdletRunner = new CmdletRunner<GetCASMailbox, Microsoft.Exchange.Data.Directory.Management.CASMailbox>(base.CallContext, "Get-CASMailbox", ScopeLocation.RecipientRead, pslocalTask);
			cmdletRunner.SetTaskParameter("Identity", pslocalTask.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			cmdletRunner.Execute();
			return cmdletRunner.TaskResult;
		}

		private void SetCASMailboxInfo()
		{
			PSLocalTask<Microsoft.Exchange.Management.RecipientTasks.SetCASMailbox, Microsoft.Exchange.Data.Directory.Management.CASMailbox> taskWrapper = CmdletTaskFactory.Instance.CreateSetCASMailboxTask(base.CallContext.AccessingPrincipal);
			CmdletRunner<Microsoft.Exchange.Management.RecipientTasks.SetCASMailbox, Microsoft.Exchange.Data.Directory.Management.CASMailbox> cmdletRunner = new CmdletRunner<Microsoft.Exchange.Management.RecipientTasks.SetCASMailbox, Microsoft.Exchange.Data.Directory.Management.CASMailbox>(base.CallContext, "Set-CASMailbox", ScopeLocation.RecipientWrite, taskWrapper);
			Microsoft.Exchange.Management.RecipientTasks.SetCASMailbox task = cmdletRunner.TaskWrapper.Task;
			Microsoft.Exchange.Data.Directory.Management.CASMailbox taskParameters = (Microsoft.Exchange.Data.Directory.Management.CASMailbox)task.GetDynamicParameters();
			Microsoft.Exchange.Services.Core.Types.SetCASMailbox options = this.request.Options;
			if (this.imapPropertyChanged)
			{
				bool flag = this.ShouldUseProtocolDefaults(options.ImapForceICalForCalendarRetrievalOption, options.ImapSuppressReadReceipt, this.casMailbox.ImapEnableExactRFC822Size, this.casMailbox.ImapMessagesRetrievalMimeFormat);
				cmdletRunner.SetTaskParameter("ImapUseProtocolDefaults", taskParameters, flag);
			}
			if (this.popPropertyChanged)
			{
				bool flag2 = this.ShouldUseProtocolDefaults(options.PopForceICalForCalendarRetrievalOption, options.PopSuppressReadReceipt, this.casMailbox.PopEnableExactRFC822Size, this.casMailbox.PopMessagesRetrievalMimeFormat);
				cmdletRunner.SetTaskParameter("PopUseProtocolDefaults", taskParameters, flag2);
			}
			cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			cmdletRunner.SetRemainingModifiedTaskParameters(this.request.Options, taskParameters);
			cmdletRunner.Execute();
		}

		private bool ShouldUseProtocolDefaults(bool forceICalForCalendarRetrievalOption, bool suppressReadReceipt, bool enableExactRFC822Size, MimeTextFormat messagesRetrievalMimeFormat)
		{
			return !forceICalForCalendarRetrievalOption && !suppressReadReceipt && !enableExactRFC822Size && messagesRetrievalMimeFormat == MimeTextFormat.BestBodyFormat;
		}

		private bool imapPropertyChanged;

		private bool popPropertyChanged;

		private Microsoft.Exchange.Data.Directory.Management.CASMailbox casMailbox;
	}
}
