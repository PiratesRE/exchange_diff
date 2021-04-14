using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.Prompts.Provisioning;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUMPrompt : SingleStepServiceCommand<GetUMPromptRequest, GetUMPromptResponseMessage>
	{
		public GetUMPrompt(CallContext callContext, GetUMPromptRequest request) : base(callContext, request)
		{
			this.configurationObject = request.ConfigurationObject;
			this.promptName = request.PromptName;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetUMPromptResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetUMPromptResponseMessage> Execute()
		{
			string audioData = null;
			using (XSOUMPromptStoreAccessor xsoumpromptStoreAccessor = new XSOUMPromptStoreAccessor(base.MailboxIdentityMailboxSession, this.configurationObject))
			{
				audioData = xsoumpromptStoreAccessor.GetPrompt(this.promptName);
			}
			return new ServiceResult<GetUMPromptResponseMessage>(new GetUMPromptResponseMessage
			{
				AudioData = audioData
			});
		}

		private readonly Guid configurationObject;

		private readonly string promptName;
	}
}
