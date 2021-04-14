using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class AggTopTrafficDatas : ConfigurablePropertyBag
	{
		public AggTopTrafficDatas(IEnumerable<AggTopTrafficData> batch)
		{
			this.TvpAggTopTrafficData = DalHelper.CreateDataTable("AggTopTrafficDataDt", AggTopTrafficDatas.DataTableProperties, batch);
		}

		internal AggTopTrafficDatas(Guid organizationalUnitRoot, DataTable items)
		{
			this.OrganizationalUnitRoot = organizationalUnitRoot;
			this.TvpAggTopTrafficData = items;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public DataTable TvpAggTopTrafficData
		{
			get
			{
				return (DataTable)this[AggTopTrafficDatas.TvpAggTopTrafficDataProp];
			}
			private set
			{
				this[AggTopTrafficDatas.TvpAggTopTrafficDataProp] = value;
			}
		}

		internal Guid OrganizationalUnitRoot { get; set; }

		internal const string AggTopTrafficDataTable = "AggTopTrafficDataDt";

		internal static readonly HygienePropertyDefinition TvpAggTopTrafficDataProp = new HygienePropertyDefinition("tvp_AggTopTrafficData", typeof(DataTable));

		private static readonly HygienePropertyDefinition[] DataTableProperties = new HygienePropertyDefinition[]
		{
			CommonReportingSchema.OrganizationalUnitRootProperty,
			CommonReportingSchema.DomainHashKeyProp,
			CommonReportingSchema.TrafficTypeProperty,
			CommonReportingSchema.DateKeyProperty,
			CommonReportingSchema.HourKeyProperty,
			AggTopTrafficData.AttributeValueProperty,
			CommonReportingSchema.TenantDomainProperty,
			CommonReportingSchema.MessageCountProperty,
			DalHelper.HashBucketProp
		};
	}
}
