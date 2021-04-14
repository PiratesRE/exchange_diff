using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class FacadeBase : ConfigurablePropertyBag
	{
		protected FacadeBase(IConfigurable innerObj)
		{
			this.innerObj = innerObj;
		}

		public IConfigurable InnerConfigurable
		{
			get
			{
				return this.innerObj;
			}
		}

		public IPropertyBag InnerPropertyBag
		{
			get
			{
				return (IPropertyBag)this.InnerConfigurable;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return this.innerObj.Identity;
			}
		}

		public override object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.InnerPropertyBag[propertyDefinition];
			}
			set
			{
				ConfigurableObject configurableObject = this.InnerPropertyBag as ConfigurableObject;
				if (configurableObject != null)
				{
					DalHelper.SetConfigurableObject(value, propertyDefinition, configurableObject);
					return;
				}
				this.InnerPropertyBag[propertyDefinition] = value;
			}
		}

		public virtual void FixIdPropertiesForWebservice(IConfigDataProvider dataProvider, ADObjectId orgId, bool isServer)
		{
		}

		internal static T NewADObject<T>() where T : ADObject, new()
		{
			T result = Activator.CreateInstance<T>();
			result.ResetChangeTracking(true);
			return result;
		}

		private readonly IConfigurable innerObj;
	}
}
