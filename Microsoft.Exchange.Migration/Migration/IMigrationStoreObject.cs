using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationStoreObject : IDisposable, IPropertyBag, IReadOnlyPropertyBag
	{
		StoreObjectId Id { get; }

		ExDateTime CreationTime { get; }

		string Name { get; }

		void Delete(PropertyDefinition propertyDefinition);

		T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue);

		void Load(ICollection<PropertyDefinition> properties);

		void OpenAsReadWrite();

		void LoadMessageIdProperties();

		void Save(SaveMode saveMode);

		XElement GetDiagnosticInfo(ICollection<PropertyDefinition> properties, MigrationDiagnosticArgument argument);
	}
}
