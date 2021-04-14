using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	[Serializable]
	internal class Zone : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return DomainSchema.GetObjectId(this.ZoneId);
			}
		}

		public Guid ZoneId
		{
			get
			{
				return (Guid)this[DomainSchema.ZoneId];
			}
			set
			{
				this[DomainSchema.ZoneId] = value;
			}
		}

		public string DomainName
		{
			get
			{
				return (string)this[DomainSchema.DomainName];
			}
			set
			{
				this[DomainSchema.DomainName] = DomainSchema.GetNullIfStringEmpty(value);
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return Zone.propertydefinitions;
		}

		public override string ToString()
		{
			return this.ConvertToString();
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			DomainSchema.ZoneId,
			DomainSchema.DomainName
		};
	}
}
