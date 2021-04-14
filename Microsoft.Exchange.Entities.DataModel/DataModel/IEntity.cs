using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel
{
	public interface IEntity : IPropertyChangeTracker<PropertyDefinition>
	{
		string Id { get; set; }
	}
}
