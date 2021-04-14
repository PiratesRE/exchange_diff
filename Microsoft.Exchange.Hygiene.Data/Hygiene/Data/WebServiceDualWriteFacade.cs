using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class WebServiceDualWriteFacade<T> : TenantSettingFacade<T> where T : ADObject, new()
	{
		public WebServiceDualWriteFacade(T adObject)
		{
			T t = adObject;
			if (adObject == null)
			{
				t = FacadeBase.NewADObject<T>();
			}
			base..ctor(t);
		}

		public WebServiceDualWriteFacade() : this(default(T))
		{
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			return base.GetPropertyDefinitions(isChangedOnly).Concat(new ADPropertyDefinition[]
			{
				ADObjectSchema.Id,
				ADObjectSchema.OrganizationalUnitRoot,
				ADObjectSchema.Name,
				WebServiceDualWriteFacade<T>.whenChangedUtc
			});
		}

		private static readonly HygienePropertyDefinition whenChangedUtc = new HygienePropertyDefinition("WhenChangedUTC", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
