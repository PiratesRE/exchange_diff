using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class EmbeddedItemOpenPreFormAction : IPreFormAction
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
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = owaContext.FormsRegistryContext.Type;
				preFormActionResponse.State = owaContext.FormsRegistryContext.State;
				preFormActionResponse.Action = "New";
				string type2;
				if ((type2 = preFormActionResponse.Type) != null)
				{
					StoreObjectId storeObjectId;
					if (!(type2 == "IPM.Contact"))
					{
						if (!(type2 == "IPM.DistList"))
						{
							if (!(type2 == "IPM.Task"))
							{
								goto IL_102;
							}
							storeObjectId = owaContext.UserContext.TasksFolderId;
							item = Utilities.GetItemForRequest<Task>(owaContext, out item2, false, new PropertyDefinition[0]);
						}
						else
						{
							storeObjectId = owaContext.UserContext.ContactsFolderId;
							item = Utilities.GetItemForRequest<DistributionList>(owaContext, out item2, false, new PropertyDefinition[0]);
						}
					}
					else
					{
						storeObjectId = owaContext.UserContext.ContactsFolderId;
						item = Utilities.GetItemForRequest<Contact>(owaContext, out item2, false, new PropertyDefinition[0]);
					}
					if (item.MapiMessage == null)
					{
						item3 = MessageItem.Create(owaContext.UserContext.MailboxSession, storeObjectId);
						Item.CopyItemContent(item, item3);
					}
					else
					{
						item3 = Item.CloneItem(owaContext.UserContext.MailboxSession, storeObjectId, item, false, false, null);
					}
					Utilities.SaveItem(item3);
					item3.Load();
					HttpRequest request = owaContext.HttpContext.Request;
					if (Utilities.GetQueryStringParameter(request, "smemb", false) != null)
					{
						Utilities.Delete(owaContext.UserContext, true, false, new OwaStoreObjectId[]
						{
							OwaStoreObjectId.CreateFromStoreObject(item)
						});
					}
					preFormActionResponse.AddParameter("id", item3.Id.ObjectId.ToBase64String());
					preFormActionResponse.AddParameter("exdltdrft", "1");
					return preFormActionResponse;
				}
				IL_102:
				throw new OwaInvalidRequestException("Item should be contact or PDL or task.");
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

		private const string Id = "id";

		private const string SMimeEmbeddedFlag = "smemb";
	}
}
