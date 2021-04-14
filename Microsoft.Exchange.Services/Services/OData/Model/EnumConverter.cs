using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class EnumConverter
	{
		public static TResult CastEnumType<TResult>(object input)
		{
			if (input == null)
			{
				return default(TResult);
			}
			return (TResult)((object)EnumConverter.CastEnumType(typeof(TResult), input));
		}

		public static TResult FromODataEnumValue<TResult>(ODataEnumValue odataValue)
		{
			if (odataValue == null)
			{
				return default(TResult);
			}
			return (TResult)((object)EnumConverter.FromODataEnumValue(typeof(TResult), odataValue));
		}

		public static object FromODataEnumValue(Type targetType, ODataEnumValue odataValue)
		{
			ArgumentValidator.ThrowIfNull("targetType", targetType);
			if (odataValue == null)
			{
				return EnumConverter.CastEnumType(targetType, null);
			}
			return EnumConverter.CastEnumType(targetType, odataValue.Value);
		}

		public static ODataEnumValue ToODataEnumValue(Enum enumValue)
		{
			return new ODataEnumValue(enumValue.ToString());
		}

		private static object CastEnumType(Type targetType, object input)
		{
			if (targetType.Equals(input.GetType()))
			{
				return input;
			}
			string value = input.ToString();
			return Enum.Parse(targetType, value, true);
		}
	}
}
