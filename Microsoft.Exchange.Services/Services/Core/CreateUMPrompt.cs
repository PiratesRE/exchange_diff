using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.Prompts.Provisioning;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CreateUMPrompt : SingleStepServiceCommand<CreateUMPromptRequest, CreateUMPromptResponseMessage>
	{
		public CreateUMPrompt(CallContext callContext, CreateUMPromptRequest request) : base(callContext, request)
		{
			this.configurationObject = request.ConfigurationObject;
			this.promptName = request.PromptName;
			this.audioData = request.AudioData;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new CreateUMPromptResponseMessage(base.Result.Code, base.Result.Error);
		}

		internal override ServiceResult<CreateUMPromptResponseMessage> Execute()
		{
			using (XSOUMPromptStoreAccessor xsoumpromptStoreAccessor = new XSOUMPromptStoreAccessor(base.MailboxIdentityMailboxSession, this.configurationObject))
			{
				xsoumpromptStoreAccessor.CreatePrompt(this.promptName, this.audioData);
			}
			return new ServiceResult<CreateUMPromptResponseMessage>(new CreateUMPromptResponseMessage());
		}

		private readonly Guid configurationObject;

		private readonly string promptName;

		private readonly string audioData;
	}
}
