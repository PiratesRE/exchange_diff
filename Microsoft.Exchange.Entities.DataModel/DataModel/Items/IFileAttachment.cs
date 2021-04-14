using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public interface IFileAttachment : IAttachment, IEntity, IPropertyChangeTracker<PropertyDefinition>
	{
		byte[] Content { get; set; }

		string ContentId { get; set; }

		string ContentLocation { get; set; }
	}
}
