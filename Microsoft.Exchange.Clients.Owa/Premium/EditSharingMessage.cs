using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class EditSharingMessage : EditMessage
	{
		public EditSharingMessage() : base(true)
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "publish", false);
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				this.isPublishing = (queryStringParameter == "1");
			}
			base.OnLoad(e);
			this.sharingMessageWriter = new SharingMessageWriter(this.sharingMessage, base.UserContext);
		}

		protected SharingMessageWriter SharingMessageWriter
		{
			get
			{
				return this.sharingMessageWriter;
			}
		}

		protected void RenderSubject()
		{
			base.RenderSubject(false);
		}

		protected void RenderTitle()
		{
			if (this.IsSharingInvitation)
			{
				string subject;
				if (this.IsDefaultFolderToBeShared)
				{
					subject = string.Format(base.UserContext.UserCulture, LocalizedStrings.GetNonEncoded(this.isPublishing ? 1155246534 : 1285974930), new object[]
					{
						base.UserContext.MailboxSession.DisplayName
					});
				}
				else
				{
					subject = string.Format(base.UserContext.UserCulture, LocalizedStrings.GetNonEncoded(this.isPublishing ? -198640513 : -2052624973), new object[]
					{
						base.UserContext.MailboxSession.DisplayName,
						this.sharingMessage.SharedFolderName
					});
				}
				RenderingUtilities.RenderSubject(base.Response.Output, subject, string.Empty);
				return;
			}
			base.RenderSubject(true);
		}

		protected bool IsDefaultFolderToBeShared
		{
			get
			{
				return this.sharingMessage.IsSharedFolderPrimary;
			}
		}

		protected void RenderSharingErrorEnum()
		{
			RenderingUtilities.RenderInteger(base.SanitizingResponse, "a_SHARE_ERROR_FOLDER_NOT_EXIST", 1);
			RenderingUtilities.RenderInteger(base.SanitizingResponse, "a_SHARE_ERROR_INVALID_RECIPIENTS", 2);
			RenderingUtilities.RenderInteger(base.SanitizingResponse, "a_SHARE_ERROR_PUBLISH_AND_TRY_AGAIN", 3);
			RenderingUtilities.RenderInteger(base.SanitizingResponse, "a_SHARE_ERROR_SEND_PUBLISH_LINKS", 4);
		}

		protected bool IsInvitationOrRequest
		{
			get
			{
				return this.sharingMessage.SharingMessageType.IsInvitationOrRequest;
			}
		}

		protected bool IsSharingInvitation
		{
			get
			{
				return this.sharingMessage.SharingMessageType == SharingMessageType.Invitation || this.sharingMessage.SharingMessageType == SharingMessageType.InvitationAndRequest;
			}
		}

		protected bool IsAllowOfRequest
		{
			get
			{
				return this.sharingMessage.SharingMessageType == SharingMessageType.AcceptOfRequest;
			}
		}

		protected bool IsResponseToRequest
		{
			get
			{
				return this.sharingMessage.SharingMessageType.IsResponseToRequest;
			}
		}

		protected override bool ShowBcc
		{
			get
			{
				return false;
			}
		}

		protected override bool ShowFrom
		{
			get
			{
				return false;
			}
		}

		protected override bool IsPageCacheable
		{
			get
			{
				return false;
			}
		}

		protected bool IsPublishing
		{
			get
			{
				return this.isPublishing;
			}
		}

		protected override void InitializeMessage()
		{
			base.Item = (base.Message = (this.sharingMessage = base.Initialize<SharingMessageItem>(false, EditMessage.PrefetchProperties)));
			if (this.sharingMessage != null)
			{
				this.isPublishing = this.sharingMessage.IsPublishing;
			}
		}

		protected override void CreateDraftMessage()
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "fldId");
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromString(queryStringParameter);
			if (owaStoreObjectId.OwaStoreObjectIdType != OwaStoreObjectIdType.MailBoxObject)
			{
				throw new OwaInvalidRequestException("Cannot share this calendar");
			}
			try
			{
				if (this.isPublishing)
				{
					try
					{
						this.sharingMessage = SharingMessageItem.CreateForPublishing(base.UserContext.MailboxSession, base.UserContext.DraftsFolderId, owaStoreObjectId.StoreId);
						goto IL_90;
					}
					catch (FolderNotPublishedException innerException)
					{
						throw new OwaInvalidRequestException("Folder is not published", innerException);
					}
				}
				this.sharingMessage = SharingMessageItem.Create(base.UserContext.MailboxSession, base.UserContext.DraftsFolderId, owaStoreObjectId.StoreId);
				IL_90:
				base.Item = (base.Message = this.sharingMessage);
			}
			catch (InvalidOperationException innerException2)
			{
				ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "Failed to create sharing message: no compatible provider for the share folder was found.");
				throw new OwaInvalidRequestException("Failed to create sharing message: no compatible provider for the share folder was found.", innerException2);
			}
			catch (CannotShareFolderException innerException3)
			{
				throw new OwaInvalidRequestException("Failed to create sharing message:  This folder has been shared with you and can't be shared with any other recipients.", innerException3);
			}
			this.sharingMessage.Save(SaveMode.ResolveConflicts);
			this.sharingMessage.Load();
			this.newItemType = NewItemType.ImplicitDraft;
			base.DeleteExistingDraft = true;
			if (this.IsDefaultFolderToBeShared)
			{
				this.sharingMessage.Subject = string.Format(base.UserContext.UserCulture, LocalizedStrings.GetNonEncoded(this.isPublishing ? -1266131020 : -2015069592), new object[0]);
				return;
			}
			this.sharingMessage.Subject = string.Format(base.UserContext.UserCulture, LocalizedStrings.GetNonEncoded(this.isPublishing ? -1899057801 : 222110147), new object[]
			{
				this.sharingMessage.SharedFolderName
			});
		}

		protected override bool PageSupportSmime
		{
			get
			{
				return false;
			}
		}

		protected override int CurrentStoreObjectType
		{
			get
			{
				return 28;
			}
		}

		protected override EditMessageToolbar BuildToolbar()
		{
			return new EditSharingMessageToolbar(this.sharingMessage.Importance, this.bodyMarkup, this.isPublishing);
		}

		private const string SharedFolderIdParameter = "fldId";

		private const string PublishParameter = "publish";

		private SharingMessageItem sharingMessage;

		private SharingMessageWriter sharingMessageWriter;

		private bool isPublishing;
	}
}
