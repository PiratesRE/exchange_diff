using System;
using System.Net;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class Testability
	{
		public static bool HandleSmtpAddressAsContact(string checkAddress)
		{
			return Testability.SmtpAddressAsContacts != null && Array.Exists<string>(Testability.SmtpAddressAsContacts, (string address) => StringComparer.OrdinalIgnoreCase.Equals(address, checkAddress));
		}

		internal static NetworkCredential WebServiceCredentials;

		public static string[] SmtpAddressAsContacts;

		public class TestWebServiceCredential : IDisposable
		{
			public TestWebServiceCredential(NetworkCredential credential)
			{
				Testability.WebServiceCredentials = credential;
			}

			public void Dispose()
			{
				Testability.WebServiceCredentials = null;
			}
		}
	}
}
