using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public interface IReferenceAttachment : IAttachment, IEntity, IPropertyChangeTracker<PropertyDefinition>
	{
		string PathName { get; set; }

		string ProviderEndpointUrl { get; set; }

		string ProviderType { get; set; }
	}
}
