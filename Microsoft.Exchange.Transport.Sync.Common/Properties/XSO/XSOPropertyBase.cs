using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Properties.XSO
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class XSOPropertyBase<T> : IXSOProperty<T>, IProperty<Item, T>, IWriteableProperty<Item, T>, IReadableProperty<Item, T>
	{
		protected XSOPropertyBase(IXSOPropertyManager propertyManager, params PropertyDefinition[] propertyDefinitions)
		{
			SyncUtilities.ThrowIfArgumentNull("propertyManager", propertyManager);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("propertyDefinitions", propertyDefinitions);
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				propertyManager.AddPropertyDefinition(propertyDefinition);
			}
		}

		public abstract T ReadProperty(Item item);

		public abstract void WriteProperty(Item item, T value);
	}
}
