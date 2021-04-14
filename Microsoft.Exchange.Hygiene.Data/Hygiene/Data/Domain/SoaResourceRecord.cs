using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	[Serializable]
	internal class SoaResourceRecord : ConfigurablePropertyBag
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

		public string PrimaryNameServer
		{
			get
			{
				return (string)this[DomainSchema.PrimaryNameServer];
			}
			set
			{
				this[DomainSchema.PrimaryNameServer] = DomainSchema.GetNullIfStringEmpty(value);
			}
		}

		public string ResponsibleMailServer
		{
			get
			{
				return (string)this[DomainSchema.ResponsibleMailServer];
			}
			set
			{
				this[DomainSchema.ResponsibleMailServer] = DomainSchema.GetNullIfStringEmpty(value);
			}
		}

		public int Refresh
		{
			get
			{
				return (int)this[DomainSchema.Refresh];
			}
			set
			{
				this[DomainSchema.Refresh] = value;
			}
		}

		public int Retry
		{
			get
			{
				return (int)this[DomainSchema.Retry];
			}
			set
			{
				this[DomainSchema.Retry] = value;
			}
		}

		public int Expire
		{
			get
			{
				return (int)this[DomainSchema.Expire];
			}
			set
			{
				this[DomainSchema.Expire] = value;
			}
		}

		public int Serial
		{
			get
			{
				return (int)this[DomainSchema.Serial];
			}
			set
			{
				this[DomainSchema.Serial] = value;
			}
		}

		public int DefaultTtl
		{
			get
			{
				return (int)this[DomainSchema.DefaultTtl];
			}
			set
			{
				this[DomainSchema.DefaultTtl] = value;
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return SoaResourceRecord.propertydefinitions;
		}

		public override string ToString()
		{
			return this.ConvertToString();
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			DomainSchema.ResourceRecordId,
			DomainSchema.DomainName,
			DomainSchema.PrimaryNameServer,
			DomainSchema.ResponsibleMailServer,
			DomainSchema.Refresh,
			DomainSchema.Retry,
			DomainSchema.Expire,
			DomainSchema.Serial,
			DomainSchema.DefaultTtl
		};
	}
}
