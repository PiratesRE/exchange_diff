using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Entities.EntitySets
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(52452988U, "UnsupportedEstimatedTotalCount");
			Strings.stringIDs.Add(1964246015U, "StringCompareMustCompareToZero");
		}

		public static LocalizedString UnsupportedEstimatedTotalCount
		{
			get
			{
				return new LocalizedString("UnsupportedEstimatedTotalCount", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StringCompareMustCompareToZero
		{
			get
			{
				return new LocalizedString("StringCompareMustCompareToZero", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedPropertyValue(Expression expression)
		{
			return new LocalizedString("UnsupportedPropertyValue", Strings.ResourceManager, new object[]
			{
				expression
			});
		}

		public static LocalizedString UnsupportedOperatorWithNull(ExpressionType operatorType)
		{
			return new LocalizedString("UnsupportedOperatorWithNull", Strings.ResourceManager, new object[]
			{
				operatorType
			});
		}

		public static LocalizedString UnsupportedOperator(ExpressionType operatorType)
		{
			return new LocalizedString("UnsupportedOperator", Strings.ResourceManager, new object[]
			{
				operatorType
			});
		}

		public static LocalizedString UnsupportedOperatorWithBlob(ExpressionType operatorType)
		{
			return new LocalizedString("UnsupportedOperatorWithBlob", Strings.ResourceManager, new object[]
			{
				operatorType
			});
		}

		public static LocalizedString UnsupportedMethodCall(MethodInfo method)
		{
			return new LocalizedString("UnsupportedMethodCall", Strings.ResourceManager, new object[]
			{
				method
			});
		}

		public static LocalizedString UnsupportedFilterExpression(Expression expression)
		{
			return new LocalizedString("UnsupportedFilterExpression", Strings.ResourceManager, new object[]
			{
				expression
			});
		}

		public static LocalizedString UnsupportedPropertyExpression(Expression expression)
		{
			return new LocalizedString("UnsupportedPropertyExpression", Strings.ResourceManager, new object[]
			{
				expression
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Entities.EntitySets.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			UnsupportedEstimatedTotalCount = 52452988U,
			StringCompareMustCompareToZero = 1964246015U
		}

		private enum ParamIDs
		{
			UnsupportedPropertyValue,
			UnsupportedOperatorWithNull,
			UnsupportedOperator,
			UnsupportedOperatorWithBlob,
			UnsupportedMethodCall,
			UnsupportedFilterExpression,
			UnsupportedPropertyExpression
		}
	}
}
