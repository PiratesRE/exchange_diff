using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	internal class DICommon : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return DomainSchema.GetObjectId(this.Identifier);
			}
		}

		public Guid Identifier
		{
			get
			{
				return (Guid)this[DomainSchema.Identifier];
			}
		}

		public Guid TenantId
		{
			get
			{
				return DomainSchema.GetGuidEmptyIfNull(this[DomainSchema.TenantId]);
			}
		}

		public DataDatetimeGroup EntityTimeStamp
		{
			get
			{
				return new DataDatetimeGroup
				{
					createdDatetime = (DateTime?)this[DomainSchema.CreatedDatetime],
					changedDatetime = (DateTime?)this[DomainSchema.ChangedDatetime],
					deletedDatetime = (DateTime?)this[DomainSchema.DeletedDatetime]
				};
			}
		}

		public DataDatetimeGroup PropertyTimeStamp
		{
			get
			{
				return new DataDatetimeGroup
				{
					createdDatetime = (DateTime?)this[DomainSchema.PropertyCreatedDatetime],
					changedDatetime = (DateTime?)this[DomainSchema.PropertyChangedDatetime],
					deletedDatetime = (DateTime?)this[DomainSchema.PropertyDeletedDatetime]
				};
			}
		}

		public int? PropertyId
		{
			get
			{
				return (int?)this[DomainSchema.PropertyId];
			}
		}

		public int? EntityId
		{
			get
			{
				return (int?)this[DomainSchema.EntityId];
			}
		}

		public string PropertyValue
		{
			get
			{
				return this[DomainSchema.PropertyValue] as string;
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return DICommon.propertydefinitions;
		}

		public override string ToString()
		{
			return this.ConvertToString();
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			DomainSchema.TenantId,
			DomainSchema.PropertyValue,
			DomainSchema.EntityId,
			DomainSchema.PropertyId,
			DomainSchema.CreatedDatetime,
			DomainSchema.ChangedDatetime,
			DomainSchema.DeletedDatetime,
			DomainSchema.PropertyCreatedDatetime,
			DomainSchema.PropertyChangedDatetime,
			DomainSchema.PropertyDeletedDatetime
		};
	}
}
