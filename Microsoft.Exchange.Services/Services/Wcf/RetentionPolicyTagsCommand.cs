using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class RetentionPolicyTagsCommand<TRequest, TResponse> : OptionServiceCommandBase<TRequest, TResponse> where TResponse : OptionsResponseBase, new()
	{
		public RetentionPolicyTagsCommand(CallContext callContext, TRequest request) : base(callContext, request)
		{
		}

		protected List<PresentationRetentionPolicyTag> GetRetentionPolicyTags(bool currentUserTagsOnly, Microsoft.Exchange.Services.Core.Types.ElcFolderType[] types, bool optionalInMailbox)
		{
			PSLocalTask<GetRetentionPolicyTag, Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionPolicyTag> pslocalTask = CmdletTaskFactory.Instance.CreateGetRetentionPolicyTagTask(base.CallContext.AccessingPrincipal);
			CmdletRunner<GetRetentionPolicyTag, Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionPolicyTag> cmdletRunner = new CmdletRunner<GetRetentionPolicyTag, Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionPolicyTag>(base.CallContext, "Get-RetentionPolicyTag", ScopeLocation.RecipientRead, pslocalTask);
			cmdletRunner.SetTaskParameter("Types", pslocalTask.Task, types);
			if (currentUserTagsOnly)
			{
				cmdletRunner.SetTaskParameter("Mailbox", pslocalTask.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			}
			if (optionalInMailbox)
			{
				cmdletRunner.SetTaskParameter("OptionalInMailbox", pslocalTask.Task, SwitchParameter.Present);
			}
			cmdletRunner.Execute();
			return (from tag in cmdletRunner.TaskAllResults
			select new PresentationRetentionPolicyTag(tag)).ToList<PresentationRetentionPolicyTag>();
		}

		protected void SetRetentionPolicyTags(RetentionPolicyTagIdParameter[] retentionPolicyTagIds)
		{
			PSLocalTask<SetRetentionPolicyTag, Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionPolicyTag> pslocalTask = CmdletTaskFactory.Instance.CreateSetRetentionPolicyTagTask(base.CallContext.AccessingPrincipal);
			CmdletRunner<SetRetentionPolicyTag, Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionPolicyTag> cmdletRunner = new CmdletRunner<SetRetentionPolicyTag, Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionPolicyTag>(base.CallContext, "Set-RetentionPolicyTag", ScopeLocation.RecipientWrite, pslocalTask);
			cmdletRunner.SetTaskParameter("Mailbox", pslocalTask.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
			cmdletRunner.SetTaskParameter("OptionalInMailbox", pslocalTask.Task, retentionPolicyTagIds);
			cmdletRunner.Execute();
		}

		protected bool UserHasArchive
		{
			get
			{
				ADUser accessingADUser = base.CallContext.AccessingADUser;
				return accessingADUser.ArchiveState == ArchiveState.Local || accessingADUser.ArchiveState == ArchiveState.HostedProvisioned;
			}
		}

		private const string OptionalInMailboxPropertyName = "OptionalInMailbox";
	}
}
