using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Properties.XSO
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class XSOProperty<T> : XSOPropertyBase<T>
	{
		public XSOProperty(IXSOPropertyManager propertyManager, PropertyDefinition propertyDefinition) : base(propertyManager, new PropertyDefinition[]
		{
			propertyDefinition
		})
		{
			this.propertyDefinition = propertyDefinition;
		}

		public override T ReadProperty(Item item)
		{
			SyncUtilities.ThrowIfArgumentNull("item", item);
			return SyncUtilities.SafeGetProperty<T>(item, this.propertyDefinition);
		}

		public override void WriteProperty(Item item, T value)
		{
			SyncUtilities.ThrowIfArgumentNull("item", item);
			if (!object.Equals(default(T), value))
			{
				item[this.propertyDefinition] = value;
			}
		}

		private PropertyDefinition propertyDefinition;
	}
}
