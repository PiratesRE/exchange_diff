using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class AggTrafficDatas : ConfigurablePropertyBag
	{
		public AggTrafficDatas(IEnumerable<AggTrafficData> batch)
		{
			this.TvpAggTrafficData = DalHelper.CreateDataTable("AggTrafficDataDt", AggTrafficDatas.DataTableProperties, batch);
		}

		internal AggTrafficDatas(Guid organizationalUnitRoot, DataTable items)
		{
			this.OrganizationalUnitRoot = organizationalUnitRoot;
			this.TvpAggTrafficData = items;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public DataTable TvpAggTrafficData
		{
			get
			{
				return (DataTable)this[AggTrafficDatas.TvpAggTrafficDataProp];
			}
			private set
			{
				this[AggTrafficDatas.TvpAggTrafficDataProp] = value;
			}
		}

		internal Guid OrganizationalUnitRoot { get; set; }

		internal const string AggTrafficDataTable = "AggTrafficDataDt";

		internal static readonly HygienePropertyDefinition TvpAggTrafficDataProp = new HygienePropertyDefinition("tvp_AggTrafficData", typeof(DataTable));

		private static readonly HygienePropertyDefinition[] DataTableProperties = new HygienePropertyDefinition[]
		{
			CommonReportingSchema.OrganizationalUnitRootProperty,
			CommonReportingSchema.DomainHashKeyProp,
			CommonReportingSchema.TrafficTypeProperty,
			CommonReportingSchema.DateKeyProperty,
			CommonReportingSchema.HourKeyProperty,
			CommonReportingSchema.TenantDomainProperty,
			CommonReportingSchema.MessageCountProperty,
			CommonReportingSchema.RecipientCountProperty,
			DalHelper.HashBucketProp
		};
	}
}
