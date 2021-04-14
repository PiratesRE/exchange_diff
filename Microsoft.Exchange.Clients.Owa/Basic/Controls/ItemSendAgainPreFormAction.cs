using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class ItemSendAgainPreFormAction : IPreFormAction
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
			MessageItem messageItem = null;
			try
			{
				HttpContext httpContext = owaContext.HttpContext;
				UserContext userContext = owaContext.UserContext;
				PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
				{
					ItemSchema.SentTime
				};
				item = Utilities.GetItemForRequest<MessageItem>(owaContext, out item2, prefetchProperties);
				messageItem = ((ReportMessage)item).CreateSendAgain(userContext.DraftsFolderId);
				if (messageItem is RightsManagedMessageItem)
				{
					SanitizedHtmlString sanitizedHtmlString = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1049269714), new object[]
					{
						Utilities.GetOfficeDownloadAnchor(BodyFormat.TextPlain, userContext.UserCulture)
					});
					throw new OwaCannotEditIrmDraftException(sanitizedHtmlString.ToString());
				}
				messageItem.Save(SaveMode.ResolveConflicts);
				messageItem.Load();
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = messageItem.ClassName;
				preFormActionResponse.Action = "Open";
				preFormActionResponse.State = "Draft";
				preFormActionResponse.AddParameter("id", messageItem.Id.ObjectId.ToBase64String());
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
				if (messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
				if (item2 != null)
				{
					item2.Dispose();
					item2 = null;
				}
			}
			return preFormActionResponse;
		}
	}
}
