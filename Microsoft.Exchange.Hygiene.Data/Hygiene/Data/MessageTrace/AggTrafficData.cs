using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class AggTrafficData : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[CommonReportingSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[CommonReportingSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public string TrafficType
		{
			get
			{
				return (string)this[CommonReportingSchema.TrafficTypeProperty];
			}
			set
			{
				this[CommonReportingSchema.TrafficTypeProperty] = value;
			}
		}

		public int DateKey
		{
			get
			{
				return (int)this[CommonReportingSchema.DateKeyProperty];
			}
			set
			{
				this[CommonReportingSchema.DateKeyProperty] = value;
			}
		}

		public short HourKey
		{
			get
			{
				return (short)this[CommonReportingSchema.HourKeyProperty];
			}
			set
			{
				this[CommonReportingSchema.HourKeyProperty] = value;
			}
		}

		public string TenantDomain
		{
			get
			{
				return (string)this[CommonReportingSchema.TenantDomainProperty];
			}
			set
			{
				this[CommonReportingSchema.TenantDomainProperty] = value;
			}
		}

		public long MessageCount
		{
			get
			{
				return (long)this[CommonReportingSchema.MessageCountProperty];
			}
			set
			{
				this[CommonReportingSchema.MessageCountProperty] = value;
			}
		}

		public long RecipientCount
		{
			get
			{
				return (long)this[CommonReportingSchema.RecipientCountProperty];
			}
			set
			{
				this[CommonReportingSchema.RecipientCountProperty] = value;
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return AggTrafficData.propertydefinitions;
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			CommonReportingSchema.OrganizationalUnitRootProperty,
			CommonReportingSchema.TrafficTypeProperty,
			CommonReportingSchema.DateKeyProperty,
			CommonReportingSchema.HourKeyProperty,
			CommonReportingSchema.TenantDomainProperty,
			CommonReportingSchema.MessageCountProperty,
			CommonReportingSchema.RecipientCountProperty
		};
	}
}
