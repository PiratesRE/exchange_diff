using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.Prompts.Provisioning;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class DeleteUMPrompts : SingleStepServiceCommand<DeleteUMPromptsRequest, DeleteUMPromptsResponseMessage>
	{
		public DeleteUMPrompts(CallContext callContext, DeleteUMPromptsRequest request) : base(callContext, request)
		{
			this.configurationObject = request.ConfigurationObject;
			this.promptNames = request.PromptNames;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new DeleteUMPromptsResponseMessage(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<DeleteUMPromptsResponseMessage> Execute()
		{
			using (XSOUMPromptStoreAccessor xsoumpromptStoreAccessor = new XSOUMPromptStoreAccessor(base.MailboxIdentityMailboxSession, this.configurationObject))
			{
				if (this.promptNames != null)
				{
					xsoumpromptStoreAccessor.DeletePrompts(this.promptNames);
				}
				else
				{
					xsoumpromptStoreAccessor.DeleteAllPrompts();
				}
			}
			return new ServiceResult<DeleteUMPromptsResponseMessage>(new DeleteUMPromptsResponseMessage());
		}

		private readonly Guid configurationObject;

		private string[] promptNames;
	}
}
