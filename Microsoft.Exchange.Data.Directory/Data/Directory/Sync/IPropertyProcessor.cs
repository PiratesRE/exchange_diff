using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal interface IPropertyProcessor
	{
		void Process<T>(SyncPropertyDefinition propertyDefinition, ref T values) where T : DirectoryProperty, new();
	}
}
