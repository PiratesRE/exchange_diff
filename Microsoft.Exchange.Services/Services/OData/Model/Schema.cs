using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class Schema
	{
		public virtual ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ReadOnlyCollection<PropertyDefinition> MandatoryCreationProperties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual EdmEntityType EdmEntityType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool TryGetPropertyByName(string name, out PropertyDefinition propertyDefinition)
		{
			ArgumentValidator.ThrowIfNull("name", name);
			propertyDefinition = this.AllProperties.FirstOrDefault((PropertyDefinition x) => string.Equals(x.Name, name, StringComparison.Ordinal));
			return propertyDefinition != null;
		}

		public virtual void RegisterEdmModel(EdmModel model)
		{
			ArgumentValidator.ThrowIfNull("model", model);
			foreach (PropertyDefinition propertyDefinition in this.DeclaredProperties)
			{
				propertyDefinition.Validate(base.GetType());
				if (propertyDefinition.IsNavigation)
				{
					EdmMultiplicity targetMultiplicity = 1;
					if (propertyDefinition.Type.GetInterface(typeof(IEnumerable).Name) != null)
					{
						targetMultiplicity = 3;
					}
					EdmNavigationPropertyInfo edmNavigationPropertyInfo = new EdmNavigationPropertyInfo
					{
						Name = propertyDefinition.Name,
						Target = propertyDefinition.NavigationTargetEntity,
						TargetMultiplicity = targetMultiplicity,
						ContainsTarget = true
					};
					this.EdmEntityType.AddUnidirectionalNavigation(edmNavigationPropertyInfo);
				}
				else
				{
					EdmStructuralProperty edmStructuralProperty = new EdmStructuralProperty(this.EdmEntityType, propertyDefinition.Name, propertyDefinition.EdmType);
					this.EdmEntityType.AddProperty(edmStructuralProperty);
					if (propertyDefinition.Equals(EntitySchema.Id))
					{
						this.EdmEntityType.AddKeys(new IEdmStructuralProperty[]
						{
							edmStructuralProperty
						});
					}
				}
			}
			model.AddElement(this.EdmEntityType);
		}
	}
}
