using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class HttpProxyStrings
	{
		static HttpProxyStrings()
		{
			HttpProxyStrings.stringIDs.Add(594155080U, "ErrorInternalServerError");
			HttpProxyStrings.stringIDs.Add(3579904699U, "ErrorAccessDenied");
		}

		public static LocalizedString ErrorInternalServerError
		{
			get
			{
				return new LocalizedString("ErrorInternalServerError", HttpProxyStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAccessDenied
		{
			get
			{
				return new LocalizedString("ErrorAccessDenied", HttpProxyStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(HttpProxyStrings.IDs key)
		{
			return new LocalizedString(HttpProxyStrings.stringIDs[(uint)key], HttpProxyStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.HttpProxy.Strings", typeof(HttpProxyStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorInternalServerError = 594155080U,
			ErrorAccessDenied = 3579904699U
		}
	}
}
