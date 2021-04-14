using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IStorePropertyBag : IPropertyBag, IReadOnlyPropertyBag
	{
		bool IsDirty { get; }

		bool IsPropertyDirty(PropertyDefinition propertyDefinition);

		void Load();

		void Load(ICollection<PropertyDefinition> propertyDefinitions);

		Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode);

		object TryGetProperty(PropertyDefinition propertyDefinition);

		void Delete(PropertyDefinition propertyDefinition);

		T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue);

		void SetOrDeleteProperty(PropertyDefinition propertyDefinition, object propertyValue);
	}
}
