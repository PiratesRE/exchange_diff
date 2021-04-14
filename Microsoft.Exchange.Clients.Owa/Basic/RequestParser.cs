using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal sealed class RequestParser
	{
		public static int GetIntValueFromQueryString(HttpRequest request, string parameterName)
		{
			return RequestParser.GetIntValueFromRequest(request, parameterName, ParameterIn.QueryString, true, 0);
		}

		public static int TryGetIntValueFromQueryString(HttpRequest request, string parameterName, int defaultValue)
		{
			return RequestParser.GetIntValueFromRequest(request, parameterName, ParameterIn.QueryString, false, defaultValue);
		}

		public static int GetIntValueFromForm(HttpRequest request, string parameterName)
		{
			return RequestParser.GetIntValueFromRequest(request, parameterName, ParameterIn.Form, true, 0);
		}

		public static int TryGetIntValueFromForm(HttpRequest request, string parameterName, int defaultValue)
		{
			return RequestParser.GetIntValueFromRequest(request, parameterName, ParameterIn.Form, false, defaultValue);
		}

		public static int GetIntValueFromRequest(HttpRequest request, string parameterName, ParameterIn parameterIn, bool required, int defaultValue)
		{
			if (request == null)
			{
				throw new ArgumentNullException("HttpRequest");
			}
			if (string.IsNullOrEmpty(parameterName))
			{
				throw new ArgumentException("parameterName is null or empty.");
			}
			string text = null;
			if (parameterIn == ParameterIn.QueryString)
			{
				text = Utilities.GetQueryStringParameter(request, parameterName, required);
				if (text == null)
				{
					return defaultValue;
				}
			}
			else if (parameterIn == ParameterIn.Form)
			{
				text = Utilities.GetFormParameter(request, parameterName, required);
				if (text == null || (!required && string.IsNullOrEmpty(text)))
				{
					return defaultValue;
				}
			}
			int result;
			if (!int.TryParse(text, out result))
			{
				throw new OwaInvalidRequestException(parameterName + " should be a valid number");
			}
			return result;
		}

		public static NavigationModule GetNavigationModuleFromQueryString(HttpRequest request, NavigationModule defaultModule, bool required)
		{
			return (NavigationModule)RequestParser.GetIntValueFromRequest(request, "m", ParameterIn.QueryString, required, (int)defaultModule);
		}

		public static StoreObjectId GetFolderIdFromQueryString(HttpRequest request, bool required)
		{
			return RequestParser.GetStoreObjectId(request, "fid", required, ParameterIn.QueryString);
		}

		public static StoreObjectId GetTargetFolderIdFromQueryString(HttpRequest request, bool required)
		{
			return RequestParser.GetStoreObjectId(request, "tfId", required, ParameterIn.Form);
		}

		public static StoreObjectId GetStoreObjectId(HttpRequest request, string parameterName, bool required, ParameterIn parameterIn)
		{
			string text;
			if (parameterIn == ParameterIn.QueryString)
			{
				text = Utilities.GetQueryStringParameter(request, parameterName, required);
			}
			else
			{
				text = Utilities.GetFormParameter(request, parameterName, required);
			}
			if (text == null)
			{
				return null;
			}
			return Utilities.CreateStoreObjectId(UserContextManager.GetUserContext().MailboxSession, text);
		}

		public static StoreObjectId[] GetStoreObjectIdsFromForm(HttpRequest request, bool required)
		{
			return RequestParser.GetStoreObjectIdsFromForm(request, "hidid", required);
		}

		public static StoreObjectId[] GetStoreObjectIdsFromForm(HttpRequest request, string parameterName, bool required)
		{
			UserContext userContext = UserContextManager.GetUserContext();
			string formParameter = Utilities.GetFormParameter(request, parameterName, required);
			if (formParameter == null && !required)
			{
				return null;
			}
			string[] array = formParameter.Split(new char[]
			{
				','
			});
			if (userContext.UserOptions.BasicViewRowCount < array.Length)
			{
				throw new OwaInvalidRequestException("According to the user's option, at most " + userContext.UserOptions.BasicViewRowCount + " items are allow to be selected at one time");
			}
			StoreObjectId[] array2 = new StoreObjectId[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = Utilities.CreateStoreObjectId(userContext.MailboxSession, array[i]);
			}
			return array2;
		}

		public const string FolderIdQueryParameter = "fid";

		public const string ItemIdQueryParameter = "id";

		public const string ItemIdFormParameter = "hidid";

		public const string TargetFolderIdFormParameter = "tfId";

		public const string NavigationModuleQueryParameter = "m";

		public const string ItemClassNameFormParameter = "hidt";
	}
}
