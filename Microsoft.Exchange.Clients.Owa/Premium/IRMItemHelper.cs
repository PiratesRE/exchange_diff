using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class IRMItemHelper
	{
		internal IRMItemHelper(MessageItem message, UserContext userContext, bool isPreviewForm, bool isEmbeddedItem)
		{
			this.message = message;
			this.userContext = userContext;
			this.isPreviewForm = isPreviewForm;
			this.isEmbeddedItem = isEmbeddedItem;
			this.isIrmMessageInJunkEmailFolder = (message.IsRestricted && JunkEmailUtilities.IsInJunkEmailFolder(message, isEmbeddedItem, userContext));
		}

		public bool IsPrintRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.Print);
			}
		}

		public bool IsCopyRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.Extract);
			}
		}

		public bool IsReplyRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.Reply);
			}
		}

		public bool IsReplyAllRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.ReplyAll);
			}
		}

		public bool IsForwardRestricted
		{
			get
			{
				return this.IsUsageRightRestricted(ContentRight.Forward);
			}
		}

		public bool IsRemoveAllowed
		{
			get
			{
				return !this.isEmbeddedItem && !this.IsUsageRightRestricted(ContentRight.Export);
			}
		}

		public void IrmDecryptIfRestricted()
		{
			if (!Utilities.IsIrmRestricted(this.message))
			{
				return;
			}
			RightsManagedMessageItem rightsManagedMessageItem = (RightsManagedMessageItem)this.message;
			if (!this.userContext.IsIrmEnabled)
			{
				this.irmDecryptionStatus = RightsManagedMessageDecryptionStatus.FeatureDisabled;
				return;
			}
			if (!rightsManagedMessageItem.CanDecode)
			{
				this.irmDecryptionStatus = RightsManagedMessageDecryptionStatus.NotSupported;
				return;
			}
			try
			{
				Utilities.IrmDecryptIfRestricted(this.message, this.userContext, !this.isPreviewForm);
			}
			catch (MissingRightsManagementLicenseException ex)
			{
				this.irmDecryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(ex);
				if (!Utilities.IsPrefetchRequest(OwaContext.Current) && !this.isEmbeddedItem && ex.MessageStoreId != null && !string.IsNullOrEmpty(ex.PublishLicense))
				{
					OwaStoreObjectId messageId = OwaStoreObjectId.CreateFromItemId(ex.MessageStoreId, null, ex.MessageInArchive ? OwaStoreObjectIdType.ArchiveMailboxObject : OwaStoreObjectIdType.MailBoxObject, ex.MailboxOwnerLegacyDN);
					this.userContext.AsyncAcquireIrmLicenses(messageId, ex.PublishLicense, ex.RequestCorrelator);
				}
			}
			catch (RightsManagementPermanentException exception)
			{
				this.irmDecryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception);
			}
			catch (RightsManagementTransientException exception2)
			{
				this.irmDecryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception2);
			}
		}

		private bool IsUsageRightRestricted(ContentRight right)
		{
			if (this.isIrmMessageInJunkEmailFolder)
			{
				if (right <= ContentRight.Forward)
				{
					if (right != ContentRight.Print && right != ContentRight.Forward)
					{
						goto IL_32;
					}
				}
				else if (right != ContentRight.Reply && right != ContentRight.ReplyAll)
				{
					goto IL_32;
				}
				return true;
			}
			IL_32:
			RightsManagedMessageItem rightsManagedMessageItem = this.message as RightsManagedMessageItem;
			if (rightsManagedMessageItem == null || !rightsManagedMessageItem.IsRestricted)
			{
				return false;
			}
			if (!this.userContext.IsIrmEnabled)
			{
				return false;
			}
			if (this.irmDecryptionStatus.Failed)
			{
				return !right.IsUsageRightGranted(ContentRight.Extract) && !right.IsUsageRightGranted(ContentRight.Print);
			}
			return !rightsManagedMessageItem.UsageRights.IsUsageRightGranted(right);
		}

		internal bool IsRestrictedButIrmFeatureDisabledOrDecryptionFailed
		{
			get
			{
				return this.message.IsRestricted && (!this.userContext.IsIrmEnabled || this.irmDecryptionStatus.Failed);
			}
		}

		internal bool IsRestrictedAndIrmFeatureEnabled
		{
			get
			{
				return this.message.IsRestricted && this.userContext.IsIrmEnabled;
			}
		}

		internal void RenderAlternateBodyForIrm(TextWriter writer, bool isProtectedVoicemail)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(Utilities.GetAlternateBodyForIrm(this.userContext, Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml, this.irmDecryptionStatus, isProtectedVoicemail));
		}

		private readonly bool isIrmMessageInJunkEmailFolder;

		private MessageItem message;

		private UserContext userContext;

		private bool isPreviewForm;

		private bool isEmbeddedItem;

		private RightsManagedMessageDecryptionStatus irmDecryptionStatus;
	}
}
