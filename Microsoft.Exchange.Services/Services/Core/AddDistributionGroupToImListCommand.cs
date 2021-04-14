using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class AddDistributionGroupToImListCommand : SingleStepServiceCommand<AddDistributionGroupToImListRequest, ImGroup>
	{
		public AddDistributionGroupToImListCommand(CallContext callContext, AddDistributionGroupToImListRequest request) : base(callContext, request)
		{
			this.smtpAddress = request.SmtpAddress;
			this.displayName = request.DisplayName;
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new AddDistributionGroupToImListResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<ImGroup> Execute()
		{
			if (!SmtpAddress.IsValidSmtpAddress(this.smtpAddress))
			{
				throw new InvalidImDistributionGroupSmtpAddressException();
			}
			RawImGroup rawImGroup = new AddDistributionGroupToImList(this.session, this.smtpAddress, this.displayName, new XSOFactory(), Global.UnifiedContactStoreConfiguration).Execute();
			ImGroup value = ImGroup.LoadFromRawImGroup(rawImGroup, this.session);
			return new ServiceResult<ImGroup>(value);
		}

		private readonly MailboxSession session;

		private readonly string smtpAddress;

		private readonly string displayName;
	}
}
