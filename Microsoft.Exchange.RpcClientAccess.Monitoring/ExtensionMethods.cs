using System;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal static class ExtensionMethods
	{
		public static bool TryGet<TValue>(this IPropertyBag propertyBag, ContextProperty property, out TValue value)
		{
			if (!typeof(TValue).IsAssignableFrom(property.Type))
			{
				throw new InvalidOperationException(string.Format("Property {0} of type {1} cannot be retrieved as type {2}", property, property.Type, typeof(TValue)));
			}
			object obj;
			if (propertyBag.TryGet(property, out obj))
			{
				value = (TValue)((object)obj);
				return true;
			}
			value = default(TValue);
			return false;
		}

		public static bool TryGet<TValue>(this IPropertyBag propertyBag, ContextProperty<TValue> property, out TValue value)
		{
			return propertyBag.TryGet(property, out value);
		}

		public static TValue GetOrDefault<TValue>(this IPropertyBag propertyBag, ContextProperty<TValue> property) where TValue : class
		{
			TValue result;
			if (!propertyBag.TryGet(property, out result))
			{
				return default(TValue);
			}
			return result;
		}

		public static TValue? GetNullableOrDefault<TValue>(this IPropertyBag propertyBag, ContextProperty<TValue> property) where TValue : struct
		{
			TValue value;
			if (!propertyBag.TryGet(property, out value))
			{
				return null;
			}
			return new TValue?(value);
		}

		public static TValue Get<TValue>(this IPropertyBag propertyBag, ContextProperty<TValue> property)
		{
			TValue result;
			if (propertyBag.TryGet(property, out result))
			{
				return result;
			}
			throw new PropertyNotFoundException(property.ToString());
		}

		public static bool IsPropertyFound(this IPropertyBag propertyBag, ContextProperty property)
		{
			object obj;
			return propertyBag.TryGet(property, out obj);
		}

		public static T[] Concat<T>(this T[] x, params T[] y)
		{
			T[] array = new T[x.Length + y.Length];
			Array.Copy(x, 0, array, 0, x.Length);
			Array.Copy(y, 0, array, x.Length, y.Length);
			return array;
		}
	}
}
