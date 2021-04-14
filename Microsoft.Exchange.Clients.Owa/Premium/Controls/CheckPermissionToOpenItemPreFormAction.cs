using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class CheckPermissionToOpenItemPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			applicationElement = ApplicationElement.NotSet;
			type = string.Empty;
			state = string.Empty;
			action = string.Empty;
			Item item = null;
			Item item2 = null;
			PreFormActionResponse result;
			try
			{
				item = Utilities.GetItemForRequest<Item>(owaContext, out item2, new PropertyDefinition[]
				{
					StoreObjectSchema.EffectiveRights
				});
				bool flag = ItemUtility.UserCanEditItem(item);
				PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
				preFormActionResponse.ApplicationElement = ApplicationElement.Item;
				preFormActionResponse.Type = owaContext.FormsRegistryContext.Type;
				preFormActionResponse.AddParameter("id", Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "id"));
				if (flag)
				{
					preFormActionResponse.Action = "Open";
					preFormActionResponse.State = "Draft";
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
			}
			return result;
		}
	}
}
