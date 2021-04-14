using System;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CreateResendDraft : ServiceCommand<string>
	{
		public CreateResendDraft(CallContext callContext, string ndrMessageId, string draftsFolderId) : base(callContext)
		{
			this.ndrMessageId = StoreId.EwsIdToStoreObjectId(ndrMessageId);
			this.draftFolderId = StoreId.EwsIdToFolderStoreObjectId(draftsFolderId);
		}

		protected override string InternalExecute()
		{
			Item item = null;
			Item item2 = null;
			string result;
			try
			{
				MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
				item = Item.Bind(mailboxIdentityMailboxSession, this.ndrMessageId, ItemBindOption.None);
				item2 = ((ReportMessage)item).CreateSendAgain(this.draftFolderId);
				if (this.IrmDecryptIfRestricted(item2, UserContextManager.GetUserContext(base.CallContext.HttpContext)))
				{
					((RightsManagedMessageItem)item2).PrepareAcquiredLicensesBeforeSave();
				}
				StoreId storeId = null;
				if (item2 is MessageItem)
				{
					item2.Save(SaveMode.NoConflictResolutionForceSave);
					item2.Load();
					storeId = item2.Id;
				}
				result = StoreId.StoreIdToEwsId(base.CallContext.AccessingPrincipal.MailboxInfo.MailboxGuid, storeId);
			}
			catch (Exception ex)
			{
				OwaServerTraceLogger.AppendToLog(new WacAttachmentLogEvent("Exception generating CreateResendDraft", ex));
				result = string.Empty;
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
				if (item2 != null)
				{
					item2.Dispose();
					item2 = null;
				}
			}
			return result;
		}

		private bool IrmDecryptIfRestricted(Item item, UserContext userContext)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			if (rightsManagedMessageItem != null && rightsManagedMessageItem.IsRestricted)
			{
				if (!rightsManagedMessageItem.IsDecoded)
				{
					rightsManagedMessageItem.Decode(this.CreateOutboundConversionOptions(userContext), true);
				}
				return true;
			}
			return false;
		}

		private OutboundConversionOptions CreateOutboundConversionOptions(UserContext userContext)
		{
			return new OutboundConversionOptions(string.Empty)
			{
				UserADSession = UserContextUtilities.CreateADRecipientSession(base.CallContext.ClientCulture.LCID, true, ConsistencyMode.IgnoreInvalid, false, userContext, true, base.CallContext.Budget)
			};
		}

		private readonly StoreObjectId ndrMessageId;

		private readonly StoreObjectId draftFolderId;
	}
}
