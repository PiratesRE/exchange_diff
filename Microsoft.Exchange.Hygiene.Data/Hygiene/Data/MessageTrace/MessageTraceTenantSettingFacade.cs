using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageTraceTenantSettingFacade<T> : FacadeBase where T : ADObject, new()
	{
		public MessageTraceTenantSettingFacade(IConfigurable configurable) : base(configurable ?? FacadeBase.NewADObject<T>())
		{
		}

		public MessageTraceTenantSettingFacade() : this(null)
		{
		}

		public PropertyDefinition[] DeclaredProperties
		{
			get
			{
				if (this.declaredProperties == null)
				{
					IEnumerable<PropertyDefinition> declaredReflectedProperties = DalHelper.GetDeclaredReflectedProperties((ADObject)base.InnerConfigurable);
					this.declaredProperties = declaredReflectedProperties.ToArray<PropertyDefinition>();
				}
				return this.declaredProperties;
			}
			set
			{
				this.declaredProperties = value;
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			T innerSetting = base.InnerConfigurable as T;
			IEnumerable<PropertyDefinition> enumerable = this.DeclaredProperties;
			if (isChangedOnly)
			{
				enumerable = from property in enumerable.OfType<ProviderPropertyDefinition>()
				where !property.IsCalculated && innerSetting.IsModified(property)
				select property;
			}
			return enumerable.Concat(MessageTraceTenantSettingFacade<T>.stdProperties);
		}

		public static readonly HygienePropertyDefinition WhenChangedAtSourceProp = new HygienePropertyDefinition("WhenChangedAtSource", typeof(DateTime?));

		private static readonly PropertyDefinition[] stdProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADObjectSchema.OrganizationalUnitRoot,
			ADObjectSchema.RawName,
			ADObjectSchema.WhenChangedRaw,
			ADObjectSchema.WhenCreatedRaw,
			ADObjectSchema.ObjectState,
			ADObjectSchema.WhenChangedUTC,
			MessageTraceTenantSettingFacade<T>.WhenChangedAtSourceProp
		};

		private PropertyDefinition[] declaredProperties;
	}
}
