using System;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetSharingFolder : SingleStepServiceCommand<GetSharingFolderRequest, XmlElement>
	{
		public GetSharingFolder(CallContext callContext, GetSharingFolderRequest request) : base(callContext, request)
		{
		}

		internal override ServiceResult<XmlElement> Execute()
		{
			Util.ValidateSmtpAddress(base.Request.SmtpAddress);
			MailboxSession mailboxIdentityMailboxSession = base.GetMailboxIdentityMailboxSession();
			StoreObjectId sharingFolderId;
			if (!string.IsNullOrEmpty(base.Request.DataType) && string.IsNullOrEmpty(base.Request.SharedFolderId))
			{
				sharingFolderId = this.GetSharingFolderByDataType(mailboxIdentityMailboxSession, base.Request.SmtpAddress, base.Request.DataType);
			}
			else
			{
				if (!string.IsNullOrEmpty(base.Request.DataType) || string.IsNullOrEmpty(base.Request.SharedFolderId))
				{
					throw new InvalidGetSharingFolderRequestException();
				}
				sharingFolderId = this.GetSharingFolderBySharedFolderId(mailboxIdentityMailboxSession, base.Request.SmtpAddress, base.Request.SharedFolderId);
			}
			return new ServiceResult<XmlElement>(GetSharingFolder.CreateSharingFolderIdXml(sharingFolderId, mailboxIdentityMailboxSession));
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetSharingFolderResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		private static XmlElement CreateSharingFolderIdXml(StoreObjectId sharingFolderId, MailboxSession mailboxSession)
		{
			if (sharingFolderId == null)
			{
				return null;
			}
			XmlElement xmlElement = ServiceXml.CreateElement(new SafeXmlDocument(), "TemporaryContainerName", "http://schemas.microsoft.com/exchange/services/2006/messages");
			IdConverter.CreateStoreIdXml(xmlElement, sharingFolderId, new MailboxId(mailboxSession), "SharingFolderId", "http://schemas.microsoft.com/exchange/services/2006/messages");
			return (XmlElement)xmlElement.FirstChild;
		}

		private StoreObjectId GetSharingFolderByDataType(MailboxSession mailboxSession, string smtpAddress, string dataType)
		{
			StoreObjectId result;
			using (SharingSubscriptionManager sharingSubscriptionManager = new SharingSubscriptionManager(mailboxSession))
			{
				SharingSubscriptionData primary = sharingSubscriptionManager.GetPrimary(dataType, smtpAddress);
				if (primary != null)
				{
					IdAndName idAndName = new SharingFolderManager(mailboxSession).EnsureFolder(primary);
					if (!primary.LocalFolderId.Equals(idAndName.Id))
					{
						primary.LocalFolderId = idAndName.Id;
						result = sharingSubscriptionManager.CreateOrUpdate(primary, false).LocalFolderId;
					}
					else
					{
						result = primary.LocalFolderId;
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private StoreObjectId GetSharingFolderBySharedFolderId(MailboxSession mailboxSession, string smtpAddress, string sharedFolderId)
		{
			using (SharingSubscriptionManager sharingSubscriptionManager = new SharingSubscriptionManager(mailboxSession))
			{
				SharingSubscriptionKey subscriptionKey = new SharingSubscriptionKey(smtpAddress, sharedFolderId);
				SharingSubscriptionData existing = sharingSubscriptionManager.GetExisting(subscriptionKey);
				if (existing != null)
				{
					IdAndName folder = new SharingFolderManager(mailboxSession).GetFolder(existing);
					if (folder != null)
					{
						return folder.Id;
					}
				}
			}
			return null;
		}
	}
}
