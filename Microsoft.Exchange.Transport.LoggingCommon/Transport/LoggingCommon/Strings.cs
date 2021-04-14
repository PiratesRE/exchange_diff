using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(581603311U, "MissingLogSchemaInfo");
		}

		public static LocalizedString MissingLogSchemaInfo
		{
			get
			{
				return new LocalizedString("MissingLogSchemaInfo", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Transport.LoggingCommon.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			MissingLogSchemaInfo = 581603311U
		}
	}
}
