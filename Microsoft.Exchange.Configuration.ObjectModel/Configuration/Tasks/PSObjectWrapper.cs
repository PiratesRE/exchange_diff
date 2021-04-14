using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PSObjectWrapper : IPropertyBag, IReadOnlyPropertyBag
	{
		public PSObjectWrapper(PSObject mshObject)
		{
			this.mshObject = mshObject;
			this.innerPropBag = (mshObject.BaseObject as IPropertyBag);
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				object result = null;
				Type type = propertyDefinition.Type;
				if (((ProviderPropertyDefinition)propertyDefinition).IsMultivalued)
				{
					type = typeof(MultiValuedProperty<>).MakeGenericType(new Type[]
					{
						type
					});
				}
				if (this.innerPropBag != null)
				{
					LanguagePrimitives.TryConvertTo(this.innerPropBag[propertyDefinition], type, out result);
				}
				else if (this.mshObject.Properties[propertyDefinition.Name] != null)
				{
					LanguagePrimitives.TryConvertTo(this.mshObject.Properties[propertyDefinition.Name].Value, type, out result);
				}
				return result;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			throw new NotImplementedException();
		}

		public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
		{
			throw new NotImplementedException();
		}

		private PSObject mshObject;

		private IPropertyBag innerPropBag;
	}
}
