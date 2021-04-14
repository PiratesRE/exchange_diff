using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class ReadMessagePreFormAction : IPreFormAction
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
			HttpContext httpContext = owaContext.HttpContext;
			UserContext userContext = owaContext.UserContext;
			HttpRequest request = httpContext.Request;
			if (!Utilities.IsPostRequest(request) && owaContext.FormsRegistryContext.Action != "Prev" && owaContext.FormsRegistryContext.Action != "Next")
			{
				return userContext.LastClientViewState.ToPreFormActionResponse();
			}
			string storeObjectId;
			string storeObjectId2;
			if (Utilities.IsPostRequest(request))
			{
				storeObjectId = Utilities.GetFormParameter(request, "hidfldid", true);
				storeObjectId2 = Utilities.GetFormParameter(request, "hidid", true);
			}
			else
			{
				storeObjectId = Utilities.GetQueryStringParameter(request, "fId", true);
				storeObjectId2 = Utilities.GetQueryStringParameter(request, "id", true);
			}
			StoreObjectId folderId = Utilities.CreateStoreObjectId(userContext.MailboxSession, storeObjectId);
			StoreObjectId storeObjectId3 = Utilities.CreateStoreObjectId(userContext.MailboxSession, storeObjectId2);
			ItemOperations.Result result = null;
			string action2;
			if ((action2 = owaContext.FormsRegistryContext.Action) != null)
			{
				if (!(action2 == "Prev"))
				{
					if (!(action2 == "Next"))
					{
						if (!(action2 == "Del"))
						{
							if (!(action2 == "Junk"))
							{
								if (!(action2 == "NotJunk"))
								{
									goto IL_1FA;
								}
								if (!userContext.IsJunkEmailEnabled)
								{
									throw new OwaInvalidRequestException(LocalizedStrings.GetNonEncoded(552277155));
								}
								owaContext[OwaContextProperty.InfobarMessage] = JunkEmailHelper.MarkAsNotJunk(userContext, new StoreObjectId[]
								{
									storeObjectId3
								});
								userContext.ForceNewSearch = true;
							}
							else
							{
								if (!userContext.IsJunkEmailEnabled)
								{
									throw new OwaInvalidRequestException(LocalizedStrings.GetNonEncoded(552277155));
								}
								owaContext[OwaContextProperty.InfobarMessage] = JunkEmailHelper.MarkAsJunk(userContext, new StoreObjectId[]
								{
									storeObjectId3
								});
								userContext.ForceNewSearch = true;
							}
						}
						else
						{
							result = ItemOperations.DeleteItem(userContext, storeObjectId3, folderId);
							userContext.ForceNewSearch = true;
						}
					}
					else
					{
						result = ItemOperations.GetNextViewItem(userContext, ItemOperations.Action.Next, storeObjectId3, folderId);
					}
				}
				else
				{
					result = ItemOperations.GetNextViewItem(userContext, ItemOperations.Action.Prev, storeObjectId3, folderId);
				}
				return ItemOperations.GetPreFormActionResponse(userContext, result);
			}
			IL_1FA:
			throw new OwaInvalidRequestException("Unknown command");
		}

		private const string QueryStringMessageId = "id";

		private const string QueryStringFolderId = "fId";

		private const string FormMessageId = "hidid";

		private const string FormFolderId = "hidfldid";

		private const string PreviousItemAction = "Prev";

		private const string NextItemAction = "Next";

		private const string DeleteAction = "Del";

		private const string JunkAction = "Junk";

		private const string NotJunkAction = "NotJunk";
	}
}
