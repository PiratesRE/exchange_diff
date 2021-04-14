using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal class PropertyChangeMetadataConverter : IConverter<PropertyChangeMetadata, IList<string>>
	{
		public PropertyChangeMetadataConverter(IConverter<PropertyChangeMetadata.PropertyGroup, IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>> propertyMapping)
		{
			this.propertyMapping = propertyMapping;
		}

		public IList<string> Convert(PropertyChangeMetadata value)
		{
			if (value == null)
			{
				throw new ExArgumentNullException("value");
			}
			IEnumerable<string> source = from overriddenGroup in this.GetOverriddenGroups(value)
			select this.propertyMapping.Convert(overriddenGroup) into properties
			where properties != null
			from property in properties
			select property.Name;
			return source.ToList<string>();
		}

		protected virtual IEnumerable<PropertyChangeMetadata.PropertyGroup> GetOverriddenGroups(PropertyChangeMetadata value)
		{
			return value.GetOverriddenGroups();
		}

		private readonly IConverter<PropertyChangeMetadata.PropertyGroup, IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>> propertyMapping;
	}
}
