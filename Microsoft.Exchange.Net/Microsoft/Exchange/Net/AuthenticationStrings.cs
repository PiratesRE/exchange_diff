using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	internal static class AuthenticationStrings
	{
		static AuthenticationStrings()
		{
			AuthenticationStrings.stringIDs.Add(1520490179U, "AuthenticationException");
		}

		public static LocalizedString AuthenticationException
		{
			get
			{
				return new LocalizedString("AuthenticationException", "", false, false, AuthenticationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaximumUriRedirectionsReachedException(int maximumUriRedirections)
		{
			return new LocalizedString("MaximumUriRedirectionsReachedException", "", false, false, AuthenticationStrings.ResourceManager, new object[]
			{
				maximumUriRedirections
			});
		}

		public static LocalizedString GetLocalizedString(AuthenticationStrings.IDs key)
		{
			return new LocalizedString(AuthenticationStrings.stringIDs[(uint)key], AuthenticationStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Net.AuthenticationStrings", typeof(AuthenticationStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			AuthenticationException = 1520490179U
		}

		private enum ParamIDs
		{
			MaximumUriRedirectionsReachedException
		}
	}
}
