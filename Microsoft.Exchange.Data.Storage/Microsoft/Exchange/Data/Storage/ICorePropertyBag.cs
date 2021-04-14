using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICorePropertyBag : ILocationIdentifierSetter
	{
		object TryGetProperty(PropertyDefinition propertyDefinition);

		void SetProperty(PropertyDefinition propertyDefinition, object value);

		void Delete(PropertyDefinition propertyDefinition);

		Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode);

		ICollection<PropertyDefinition> AllFoundProperties { get; }

		object this[PropertyDefinition propertyDefinition]
		{
			get;
			set;
		}

		T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition);

		T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue);

		T? GetValueAsNullable<T>(StorePropertyDefinition propertyDefinition) where T : struct;

		bool IsPropertyDirty(PropertyDefinition propertyDefinition);

		bool IsDirty { get; }

		void Load(ICollection<PropertyDefinition> propsToLoad);

		void Reload();

		void Clear();
	}
}
