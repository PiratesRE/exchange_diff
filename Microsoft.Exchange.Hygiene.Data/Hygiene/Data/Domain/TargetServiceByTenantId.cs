using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	[Serializable]
	internal class TargetServiceByTenantId : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return DomainSchema.GetObjectId(this.TenantId);
			}
		}

		public Guid TenantId
		{
			get
			{
				return DomainSchema.GetGuidEmptyIfNull(this[DomainSchema.TenantId]);
			}
			set
			{
				this[DomainSchema.TenantId] = DomainSchema.GetNullIfGuidEmpty(value);
			}
		}

		public Dictionary<int, Dictionary<int, string>> Properties
		{
			get
			{
				return this[DomainSchema.PropertiesAsId] as Dictionary<int, Dictionary<int, string>>;
			}
			set
			{
				this[DomainSchema.PropertiesAsId] = value;
			}
		}

		public IEnumerable<string> UpdatedDomainKeys
		{
			get
			{
				return this[DomainSchema.UpdatedDomains] as IEnumerable<string>;
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return TargetServiceByTenantId.propertydefinitions;
		}

		public override string ToString()
		{
			return this.ConvertToString();
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			DomainSchema.TenantId,
			DomainSchema.PropertiesAsId
		};
	}
}
