using System;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Configuration.CertificateAuthentication.LocStrings
{
	internal static class Strings
	{
		public static string UserNotFound(string certSubject)
		{
			return string.Format(Strings.ResourceManager.GetString("UserNotFound"), certSubject);
		}

		public static string ADTransientError(string certSubject)
		{
			return string.Format(Strings.ResourceManager.GetString("ADTransientError"), certSubject);
		}

		public static string UnknownInternalError(string certSubject)
		{
			return string.Format(Strings.ResourceManager.GetString("UnknownInternalError"), certSubject);
		}

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Configuration.CertificateAuthentication.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			UserNotFound,
			ADTransientError,
			UnknownInternalError
		}
	}
}
