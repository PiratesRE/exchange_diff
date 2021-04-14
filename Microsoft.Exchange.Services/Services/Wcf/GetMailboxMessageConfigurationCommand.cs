using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetMailboxMessageConfigurationCommand : SingleCmdletCommandBase<object, GetMailboxMessageConfigurationResponse, GetMailboxMessageConfiguration, MailboxMessageConfiguration>
	{
		public GetMailboxMessageConfigurationCommand(CallContext callContext) : base(callContext, null, "Get-MailboxMessageConfiguration", ScopeLocation.RecipientRead)
		{
		}

		protected override void PopulateTaskParameters()
		{
			PSLocalTask<GetMailboxMessageConfiguration, MailboxMessageConfiguration> taskWrapper = this.cmdletRunner.TaskWrapper;
			this.cmdletRunner.SetTaskParameter("Identity", taskWrapper.Task, new MailboxIdParameter(base.CallContext.AccessingPrincipal.ObjectId));
		}

		protected override void PopulateResponseData(GetMailboxMessageConfigurationResponse response)
		{
			PSLocalTask<GetMailboxMessageConfiguration, MailboxMessageConfiguration> taskWrapper = this.cmdletRunner.TaskWrapper;
			MailboxMessageConfiguration result = taskWrapper.Result;
			response.Options = new MailboxMessageConfigurationOptions
			{
				AfterMoveOrDeleteBehavior = result.AfterMoveOrDeleteBehavior,
				AlwaysShowBcc = result.AlwaysShowBcc,
				AlwaysShowFrom = result.AlwaysShowFrom,
				AutoAddSignature = result.AutoAddSignature,
				AutoAddSignatureOnMobile = result.AutoAddSignatureOnMobile,
				CheckForForgottenAttachments = result.CheckForForgottenAttachments,
				ConversationSortOrder = result.ConversationSortOrder,
				DefaultFontColor = result.DefaultFontColor,
				DefaultFontFlags = result.DefaultFontFlags,
				DefaultFontName = result.DefaultFontName,
				DefaultFontSize = result.DefaultFontSize,
				DefaultFormat = result.DefaultFormat,
				EmailComposeMode = result.EmailComposeMode,
				EmptyDeletedItemsOnLogoff = result.EmptyDeletedItemsOnLogoff,
				HideDeletedItems = result.HideDeletedItems,
				NewItemNotification = result.NewItemNotification,
				PreviewMarkAsReadBehavior = result.PreviewMarkAsReadBehavior,
				PreviewMarkAsReadDelaytime = result.PreviewMarkAsReadDelaytime,
				ReadReceiptResponse = result.ReadReceiptResponse,
				ShowConversationAsTree = result.ShowConversationAsTree,
				SendAddressDefault = result.SendAddressDefault,
				SignatureHtml = result.SignatureHtml,
				SignatureText = result.SignatureText,
				SignatureTextOnMobile = result.SignatureTextOnMobile,
				UseDefaultSignatureOnMobile = result.UseDefaultSignatureOnMobile
			};
		}

		protected override PSLocalTask<GetMailboxMessageConfiguration, MailboxMessageConfiguration> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateGetMailboxMessageConfigurationTask(base.CallContext.AccessingPrincipal);
		}
	}
}
