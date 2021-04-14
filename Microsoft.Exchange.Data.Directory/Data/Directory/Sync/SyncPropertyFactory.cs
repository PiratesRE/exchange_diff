using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal static class SyncPropertyFactory
	{
		internal static object Create(Type type, object value, bool multiValued)
		{
			return SyncPropertyFactory.GetFactoryInstance(type).Create(value, multiValued);
		}

		internal static object CreateDefault(Type type, bool multiValued)
		{
			return SyncPropertyFactory.GetFactoryInstance(type).GetDefault(multiValued);
		}

		private static IGenericSyncPropertyFactory GetFactoryInstance(Type type)
		{
			return SyncPropertyFactory.factoryInstances.AddIfNotExists(type, delegate(Type param0)
			{
				Type type2 = typeof(GenericSyncPropertyFactory<>).MakeGenericType(new Type[]
				{
					type
				});
				return (IGenericSyncPropertyFactory)Activator.CreateInstance(type2, true);
			});
		}

		private static SynchronizedDictionary<Type, IGenericSyncPropertyFactory> factoryInstances = new SynchronizedDictionary<Type, IGenericSyncPropertyFactory>();
	}
}
