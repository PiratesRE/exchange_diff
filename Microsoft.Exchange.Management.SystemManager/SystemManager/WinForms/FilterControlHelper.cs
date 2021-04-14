using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class FilterControlHelper
	{
		static FilterControlHelper()
		{
			FilterControlHelper.operators.Add(typeof(string), new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.Contains,
				PropertyFilterOperator.NotContains,
				PropertyFilterOperator.StartsWith,
				PropertyFilterOperator.EndsWith
			});
			FilterControlHelper.operators.Add(typeof(Enum), new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual
			});
			Dictionary<Type, PropertyFilterOperator[]> dictionary = FilterControlHelper.operators;
			Type typeFromHandle = typeof(bool);
			PropertyFilterOperator[] value = new PropertyFilterOperator[1];
			dictionary.Add(typeFromHandle, value);
			FilterControlHelper.operators.Add(typeof(int), new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.GreaterThan,
				PropertyFilterOperator.GreaterThanOrEqual,
				PropertyFilterOperator.LessThan,
				PropertyFilterOperator.LessThanOrEqual
			});
			FilterControlHelper.operators.Add(typeof(ulong), new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.GreaterThan,
				PropertyFilterOperator.GreaterThanOrEqual,
				PropertyFilterOperator.LessThan,
				PropertyFilterOperator.LessThanOrEqual
			});
			FilterControlHelper.operators.Add(typeof(long), new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.GreaterThan,
				PropertyFilterOperator.GreaterThanOrEqual,
				PropertyFilterOperator.LessThan,
				PropertyFilterOperator.LessThanOrEqual
			});
			FilterControlHelper.operators.Add(typeof(ADObjectId), new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual
			});
		}

		public static PropertyFilterOperator[] GetFilterOperators(Type type)
		{
			Type key = typeof(Enum).IsAssignableFrom(type) ? typeof(Enum) : type;
			if (!FilterControlHelper.operators.ContainsKey(key))
			{
				return new PropertyFilterOperator[0];
			}
			return FilterControlHelper.operators[key];
		}

		public static ProviderPropertyDefinition GenerateEmptyPropertyDefinition(string propertyName, Type type)
		{
			return new SimpleProviderPropertyDefinition(propertyName, ExchangeObjectVersion.Exchange2003, FilterControlHelper.GetEffectiveType(type), FilterControlHelper.GetDefaultPropertyDefinitionFlagsForType(type), FilterControlHelper.GetDefaultValueForType(type), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		public static ProviderPropertyDefinition CopyPropertyDefinition(string propertyName, ProviderPropertyDefinition definition)
		{
			return new SimpleProviderPropertyDefinition(propertyName, definition.VersionAdded, definition.Type, (definition as SimpleProviderPropertyDefinition).Flags, definition.DefaultValue, PropertyDefinitionConstraint.None, definition.AllConstraints.ToArray<PropertyDefinitionConstraint>(), definition.SupportingProperties.ToArray<ProviderPropertyDefinition>(), definition.CustomFilterBuilderDelegate, definition.GetterDelegate, definition.SetterDelegate);
		}

		public static Type GetEffectiveType(Type type)
		{
			if (null == type)
			{
				throw new ArgumentNullException("type");
			}
			if (typeof(ICollection).IsAssignableFrom(type) && !type.ContainsGenericParameters)
			{
				Type[] genericArguments = type.GetGenericArguments();
				if (genericArguments.Length == 1)
				{
					return genericArguments[0];
				}
			}
			return type;
		}

		public static object GetDefaultValueForType(Type type)
		{
			object result = type.IsValueType ? Activator.CreateInstance(type) : null;
			if (!(type == typeof(string)))
			{
				return result;
			}
			return string.Empty;
		}

		public static PropertyDefinitionFlags GetDefaultPropertyDefinitionFlagsForType(Type type)
		{
			PropertyDefinitionFlags result = PropertyDefinitionFlags.None;
			if (typeof(ICollection).IsAssignableFrom(type))
			{
				result = PropertyDefinitionFlags.MultiValued;
			}
			else if (type == typeof(int) || type == typeof(long) || type == typeof(ulong))
			{
				result = PropertyDefinitionFlags.PersistDefaultValue;
			}
			return result;
		}

		public static int GetIntFromUnlimitedInt(object unlimitedValue)
		{
			Unlimited<int> unlimited = (Unlimited<int>)unlimitedValue;
			if (!unlimited.IsUnlimited)
			{
				return unlimited.Value;
			}
			return int.MaxValue;
		}

		public static long GetDaysFromEnhancedTimeSpan(object timeSpanObj)
		{
			long result = long.MinValue;
			if (!timeSpanObj.IsNullValue())
			{
				if (timeSpanObj.GetType() == typeof(EnhancedTimeSpan))
				{
					result = (long)((EnhancedTimeSpan)timeSpanObj).TotalDays;
				}
				else
				{
					Unlimited<EnhancedTimeSpan> unlimited = (Unlimited<EnhancedTimeSpan>)timeSpanObj;
					result = (unlimited.IsUnlimited ? long.MaxValue : ((long)unlimited.Value.TotalDays));
				}
			}
			return result;
		}

		public static ulong GetMBFromByteQuantifiedSize(object sizeObj)
		{
			ulong result = 0UL;
			if (!sizeObj.IsNullValue())
			{
				if (sizeObj.GetType() == typeof(ByteQuantifiedSize))
				{
					result = ((ByteQuantifiedSize)sizeObj).ToMB();
				}
				else
				{
					Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)sizeObj;
					result = (unlimited.IsUnlimited ? ulong.MaxValue : unlimited.Value.ToMB());
				}
			}
			return result;
		}

		private static Dictionary<Type, PropertyFilterOperator[]> operators = new Dictionary<Type, PropertyFilterOperator[]>();
	}
}
