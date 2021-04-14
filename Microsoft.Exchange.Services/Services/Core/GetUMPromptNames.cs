using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.Prompts.Provisioning;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUMPromptNames : SingleStepServiceCommand<GetUMPromptNamesRequest, GetUMPromptNamesResponseMessage>
	{
		public GetUMPromptNames(CallContext callContext, GetUMPromptNamesRequest request) : base(callContext, request)
		{
			this.configurationObject = request.ConfigurationObject;
			this.hoursElapsedSinceLastModified = request.HoursElapsedSinceLastModified;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetUMPromptNamesResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<GetUMPromptNamesResponseMessage> Execute()
		{
			string[] promptNames = null;
			using (XSOUMPromptStoreAccessor xsoumpromptStoreAccessor = new XSOUMPromptStoreAccessor(base.MailboxIdentityMailboxSession, this.configurationObject))
			{
				promptNames = ((this.hoursElapsedSinceLastModified == 0) ? xsoumpromptStoreAccessor.GetPromptNames() : xsoumpromptStoreAccessor.GetPromptNames(TimeSpan.FromHours((double)this.hoursElapsedSinceLastModified)));
			}
			return new ServiceResult<GetUMPromptNamesResponseMessage>(new GetUMPromptNamesResponseMessage
			{
				PromptNames = promptNames
			});
		}

		private readonly Guid configurationObject;

		private readonly int hoursElapsedSinceLastModified;
	}
}
