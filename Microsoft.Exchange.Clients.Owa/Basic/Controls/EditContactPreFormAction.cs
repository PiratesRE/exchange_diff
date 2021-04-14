using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class EditContactPreFormAction : IPreFormAction
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
			HttpRequest request = httpContext.Request;
			UserContext userContext = owaContext.UserContext;
			if (!Utilities.IsPostRequest(request))
			{
				return userContext.LastClientViewState.ToPreFormActionResponse();
			}
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			string strA = string.Empty;
			if (owaContext.FormsRegistryContext.Action != null)
			{
				strA = owaContext.FormsRegistryContext.Action;
			}
			string queryStringParameter = Utilities.GetQueryStringParameter(request, "pa", false);
			using (EditContactHelper editContactHelper = new EditContactHelper(userContext, request, true))
			{
				SaveResult saveResult = SaveResult.Success;
				bool flag = false;
				try
				{
					saveResult = editContactHelper.SaveContact();
				}
				catch (QuotaExceededException)
				{
					throw;
				}
				catch (StoragePermanentException)
				{
					flag = true;
				}
				catch (StorageTransientException)
				{
					flag = true;
				}
				if (flag)
				{
					EditContactPreFormAction.RedirectToCompose(editContactHelper, owaContext, preFormActionResponse, LocalizedStrings.GetNonEncoded(1242051590));
				}
				else if (saveResult == SaveResult.IrresolvableConflict)
				{
					EditContactPreFormAction.RedirectToCompose(editContactHelper, owaContext, preFormActionResponse, LocalizedStrings.GetNonEncoded(-482397486));
				}
				else if (string.Compare(strA, "Attach", StringComparison.OrdinalIgnoreCase) == 0)
				{
					EditContactPreFormAction.RedirectToAttachmentManager(editContactHelper, owaContext, preFormActionResponse);
				}
				else if (string.Compare(queryStringParameter, "Open", StringComparison.OrdinalIgnoreCase) == 0)
				{
					EditContactPreFormAction.RedirectToReadContact(editContactHelper, owaContext, preFormActionResponse);
				}
				else
				{
					preFormActionResponse = userContext.LastClientViewState.ToPreFormActionResponse();
				}
			}
			return preFormActionResponse;
		}

		private static void RedirectToCompose(EditContactHelper helper, OwaContext owaContext, PreFormActionResponse response, string errorMessage)
		{
			if (!string.IsNullOrEmpty(errorMessage))
			{
				owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateText(errorMessage, InfobarMessageType.Error);
			}
			helper.Contact.Load();
			response.ApplicationElement = ApplicationElement.Item;
			response.Type = "IPM.Contact";
			response.Action = "Open";
			response.State = "Draft";
			if (helper.Contact.Id != null)
			{
				response.AddParameter("id", helper.Contact.Id.ObjectId.ToBase64String());
			}
		}

		private static void RedirectToAttachmentManager(EditContactHelper helper, OwaContext owaContext, PreFormActionResponse response)
		{
			helper.Contact.Load();
			response.ApplicationElement = ApplicationElement.Dialog;
			response.Type = "Attach";
			response.AddParameter("id", helper.Contact.Id.ObjectId.ToBase64String());
		}

		private static void RedirectToReadContact(EditContactHelper helper, OwaContext owaContext, PreFormActionResponse response)
		{
			helper.Contact.Load();
			response.ApplicationElement = ApplicationElement.Item;
			response.Type = "IPM.Contact";
			response.AddParameter("id", helper.Contact.Id.ObjectId.ToBase64String());
		}
	}
}
