using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetSharingMetadata : SingleStepServiceCommand<GetSharingMetadataRequest, EncryptionResults>
	{
		public GetSharingMetadata(CallContext callContext, GetSharingMetadataRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetSharingMetadataResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<EncryptionResults> Execute()
		{
			Util.ValidateSmtpAddress(base.Request.SenderSmtpAddress);
			foreach (string smtpAddress in base.Request.Recipients)
			{
				Util.ValidateSmtpAddress(smtpAddress);
			}
			IdAndSession idAndSession = base.IdConverter.ConvertXmlToIdAndSessionReadOnly(base.Request.IdOfFolderToShare, BasicTypes.Folder);
			MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new InvalidFolderTypeForOperationException(CoreResources.IDs.ErrorGetSharingMetadataOnlyForMailbox);
			}
			string className;
			using (Folder folder = Folder.Bind(mailboxSession, idAndSession.Id))
			{
				className = folder.ClassName;
			}
			SharingDataType sharingDataType = SharingDataType.FromContainerClass(className);
			if (sharingDataType == null || !sharingDataType.IsExternallySharable)
			{
				throw new InvalidFolderTypeForOperationException(CoreResources.IDs.ErrorGetSharingMetadataNotSupported);
			}
			string folderId = StoreId.StoreIdToEwsId(mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, idAndSession.Id);
			SharedFolderDataEncryption sharedFolderDataEncryption = new SharedFolderDataEncryption(ExternalAuthentication.GetCurrent());
			ServiceResult<EncryptionResults> result;
			using (ExternalUserCollection externalUsers = mailboxSession.GetExternalUsers())
			{
				EncryptionResults value = sharedFolderDataEncryption.Encrypt(mailboxSession.MailboxOwner, externalUsers, base.Request.Recipients, base.Request.SenderSmtpAddress, className, folderId, new FrontEndLocator(), null);
				result = new ServiceResult<EncryptionResults>(value);
			}
			return result;
		}
	}
}
