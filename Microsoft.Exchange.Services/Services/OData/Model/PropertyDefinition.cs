using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class PropertyDefinition : IEquatable<PropertyDefinition>
	{
		public PropertyDefinition(string name, Type type)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("name", name);
			ArgumentValidator.ThrowIfNull("type", type);
			this.Name = name;
			this.Type = type;
		}

		public string Name { get; private set; }

		public Type Type { get; private set; }

		public IEdmTypeReference EdmType { get; set; }

		public PropertyDefinitionFlags Flags { get; set; }

		public object DefaultValue { get; set; }

		public PropertyProvider EwsPropertyProvider { get; set; }

		public PropertyProvider ADDriverPropertyProvider { get; set; }

		public PropertyProvider DataEntityPropertyProvider { get; set; }

		public IODataPropertyValueConverter ODataPropertyValueConverter { get; set; }

		public IEdmEntityType NavigationTargetEntity { get; set; }

		public bool IsNavigation
		{
			get
			{
				return this.Flags.HasFlag(PropertyDefinitionFlags.Navigation);
			}
		}

		public bool Equals(PropertyDefinition other)
		{
			return other != null && string.Equals(this.Name, other.Name);
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public void Validate(Type entitySchemaType)
		{
			ArgumentValidator.ThrowIfNull("entitySchemaType", entitySchemaType);
			if (this.IsNavigation)
			{
				if (this.NavigationTargetEntity == null)
				{
					throw new ArgumentException(string.Format("Navigation property definition {0}.{1} requires NavigationTargetEntity.", entitySchemaType.FullName, this.Name));
				}
			}
			else
			{
				if (this.EdmType == null)
				{
					throw new ArgumentException(string.Format("Non-navigation property definition {0}.{1} requires EdmType.", entitySchemaType.FullName, this.Name));
				}
				if (this.ODataPropertyValueConverter == null && (this.EdmType is EdmCollectionTypeReference || this.EdmType is EdmComplexTypeReference))
				{
					throw new ArgumentException(string.Format("Non-primitive property definition {0}.{1} requires ODataPropertyValueConverter.", entitySchemaType.FullName, this.Name));
				}
				if (this.ADDriverPropertyProvider == null && this.DataEntityPropertyProvider == null && this.EwsPropertyProvider == null)
				{
					throw new ArgumentException(string.Format("At least one of the property providers should be assigned to property definition {0}.{1}.", entitySchemaType.FullName, this.Name));
				}
			}
		}
	}
}
