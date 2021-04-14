using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.QuickLogStrings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Strings
	{
		public static LocalizedString FailedToGetItem(string messageClass, string folder)
		{
			return new LocalizedString("FailedToGetItem", "", false, false, Strings.ResourceManager, new object[]
			{
				messageClass,
				folder
			});
		}

		public static LocalizedString descMissingProperty(string propertyName, string unexpectedObject)
		{
			return new LocalizedString("descMissingProperty", "Ex28B9D6", false, true, Strings.ResourceManager, new object[]
			{
				propertyName,
				unexpectedObject
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.QuickLog.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			FailedToGetItem,
			descMissingProperty
		}
	}
}
