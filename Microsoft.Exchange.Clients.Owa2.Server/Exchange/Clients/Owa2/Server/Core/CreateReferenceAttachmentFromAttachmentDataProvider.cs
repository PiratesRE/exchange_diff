using System;
using System.IO;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CreateReferenceAttachmentFromAttachmentDataProvider : ServiceCommand<CreateAttachmentResponse>
	{
		public CreateReferenceAttachmentFromAttachmentDataProvider(CallContext callContext, ItemId itemId, string attachmentDataProviderId, string location, string dataProviderItemId, string dataProviderParentItemId = null, string providerEndpointUrl = null, string cancellationId = null) : base(callContext)
		{
			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			if (string.IsNullOrEmpty(attachmentDataProviderId))
			{
				throw new ArgumentException("The parameter cannot be null or empty.", "attachmentDataProviderId");
			}
			this.itemId = itemId;
			this.attachmentDataProviderId = attachmentDataProviderId;
			this.location = location;
			this.providerEndpointUrl = providerEndpointUrl;
			this.cancellationId = cancellationId;
			this.dataProviderItemId = dataProviderItemId;
			this.dataProviderParentItemId = dataProviderParentItemId;
		}

		protected override CreateAttachmentResponse InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = cancellationTokenSource.Token;
			if (this.cancellationId != null && userContext.CancelAttachmentManager.OnCreateAttachment(this.cancellationId, cancellationTokenSource))
			{
				return null;
			}
			CreateAttachmentResponse createAttachmentResponse = null;
			try
			{
				AttachmentDataProvider provider = userContext.AttachmentDataProviderManager.GetProvider(base.CallContext, this.attachmentDataProviderId);
				createAttachmentResponse = CreateReferenceAttachmentFromAttachmentDataProvider.AttachReferenceAttachment(provider, userContext, this.location, this.dataProviderItemId, this.itemId.Id, base.IdConverter, this.dataProviderParentItemId, this.providerEndpointUrl);
			}
			finally
			{
				if (this.cancellationId != null)
				{
					AttachmentIdType attachmentIdFromCreateAttachmentResponse = CreateAttachmentHelper.GetAttachmentIdFromCreateAttachmentResponse(createAttachmentResponse);
					if (attachmentIdFromCreateAttachmentResponse != null)
					{
						userContext.CancelAttachmentManager.CreateAttachmentCompleted(this.cancellationId, attachmentIdFromCreateAttachmentResponse);
					}
					else
					{
						userContext.CancelAttachmentManager.CreateAttachmentCancelled(this.cancellationId);
					}
				}
			}
			return createAttachmentResponse;
		}

		internal static CreateAttachmentResponse AttachReferenceAttachment(AttachmentDataProvider attachmentDataProvider, UserContext userContext, string location, string dataProviderItemId, string parentItemId, IdConverter idConverter, string dataProviderParentItemId = null, string providerEndpointUrl = null)
		{
			CreateAttachmentResponse result = null;
			if (!userContext.IsDisposed)
			{
				if (string.IsNullOrEmpty(providerEndpointUrl))
				{
					providerEndpointUrl = attachmentDataProvider.GetEndpointUrlFromItemLocation(location);
				}
				string linkingUrl = attachmentDataProvider.GetLinkingUrl(userContext, location, providerEndpointUrl, dataProviderItemId, dataProviderParentItemId);
				string text = Path.GetFileName(HttpUtility.UrlDecode(linkingUrl));
				if (OneDriveProUtilities.IsDurableUrlFormat(text))
				{
					text = text.Substring(0, text.LastIndexOf("?", StringComparison.InvariantCulture));
				}
				try
				{
					userContext.LockAndReconnectMailboxSession();
					IdAndSession idAndSession = new IdAndSession(StoreId.EwsIdToStoreObjectId(parentItemId), userContext.MailboxSession);
					ReferenceAttachmentType referenceAttachmentType = new ReferenceAttachmentType
					{
						Name = text,
						AttachLongPathName = linkingUrl,
						ProviderEndpointUrl = providerEndpointUrl,
						ProviderType = attachmentDataProvider.Type.ToString()
					};
					if (!userContext.IsGroupUserContext)
					{
						referenceAttachmentType.ContentId = Guid.NewGuid().ToString();
						referenceAttachmentType.ContentType = "image/png";
					}
					AttachmentHierarchy attachmentHierarchy = new AttachmentHierarchy(idAndSession, true, true);
					using (AttachmentBuilder attachmentBuilder = new AttachmentBuilder(attachmentHierarchy, new AttachmentType[]
					{
						referenceAttachmentType
					}, idConverter, true))
					{
						ServiceError serviceError;
						Attachment attachment = attachmentBuilder.CreateAttachment(referenceAttachmentType, out serviceError);
						if (serviceError == null)
						{
							attachmentHierarchy.SaveAll();
						}
						result = CreateAttachmentHelper.CreateAttachmentResponse(attachmentHierarchy, attachment, referenceAttachmentType, idAndSession, serviceError);
					}
				}
				finally
				{
					userContext.UnlockAndDisconnectMailboxSession();
				}
			}
			return result;
		}

		private readonly ItemId itemId;

		private readonly string attachmentDataProviderId;

		private readonly string location;

		private readonly string providerEndpointUrl;

		private readonly string cancellationId;

		private readonly string dataProviderItemId;

		private readonly string dataProviderParentItemId;
	}
}
