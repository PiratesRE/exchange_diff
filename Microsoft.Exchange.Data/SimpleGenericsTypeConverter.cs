using System;
using System.Collections;
using System.Management.Automation;
using System.Reflection;

namespace Microsoft.Exchange.Data
{
	public class SimpleGenericsTypeConverter : PSTypeConverter
	{
		public override bool CanConvertFrom(object sourceValue, Type destinationType)
		{
			if (!destinationType.GetTypeInfo().IsGenericType)
			{
				return false;
			}
			Type[] genericArguments = destinationType.GetTypeInfo().GetGenericArguments();
			return genericArguments.Length == 1 && (sourceValue == null || !genericArguments[0].GetTypeInfo().IsAssignableFrom(sourceValue.GetType().GetTypeInfo())) && !(sourceValue is string) && (!typeof(MultiValuedProperty<>).GetTypeInfo().IsAssignableFrom(destinationType.GetGenericTypeDefinition().GetTypeInfo()) || sourceValue is ArrayList);
		}

		public override object ConvertFrom(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
		{
			if (!destinationType.GetTypeInfo().IsGenericType)
			{
				throw new ArgumentException(DataStrings.ErrorNonGeneric(destinationType.Name), "destinationType");
			}
			Type[] genericArguments = destinationType.GetTypeInfo().GetGenericArguments();
			if (genericArguments.Length != 1)
			{
				throw new ArgumentException(DataStrings.ErrorCannotConvert, "destinationType");
			}
			TypeInfo typeInfo = genericArguments[0].GetTypeInfo();
			if (destinationType.GetGenericTypeDefinition() == typeof(Unlimited<>) && string.Equals(sourceValue.ToString(), Unlimited<int>.UnlimitedString))
			{
				if (typeInfo.IsAssignableFrom(typeof(int).GetTypeInfo()))
				{
					return Unlimited<int>.UnlimitedValue;
				}
				if (typeInfo.IsAssignableFrom(typeof(EnhancedTimeSpan).GetTypeInfo()))
				{
					return Unlimited<EnhancedTimeSpan>.UnlimitedValue;
				}
				if (typeInfo.IsAssignableFrom(typeof(ByteQuantifiedSize).GetTypeInfo()))
				{
					return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				}
				throw new ArgumentException(string.Format("The type {0} is not supported to be used with the Unlimited type.", typeInfo));
			}
			else
			{
				if (typeInfo.IsAssignableFrom(typeof(int).GetTypeInfo()))
				{
					return int.Parse(sourceValue.ToString());
				}
				if (typeof(MultiValuedProperty<>).GetTypeInfo().IsAssignableFrom(destinationType.GetGenericTypeDefinition().GetTypeInfo()))
				{
					IList list = destinationType.GetTypeInfo().GetConstructor(new Type[0]).Invoke(new object[0]) as IList;
					foreach (object valueToConvert in (sourceValue as ArrayList))
					{
						list.Add(LanguagePrimitives.ConvertTo(valueToConvert, typeInfo, formatProvider));
					}
					return list;
				}
				object valueToConvert2 = LanguagePrimitives.ConvertTo(sourceValue, typeInfo, formatProvider);
				return LanguagePrimitives.ConvertTo(valueToConvert2, destinationType, formatProvider);
			}
		}

		public override bool CanConvertTo(object sourceValue, Type destinationType)
		{
			return (destinationType == typeof(ulong) || destinationType == typeof(double)) && (sourceValue is Unlimited<ByteQuantifiedSize> || sourceValue is ByteQuantifiedSize);
		}

		public override object ConvertTo(object sourceValue, Type destinationType, IFormatProvider formatProvider, bool ignoreCase)
		{
			if (sourceValue is Unlimited<ByteQuantifiedSize>)
			{
				if (destinationType == typeof(ulong))
				{
					return ((T)((Unlimited<ByteQuantifiedSize>)sourceValue)).ToBytes();
				}
				if (destinationType == typeof(double))
				{
					return ((T)((Unlimited<ByteQuantifiedSize>)sourceValue)).ToBytes();
				}
			}
			else if (sourceValue is ByteQuantifiedSize)
			{
				if (destinationType == typeof(ulong))
				{
					return ((ByteQuantifiedSize)sourceValue).ToBytes();
				}
				if (destinationType == typeof(double))
				{
					return ((ByteQuantifiedSize)sourceValue).ToBytes();
				}
			}
			throw new ArgumentException(DataStrings.ErrorCannotConvert, "destinationType");
		}
	}
}
