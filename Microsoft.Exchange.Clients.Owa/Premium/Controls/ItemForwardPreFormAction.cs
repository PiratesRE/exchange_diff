using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ItemForwardPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			applicationElement = ApplicationElement.NotSet;
			type = string.Empty;
			action = string.Empty;
			state = string.Empty;
			Item item = null;
			Item item2 = null;
			Item item3 = null;
			bool flag = false;
			BodyFormat bodyFormat = BodyFormat.TextPlain;
			PreFormActionResponse result;
			try
			{
				HttpContext httpContext = owaContext.HttpContext;
				UserContext userContext = owaContext.UserContext;
				PreFormActionResponse preFormActionResponse = new PreFormActionResponse(httpContext.Request, new string[]
				{
					"cb",
					"smime"
				});
				item = ReplyForwardUtilities.GetItemForRequest(owaContext, out item2);
				if (item != null && !ItemUtility.IsForwardSupported(item))
				{
					throw new OwaInvalidRequestException("Forwarding of such a type item is not supported.");
				}
				CalendarItemBase calendarItemBase = item as CalendarItemBase;
				if (item is Task || item is ContactBase || (calendarItemBase != null && !calendarItemBase.IsMeeting))
				{
					item3 = ReplyForwardUtilities.CreateForwardMessageWithItemAttached(item, userContext);
					preFormActionResponse.Action = "New";
					preFormActionResponse.AddParameter("exdltdrft", "1");
				}
				else
				{
					bool flag2 = false;
					string queryStringParameter = Utilities.GetQueryStringParameter(httpContext.Request, "smime", false);
					RightsManagedMessageDecryptionStatus decryptionStatus = RightsManagedMessageDecryptionStatus.FeatureDisabled;
					if (userContext.IsIrmEnabled)
					{
						try
						{
							flag = Utilities.IrmDecryptForReplyForward(owaContext, ref item, ref item2, ref bodyFormat, out decryptionStatus);
						}
						catch (RightsManagementPermanentException exception)
						{
							decryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception);
						}
					}
					if (!flag)
					{
						bodyFormat = ReplyForwardUtilities.GetReplyForwardBodyFormat(item, userContext);
					}
					bool flag3 = Utilities.IsSMimeControlNeededForEditForm(queryStringParameter, owaContext);
					flag2 = ((flag3 && Utilities.IsSMime(item)) || flag);
					bool flag4 = userContext.IsSmsEnabled && ObjectClass.IsSmsMessage(owaContext.FormsRegistryContext.Type);
					ReplyForwardFlags replyForwardFlags = ReplyForwardFlags.None;
					if (flag2)
					{
						replyForwardFlags |= ReplyForwardFlags.DropBody;
					}
					if (flag4 || flag)
					{
						replyForwardFlags |= ReplyForwardFlags.DropHeader;
					}
					StoreObjectId parentFolderId = Utilities.GetParentFolderId(item2, item);
					item3 = ReplyForwardUtilities.CreateForwardItem(flag3 ? BodyFormat.TextHtml : bodyFormat, item, replyForwardFlags, userContext, parentFolderId);
					if (flag)
					{
						item3.AttachmentCollection.RemoveAll();
						using (ItemAttachment itemAttachment = item3.AttachmentCollection.AddExistingItem(item))
						{
							itemAttachment.Save();
							goto IL_205;
						}
					}
					if (Utilities.IsIrmRestrictedAndNotDecrypted(item))
					{
						ReplyForwardUtilities.SetAlternateIrmBody(item3, flag3 ? BodyFormat.TextHtml : bodyFormat, userContext, parentFolderId, decryptionStatus, ObjectClass.IsVoiceMessage(item.ClassName));
						item3.AttachmentCollection.RemoveAll();
					}
					IL_205:
					preFormActionResponse.Action = "Forward";
					if (flag2)
					{
						preFormActionResponse.AddParameter("srcId", Utilities.GetItemIdQueryString(httpContext.Request));
						if (Utilities.GetQueryStringParameter(httpContext.Request, "cb", false) == null && Utilities.IsWebBeaconsAllowed(item))
						{
							preFormActionResponse.AddParameter("cb", "1");
						}
					}
					if (flag4)
					{
						item3.ClassName = "IPM.Note.Mobile.SMS";
					}
				}
				item3.Save(SaveMode.ResolveConflicts);
				item3.Load();
				ReplyForwardUtilities.DeleteLevelOneAttachments(item3, userContext);
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = item3.ClassName;
				preFormActionResponse.AddParameter("id", OwaStoreObjectId.CreateFromStoreObject(item3).ToBase64String());
				if (userContext.IsInOtherMailbox(item))
				{
					preFormActionResponse.AddParameter("fOMF", "1");
				}
				if (flag)
				{
					preFormActionResponse.AddParameter("fIrmAsAttach", "1");
				}
				result = preFormActionResponse;
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
				if (item3 != null)
				{
					item3.Dispose();
					item3 = null;
				}
			}
			return result;
		}
	}
}
