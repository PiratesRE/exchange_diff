using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ItemPostReplyPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			applicationElement = ApplicationElement.Item;
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
				string queryStringParameter = Utilities.GetQueryStringParameter(httpContext.Request, "fId", true);
				item = ReplyForwardUtilities.GetItemForRequest(owaContext, out item2);
				BodyFormat replyForwardBodyFormat = ReplyForwardUtilities.GetReplyForwardBodyFormat(item, userContext);
				item3 = ReplyForwardUtilities.CreatePostReplyItem(replyForwardBodyFormat, item as PostItem, userContext, Utilities.GetParentFolderId(item2, item));
				item3.Save(SaveMode.ResolveConflicts);
				item3.Load();
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = "IPM.Post";
				preFormActionResponse.Action = "PostReply";
				preFormActionResponse.AddParameter("Id", OwaStoreObjectId.CreateFromStoreObject(item3).ToBase64String());
				preFormActionResponse.AddParameter("fId", queryStringParameter);
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
