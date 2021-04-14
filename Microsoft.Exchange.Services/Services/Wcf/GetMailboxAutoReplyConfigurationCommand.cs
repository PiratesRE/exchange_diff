using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetMailboxAutoReplyConfigurationCommand : SingleCmdletCommandBase<object, GetMailboxAutoReplyConfigurationResponse, GetMailboxAutoReplyConfiguration, MailboxAutoReplyConfiguration>
	{
		public GetMailboxAutoReplyConfigurationCommand(CallContext callContext) : base(callContext, null, "Get-MailboxAutoReplyConfiguration", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			GetMailboxAutoReplyConfiguration task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(GetMailboxAutoReplyConfigurationResponse response)
		{
			MailboxAutoReplyConfiguration result = this.cmdletRunner.TaskWrapper.Result;
			response.Options = new MailboxAutoReplyConfigurationOptions
			{
				AutoReplyState = result.AutoReplyState,
				EndTime = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)result.EndTime),
				ExternalAudience = result.ExternalAudience,
				ExternalMessage = result.ExternalMessage,
				InternalMessage = result.InternalMessage,
				StartTime = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)result.StartTime)
			};
		}

		protected override PSLocalTask<GetMailboxAutoReplyConfiguration, MailboxAutoReplyConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetMailboxAutoReplyConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
