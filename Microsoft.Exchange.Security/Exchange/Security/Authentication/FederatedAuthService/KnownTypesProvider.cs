using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal static class KnownTypesProvider
	{
		public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
		{
			return new List<Type>();
		}
	}
}
