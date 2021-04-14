using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ItemReadADObjectPreFormAction : IPreFormAction
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
			HttpContext httpContext = owaContext.HttpContext;
			string queryStringParameter = Utilities.GetQueryStringParameter(httpContext.Request, "lDn", true);
			IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, owaContext.UserContext);
			Result<ADRawEntry>[] array = recipientSession.FindByLegacyExchangeDNs(new string[]
			{
				queryStringParameter
			}, ItemReadADObjectPreFormAction.recipientQueryProperties);
			if (array == null || array.Length != 1)
			{
				throw new OwaADObjectNotFoundException();
			}
			ADRawEntry data = array[0].Data;
			ADObjectId adobjectId = null;
			if (data != null)
			{
				adobjectId = (ADObjectId)data[ADObjectSchema.Id];
			}
			if (adobjectId == null)
			{
				throw new OwaADObjectNotFoundException();
			}
			preFormActionResponse.ApplicationElement = ApplicationElement.Item;
			preFormActionResponse.Action = string.Empty;
			preFormActionResponse.AddParameter("id", Convert.ToBase64String(adobjectId.ObjectGuid.ToByteArray()));
			if (Utilities.IsADDistributionList((MultiValuedProperty<string>)data[ADObjectSchema.ObjectClass]))
			{
				preFormActionResponse.Type = "ADDistList";
			}
			else
			{
				preFormActionResponse.Type = "AD.RecipientType.User";
			}
			return preFormActionResponse;
		}

		private const string LegacyDnParameter = "lDn";

		private static PropertyDefinition[] recipientQueryProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.PrimarySmtpAddress,
			ADObjectSchema.Id,
			ADRecipientSchema.Alias,
			ADRecipientSchema.RecipientDisplayType,
			ADObjectSchema.ObjectClass
		};
	}
}
