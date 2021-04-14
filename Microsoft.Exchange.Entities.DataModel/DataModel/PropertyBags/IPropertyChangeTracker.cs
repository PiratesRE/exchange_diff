using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public interface IPropertyChangeTracker<in TPropertyDefinition>
	{
		bool IsPropertySet(TPropertyDefinition property);
	}
}
