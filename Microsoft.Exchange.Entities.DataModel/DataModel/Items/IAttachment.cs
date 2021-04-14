using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public interface IAttachment : IEntity, IPropertyChangeTracker<PropertyDefinition>
	{
		string ContentType { get; set; }

		bool IsInline { get; set; }

		ExDateTime LastModifiedTime { get; set; }

		string Name { get; set; }

		long Size { get; set; }
	}
}
