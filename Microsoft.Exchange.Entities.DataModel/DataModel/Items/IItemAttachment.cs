using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public interface IItemAttachment : IAttachment, IEntity, IPropertyChangeTracker<PropertyDefinition>
	{
		IItem Item { get; set; }
	}
}
