using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageTrafficTypeMappings : ConfigurablePropertyBag
	{
		public MessageTrafficTypeMappings(IEnumerable<MessageTrafficTypeMapping> batch)
		{
			this.TvpMessageTrafficTypeMapping = DalHelper.CreateDataTable("MessageTrafficTypeMappingDt", MessageTrafficTypeMappings.DataTableProperties, batch);
		}

		internal MessageTrafficTypeMappings(Guid organizationalUnitRoot, DataTable items)
		{
			this.OrganizationalUnitRoot = organizationalUnitRoot;
			this.TvpMessageTrafficTypeMapping = items;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public DataTable TvpMessageTrafficTypeMapping
		{
			get
			{
				return (DataTable)this[MessageTrafficTypeMappings.TvpMessageTrafficTypeMappingProp];
			}
			private set
			{
				this[MessageTrafficTypeMappings.TvpMessageTrafficTypeMappingProp] = value;
			}
		}

		internal Guid OrganizationalUnitRoot { get; set; }

		internal const string MessageTrafficTypeMappingTable = "MessageTrafficTypeMappingDt";

		internal static readonly HygienePropertyDefinition TvpMessageTrafficTypeMappingProp = new HygienePropertyDefinition("tvp_MessageTrafficTypeMapping", typeof(DataTable));

		private static readonly HygienePropertyDefinition[] DataTableProperties = new HygienePropertyDefinition[]
		{
			CommonReportingSchema.OrganizationalUnitRootProperty,
			CommonReportingSchema.DataSourceProperty,
			CommonReportingSchema.DomainHashKeyProp,
			CommonReportingSchema.TrafficTypeProperty,
			MessageTrafficTypeMapping.ExMessageIdProperty,
			CommonReportingSchema.DateKeyProperty,
			CommonReportingSchema.HourKeyProperty,
			DalHelper.HashBucketProp
		};
	}
}
