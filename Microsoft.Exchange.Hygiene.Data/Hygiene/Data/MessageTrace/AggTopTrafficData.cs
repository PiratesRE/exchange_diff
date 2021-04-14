using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class AggTopTrafficData : ConfigurablePropertyBag
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

		public string AttributeValue
		{
			get
			{
				return (string)this[AggTopTrafficData.AttributeValueProperty];
			}
			set
			{
				this[AggTopTrafficData.AttributeValueProperty] = value;
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

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return AggTopTrafficData.propertydefinitions;
		}

		internal static readonly HygienePropertyDefinition AttributeValueProperty = new HygienePropertyDefinition("AttributeValue", typeof(string), string.Empty, ADPropertyDefinitionFlags.Mandatory);

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			CommonReportingSchema.OrganizationalUnitRootProperty,
			CommonReportingSchema.TrafficTypeProperty,
			CommonReportingSchema.DateKeyProperty,
			CommonReportingSchema.HourKeyProperty,
			AggTopTrafficData.AttributeValueProperty,
			CommonReportingSchema.TenantDomainProperty,
			CommonReportingSchema.MessageCountProperty
		};
	}
}
