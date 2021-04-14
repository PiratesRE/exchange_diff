using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel
{
	public interface IStorageEntity : IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
	}
}
