using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.CallRouter.Exceptions
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3452399285U, "Server");
			Strings.stringIDs.Add(1422214618U, "ServiceName");
		}

		public static LocalizedString Server
		{
			get
			{
				return new LocalizedString("Server", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServiceName
		{
			get
			{
				return new LocalizedString("ServiceName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.UM.CallRouter.Exceptions.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			Server = 3452399285U,
			ServiceName = 1422214618U
		}
	}
}
