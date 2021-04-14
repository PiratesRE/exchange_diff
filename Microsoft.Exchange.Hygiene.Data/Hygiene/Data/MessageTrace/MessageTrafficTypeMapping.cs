using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageTrafficTypeMapping : ConfigurablePropertyBag
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

		public string DataSource
		{
			get
			{
				return this[CommonReportingSchema.DataSourceProperty] as string;
			}
			set
			{
				this[CommonReportingSchema.DataSourceProperty] = value;
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

		public Guid ExMessageId
		{
			get
			{
				return (Guid)this[MessageTrafficTypeMapping.ExMessageIdProperty];
			}
			set
			{
				this[MessageTrafficTypeMapping.ExMessageIdProperty] = value;
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

		public string TenantDomain { get; set; }

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return MessageTrafficTypeMapping.propertydefinitions;
		}

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = new HygienePropertyDefinition("ExMessageId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.Mandatory);

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			CommonReportingSchema.OrganizationalUnitRootProperty,
			CommonReportingSchema.DataSourceProperty,
			CommonReportingSchema.TrafficTypeProperty,
			MessageTrafficTypeMapping.ExMessageIdProperty,
			CommonReportingSchema.DateKeyProperty,
			CommonReportingSchema.HourKeyProperty
		};
	}
}
