using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Entities.DataModel.Serialization
{
	internal static class Strings
	{
		public static LocalizedString ValueIsOutOfRange(string name, object value)
		{
			return new LocalizedString("ValueIsOutOfRange", Strings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString ErrorNonEntityType(string type, string baseType)
		{
			return new LocalizedString("ErrorNonEntityType", Strings.ResourceManager, new object[]
			{
				type,
				baseType
			});
		}

		public static LocalizedString ErrorNoDefaultConstructor(string type)
		{
			return new LocalizedString("ErrorNoDefaultConstructor", Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ErrorUnknownType(string typeName)
		{
			return new LocalizedString("ErrorUnknownType", Strings.ResourceManager, new object[]
			{
				typeName
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Entities.DataModel.Serialization.Strings", typeof(Strings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			ValueIsOutOfRange,
			ErrorNonEntityType,
			ErrorNoDefaultConstructor,
			ErrorUnknownType
		}
	}
}
