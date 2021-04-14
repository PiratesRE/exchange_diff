using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetImItemsCommand : SingleStepServiceCommand<GetImItemsRequest, ImItemList>
	{
		public GetImItemsCommand(CallContext callContext, GetImItemsRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetImItemsResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<ImItemList> Execute()
		{
			MailboxSession mailboxIdentityMailboxSession = base.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			ExtendedPropertyUri[] extendedProperties = base.Request.ExtendedProperties;
			RawImItemList rawImItemList = new GetImItems(mailboxIdentityMailboxSession, this.GetContactIds(), this.GetGroupIds(), extendedProperties, new XSOFactory()).Execute();
			return new ServiceResult<ImItemList>(ImItemList.LoadFrom(rawImItemList, extendedProperties, mailboxIdentityMailboxSession));
		}

		private StoreId[] GetContactIds()
		{
			return this.ConvertItemIdsToStoreIds(base.Request.ContactIds);
		}

		private StoreId[] GetGroupIds()
		{
			return this.ConvertItemIdsToStoreIds(base.Request.GroupIds);
		}

		private StoreId[] ConvertItemIdsToStoreIds(ItemId[] toConvert)
		{
			if (toConvert == null)
			{
				return null;
			}
			List<StoreId> list = new List<StoreId>(toConvert.Length);
			foreach (ItemId baseItemId in toConvert)
			{
				try
				{
					IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(baseItemId);
					list.Add(idAndSession.Id);
				}
				catch (InvalidStoreIdException)
				{
				}
				catch (ObjectNotFoundException)
				{
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			return list.ToArray();
		}
	}
}
