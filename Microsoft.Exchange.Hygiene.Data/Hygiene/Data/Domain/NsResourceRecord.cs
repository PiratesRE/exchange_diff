using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	[Serializable]
	internal class NsResourceRecord : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return DomainSchema.GetObjectId(this.ResourceRecordId);
			}
		}

		public Guid ResourceRecordId
		{
			get
			{
				return (Guid)this[DomainSchema.ResourceRecordId];
			}
			set
			{
				this[DomainSchema.ResourceRecordId] = value;
			}
		}

		public string NameServer
		{
			get
			{
				return (string)this[DomainSchema.NameServer];
			}
			set
			{
				this[DomainSchema.NameServer] = DomainSchema.GetNullIfStringEmpty(value);
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

		public string IpAddress
		{
			get
			{
				return (string)this[DomainSchema.IpAddress];
			}
			set
			{
				this[DomainSchema.IpAddress] = DomainSchema.GetNullIfStringEmpty(value);
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return NsResourceRecord.propertydefinitions;
		}

		public override string ToString()
		{
			return this.ConvertToString();
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			DomainSchema.ResourceRecordId,
			DomainSchema.NameServer,
			DomainSchema.DomainName,
			DomainSchema.IpAddress
		};
	}
}
