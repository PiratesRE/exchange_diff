using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class AggPolicyTrafficData : ConfigurablePropertyBag
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

		public Guid DLPId
		{
			get
			{
				return (Guid)this[AggPolicyTrafficData.DLPIdProperty];
			}
			set
			{
				this[AggPolicyTrafficData.DLPIdProperty] = value;
			}
		}

		public Guid RuleId
		{
			get
			{
				return (Guid)this[AggPolicyTrafficData.RuleIdProperty];
			}
			set
			{
				this[AggPolicyTrafficData.RuleIdProperty] = value;
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

		public string TenantDomain { get; set; }

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
			return AggPolicyTrafficData.propertydefinitions;
		}

		internal static readonly HygienePropertyDefinition DLPIdProperty = new HygienePropertyDefinition("DLPId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.Mandatory);

		internal static readonly HygienePropertyDefinition RuleIdProperty = new HygienePropertyDefinition("RuleId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.Mandatory);

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			CommonReportingSchema.OrganizationalUnitRootProperty,
			CommonReportingSchema.DataSourceProperty,
			CommonReportingSchema.TrafficTypeProperty,
			CommonReportingSchema.DateKeyProperty,
			CommonReportingSchema.HourKeyProperty,
			AggPolicyTrafficData.DLPIdProperty,
			AggPolicyTrafficData.RuleIdProperty,
			CommonReportingSchema.MessageCountProperty
		};
	}
}
