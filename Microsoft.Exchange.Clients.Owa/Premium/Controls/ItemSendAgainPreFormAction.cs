using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ItemSendAgainPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			HttpContext httpContext = owaContext.HttpContext;
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			applicationElement = ApplicationElement.NotSet;
			type = string.Empty;
			action = string.Empty;
			state = string.Empty;
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse(httpContext.Request, new string[]
			{
				"smime"
			});
			Item item = null;
			Item item2 = null;
			Item item3 = null;
			try
			{
				UserContext userContext = owaContext.UserContext;
				PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
				{
					ItemSchema.SentTime
				};
				item = Utilities.GetItemForRequest<MessageItem>(owaContext, out item3, prefetchProperties);
				item2 = ((ReportMessage)item).CreateSendAgain(userContext.DraftsFolderId);
				if (Utilities.IrmDecryptIfRestricted(item2, userContext, true))
				{
					((RightsManagedMessageItem)item2).PrepareAcquiredLicensesBeforeSave();
				}
				item2.Save(SaveMode.ResolveConflicts);
				item2.Load();
				owaContext.PreFormActionId = OwaStoreObjectId.CreateFromStoreObject(item2);
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = item2.ClassName;
				preFormActionResponse.Action = "Open";
				preFormActionResponse.State = "Draft";
				preFormActionResponse.AddParameter("exdltdrft", "1");
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
				if (item3 != null)
				{
					item3.Dispose();
					item3 = null;
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
