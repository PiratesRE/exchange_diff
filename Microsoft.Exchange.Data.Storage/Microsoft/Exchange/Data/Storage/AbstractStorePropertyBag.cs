using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractStorePropertyBag : IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag
	{
		public virtual object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsDirty
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
		{
			throw new NotImplementedException();
		}

		public virtual object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsPropertyDirty(PropertyDefinition propertyDefinition)
		{
			throw new NotImplementedException();
		}

		public virtual void Load()
		{
			throw new NotImplementedException();
		}

		public virtual void Load(ICollection<PropertyDefinition> propertyDefinitions)
		{
			throw new NotImplementedException();
		}

		public virtual Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			throw new NotImplementedException();
		}

		public virtual object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			throw new NotImplementedException();
		}

		public virtual void Delete(PropertyDefinition propertyDefinition)
		{
			throw new NotImplementedException();
		}

		public virtual T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			throw new NotImplementedException();
		}

		public virtual void SetOrDeleteProperty(PropertyDefinition propertyDefinition, object propertyValue)
		{
			throw new NotImplementedException();
		}
	}
}
