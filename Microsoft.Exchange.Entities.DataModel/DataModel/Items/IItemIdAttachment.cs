using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public interface IItemIdAttachment : IAttachment, IEntity, IPropertyChangeTracker<PropertyDefinition>
	{
		string ItemToAttachId { get; set; }
	}
}
