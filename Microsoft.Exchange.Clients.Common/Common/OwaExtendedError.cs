using System;
using System.Collections.Specialized;
using System.Net;

namespace Microsoft.Exchange.Clients.Common
{
	public static class OwaExtendedError
	{
		public static void SendError(NameValueCollection headerCollection, Action<HttpStatusCode> responseCodeSetter, OwaExtendedErrorCode code, string message = "", string user = "", string extra = "")
		{
			string name = "X-OWA-ExtendedErrorCode";
			int num = (int)code;
			headerCollection.Set(name, num.ToString());
			headerCollection.Set("X-OWA-ExtendedErrorMessage", message ?? string.Empty);
			headerCollection.Set("X-OWA-ExtendedErrorUser", user ?? string.Empty);
			headerCollection.Set("X-OWA-ExtendedErrorData", extra ?? string.Empty);
			responseCodeSetter(HttpStatusCode.PreconditionFailed);
		}

		public const HttpStatusCode ExtendedErrorStatusCode = HttpStatusCode.PreconditionFailed;

		public const string ExtendedErrorCodeHeader = "X-OWA-ExtendedErrorCode";

		public const string ExtendedErrorMessageHeader = "X-OWA-ExtendedErrorMessage";

		public const string ExtendedErrorUserHeader = "X-OWA-ExtendedErrorUser";

		public const string ExtendedErrorDataHeader = "X-OWA-ExtendedErrorData";
	}
}
