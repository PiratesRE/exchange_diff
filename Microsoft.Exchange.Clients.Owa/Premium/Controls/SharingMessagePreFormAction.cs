using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class SharingMessagePreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			applicationElement = ApplicationElement.Item;
			type = "IPM.Note";
			action = this.Action;
			state = null;
			string queryStringParameter = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "id", true);
			string queryStringParameter2 = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "t", true);
			UserContext userContext = owaContext.UserContext;
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromString(queryStringParameter);
			if (queryStringParameter2 == "IPM.Sharing" && !owaStoreObjectId.IsPublic)
			{
				using (SharingMessageItem item = Utilities.GetItem<SharingMessageItem>(userContext, owaStoreObjectId, new PropertyDefinition[0]))
				{
					StoreObjectId parentId = item.ParentId;
					bool flag = Utilities.IsItemInDefaultFolder(item, DefaultFolderType.JunkEmail);
					DefaultFolderType defaultFolderType = DefaultFolderType.None;
					try
					{
						defaultFolderType = item.SharedFolderType;
					}
					catch (NotSupportedSharingMessageException ex)
					{
						ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Cannot handle this sharing message due to {0}", ex.Message);
					}
					if (defaultFolderType == DefaultFolderType.Calendar && !flag)
					{
						type = "IPM.Sharing";
						if (item.IsDraft)
						{
							state = "Draft";
						}
					}
				}
			}
			return null;
		}

		protected abstract string Action { get; }
	}
}
