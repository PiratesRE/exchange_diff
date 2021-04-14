using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors.StorageAccessors
{
	internal static class StoreObjectAccessors
	{
		public static readonly IStoragePropertyAccessor<IStoreObject, VersionedId> IdAccessor = new DelegatedStoragePropertyAccessor<IStoreObject, VersionedId>(delegate(IStoreObject container, out VersionedId value)
		{
			value = container.Id;
			return value != null;
		}, null, null, null, new PropertyDefinition[]
		{
			ItemSchema.Id
		});

		public static readonly IStoragePropertyAccessor<IStoreObject, string> ItemClassAccessor = new DefaultStoragePropertyAccessor<IStoreObject, string>(StoreObjectSchema.ItemClass, false);

		public static readonly IStoragePropertyAccessor<ICalendarFolder, byte[]> RecordKey = new DefaultStoragePropertyAccessor<IStoreObject, byte[]>(StoreObjectSchema.RecordKey, false);
	}
}
