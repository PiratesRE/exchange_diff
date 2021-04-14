using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class WebServiceTransportRuleFacade : TenantSettingFacade<TransportRule>
	{
		public WebServiceTransportRuleFacade(TransportRule transportRule) : base(transportRule ?? FacadeBase.NewADObject<TransportRule>())
		{
		}

		public WebServiceTransportRuleFacade() : this(null)
		{
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			return base.GetPropertyDefinitions(isChangedOnly).Concat(new ADPropertyDefinition[]
			{
				ADObjectSchema.Id,
				ADObjectSchema.OrganizationalUnitRoot,
				ADObjectSchema.Name,
				WebServiceTransportRuleFacade.whenChangedUtc
			});
		}

		private static readonly HygienePropertyDefinition whenChangedUtc = new HygienePropertyDefinition("WhenChangedUTC", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
