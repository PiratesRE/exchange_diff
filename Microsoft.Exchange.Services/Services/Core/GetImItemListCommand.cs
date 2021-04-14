using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetImItemListCommand : SingleStepServiceCommand<GetImItemListRequest, ImItemList>
	{
		public GetImItemListCommand(CallContext callContext, GetImItemListRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetImItemListResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<ImItemList> Execute()
		{
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			UnifiedContactStoreUtilities unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(mailboxIdentityMailboxSession, new XSOFactory(), Global.UnifiedContactStoreConfiguration);
			ExtendedPropertyUri[] extendedProperties = base.Request.ExtendedProperties;
			ImItemList value = new GetImItemList(extendedProperties, unifiedContactStoreUtilities).Execute();
			return new ServiceResult<ImItemList>(value);
		}
	}
}
