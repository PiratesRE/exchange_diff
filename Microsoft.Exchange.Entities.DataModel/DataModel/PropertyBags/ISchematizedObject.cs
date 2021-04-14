using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public interface ISchematizedObject<out TSchema> : IPropertyChangeTracker<PropertyDefinition>
	{
		TSchema Schema { get; }
	}
}
