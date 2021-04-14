using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SharingItemManagerBase<TDataObject>
	{
		protected static T TryGetPropertyRef<T>(object[] properties, int index) where T : class
		{
			return properties[index] as T;
		}

		protected static T? TryGetPropertyVal<T>(object[] properties, int index) where T : struct
		{
			object obj = properties[index];
			if (obj != null && typeof(T).GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo()))
			{
				return (T?)obj;
			}
			return null;
		}

		protected abstract void StampItemFromDataObject(Item item, TDataObject data);

		protected abstract TDataObject CreateDataObjectFromItem(object[] properties);
	}
}
