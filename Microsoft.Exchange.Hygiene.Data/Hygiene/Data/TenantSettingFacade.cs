using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class TenantSettingFacade<T> : FacadeBase where T : ADObject, new()
	{
		public TenantSettingFacade(IConfigurable configurable) : base(configurable ?? FacadeBase.NewADObject<T>())
		{
		}

		public TenantSettingFacade() : this(null)
		{
		}

		public PropertyDefinition[] DeclaredProperties
		{
			get
			{
				if (this.declaredProperties == null)
				{
					this.declaredProperties = (from property in (base.InnerConfigurable as ADObject).ObjectSchema.AllProperties.OfType<ProviderPropertyDefinition>()
					where !property.IsCalculated
					select property).ToArray<ProviderPropertyDefinition>();
				}
				return this.declaredProperties;
			}
			set
			{
				this.declaredProperties = value;
			}
		}

		public override object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return base[propertyDefinition];
			}
			set
			{
				base[propertyDefinition] = value;
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			T innerSetting = base.InnerConfigurable as T;
			IEnumerable<PropertyDefinition> enumerable = this.DeclaredProperties;
			if (isChangedOnly)
			{
				enumerable = from property in enumerable.OfType<ProviderPropertyDefinition>()
				where innerSetting.IsModified(property)
				select property;
			}
			return enumerable.Concat(new ADPropertyDefinition[]
			{
				ADObjectSchema.OrganizationalUnitRoot,
				ADObjectSchema.Id,
				ADObjectSchema.RawName,
				DalHelper.IsTracerTokenProp
			});
		}

		public override void FixIdPropertiesForWebservice(IConfigDataProvider dataProvider, ADObjectId orgId, bool isServer)
		{
			if (typeof(T) == typeof(TenantInboundConnector))
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)this[TenantInboundConnectorSchema.AssociatedAcceptedDomains];
				if (multiValuedProperty != null && multiValuedProperty.Count != 0)
				{
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, orgId.ObjectGuid);
					IEnumerable<AcceptedDomain> source = dataProvider.Find<AcceptedDomain>(filter, null, false, null).OfType<AcceptedDomain>();
					for (int i = 0; i < multiValuedProperty.Count; i++)
					{
						ADObjectId domainId = multiValuedProperty[i];
						AcceptedDomain acceptedDomain2 = source.FirstOrDefault((AcceptedDomain acceptedDomain) => acceptedDomain.Name == domainId.Name);
						if (acceptedDomain2 != null)
						{
							domainId = new ADObjectId(domainId.DistinguishedName, acceptedDomain2.Id.ObjectGuid);
							multiValuedProperty[i] = domainId;
						}
					}
				}
			}
			base.FixIdPropertiesForWebservice(dataProvider, orgId, isServer);
		}

		private PropertyDefinition[] declaredProperties;
	}
}
