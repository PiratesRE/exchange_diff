using System;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Messaging
{
	public abstract class EmailMessage<TSchema> : Item<TSchema>, IEmailMessage, IItem, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned where TSchema : EmailMessageSchema, new()
	{
		public bool IsRead
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<bool>(schema.IsReadProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<bool>(schema.IsReadProperty, value);
			}
		}
	}
}
