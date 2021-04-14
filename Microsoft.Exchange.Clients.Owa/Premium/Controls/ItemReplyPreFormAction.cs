using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ItemReplyPreFormAction : IPreFormAction
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
				item = ReplyForwardUtilities.GetItemForRequest(owaContext, out item2);
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
				string queryStringParameter = Utilities.GetQueryStringParameter(httpContext.Request, "smime", false);
				bool flag2 = Utilities.IsSMimeControlNeededForEditForm(queryStringParameter, owaContext);
				bool flag3 = userContext.IsSmsEnabled && ObjectClass.IsSmsMessage(owaContext.FormsRegistryContext.Type);
				bool flag4 = flag3 || (flag2 && Utilities.IsSMime(item)) || flag;
				ReplyForwardFlags replyForwardFlags = ReplyForwardFlags.None;
				if (flag4)
				{
					replyForwardFlags |= ReplyForwardFlags.DropBody;
				}
				if (flag3 || flag)
				{
					replyForwardFlags |= ReplyForwardFlags.DropHeader;
				}
				StoreObjectId parentFolderId = Utilities.GetParentFolderId(item2, item);
				item3 = ReplyForwardUtilities.CreateReplyItem(flag2 ? BodyFormat.TextHtml : bodyFormat, item, replyForwardFlags, userContext, parentFolderId);
				if (flag)
				{
					using (ItemAttachment itemAttachment = item3.AttachmentCollection.AddExistingItem(item))
					{
						itemAttachment.Save();
						goto IL_160;
					}
				}
				if (Utilities.IsIrmRestrictedAndNotDecrypted(item))
				{
					ReplyForwardUtilities.SetAlternateIrmBody(item3, flag2 ? BodyFormat.TextHtml : bodyFormat, userContext, parentFolderId, decryptionStatus, ObjectClass.IsVoiceMessage(item.ClassName));
				}
				IL_160:
				type = "IPM.Note";
				if (flag3)
				{
					item3.ClassName = "IPM.Note.Mobile.SMS";
					type = "IPM.Note.Mobile.SMS";
					ReplyForwardUtilities.RemoveInvalidRecipientsFromSmsMessage((MessageItem)item3);
				}
				item3.Save(SaveMode.ResolveConflicts);
				item3.Load();
				PreFormActionResponse preFormActionResponse = new PreFormActionResponse(httpContext.Request, new string[]
				{
					"cb",
					"smime"
				});
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = type;
				preFormActionResponse.Action = "Reply";
				preFormActionResponse.AddParameter("id", OwaStoreObjectId.CreateFromStoreObject(item3).ToBase64String());
				if (flag4)
				{
					preFormActionResponse.AddParameter("srcId", Utilities.GetItemIdQueryString(httpContext.Request));
					if (Utilities.GetQueryStringParameter(httpContext.Request, "cb", false) == null && Utilities.IsWebBeaconsAllowed(item))
					{
						preFormActionResponse.AddParameter("cb", "1");
					}
				}
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
