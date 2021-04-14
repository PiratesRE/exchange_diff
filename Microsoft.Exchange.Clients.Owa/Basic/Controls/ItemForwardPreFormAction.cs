using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
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
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			Item item = null;
			Item item2 = null;
			Item item3 = null;
			try
			{
				HttpContext httpContext = owaContext.HttpContext;
				UserContext userContext = owaContext.UserContext;
				item = ReplyForwardUtilities.GetItemForRequest(owaContext, out item2);
				if (!(item is CalendarItemBase) && !(item is MessageItem))
				{
					item3 = ReplyForwardUtilities.CreateForwardMessageWithItemAttached(item, userContext);
				}
				else
				{
					item3 = ReplyForwardUtilities.CreateForwardItem(BodyFormat.TextPlain, item, ReplyForwardFlags.None, userContext, Utilities.GetParentFolderId(item2, item));
				}
				AttachmentUtility.PromoteInlineAttachments(item3);
				item3.Save(SaveMode.ResolveConflicts);
				item3.Load();
				ReplyForwardUtilities.DeleteLevelOneAttachments(item3, userContext);
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = item3.ClassName;
				preFormActionResponse.Action = "Forward";
				preFormActionResponse.AddParameter("id", item3.Id.ObjectId.ToBase64String());
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
			return preFormActionResponse;
		}
	}
}
