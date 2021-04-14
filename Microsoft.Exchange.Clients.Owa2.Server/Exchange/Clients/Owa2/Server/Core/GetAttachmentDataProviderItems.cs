using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetAttachmentDataProviderItems : ServiceCommand<GetAttachmentDataProviderItemsResponse>
	{
		public GetAttachmentDataProviderItems(CallContext callContext, GetAttachmentDataProviderItemsRequest request) : base(callContext)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (string.IsNullOrEmpty(request.AttachmentDataProviderId))
			{
				throw new ArgumentException("The parameter cannot be null or empty.", "attachmentDataProviderId");
			}
			if (request.Paging == null)
			{
				request.Paging = new AttachmentItemsPagingDetails
				{
					Sort = new AttachmentItemsSort
					{
						SortColumn = AttachmentItemsSortColumn.Name,
						SortOrder = AttachmentItemsSortOrder.Descending
					}
				};
			}
			this.request = request;
		}

		protected override GetAttachmentDataProviderItemsResponse InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			AttachmentDataProvider provider = userContext.AttachmentDataProviderManager.GetProvider(base.CallContext, this.request.AttachmentDataProviderId);
			return provider.GetItems(this.request.Paging, this.request.Scope, base.MailboxIdentityMailboxSession);
		}

		private GetAttachmentDataProviderItemsRequest request;
	}
}
