using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class AggPolicyTrafficDatas : ConfigurablePropertyBag
	{
		public AggPolicyTrafficDatas(IEnumerable<AggPolicyTrafficData> batch)
		{
			this.TvpAggPolicyTrafficData = DalHelper.CreateDataTable("AggPolicyTrafficDt", AggPolicyTrafficDatas.DataTableProperties, batch);
		}

		internal AggPolicyTrafficDatas(Guid organizationalUnitRoot, DataTable items)
		{
			this.OrganizationalUnitRoot = organizationalUnitRoot;
			this.TvpAggPolicyTrafficData = items;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public DataTable TvpAggPolicyTrafficData
		{
			get
			{
				return (DataTable)this[AggPolicyTrafficDatas.TvpAggPolicyTrafficDataProp];
			}
			private set
			{
				this[AggPolicyTrafficDatas.TvpAggPolicyTrafficDataProp] = value;
			}
		}

		internal Guid OrganizationalUnitRoot { get; set; }

		internal const string AggPolicyTrafficDataTable = "AggPolicyTrafficDt";

		internal static readonly HygienePropertyDefinition TvpAggPolicyTrafficDataProp = new HygienePropertyDefinition("tvp_AggPolicyTrafficData", typeof(DataTable));

		private static readonly HygienePropertyDefinition[] DataTableProperties = new HygienePropertyDefinition[]
		{
			CommonReportingSchema.OrganizationalUnitRootProperty,
			CommonReportingSchema.DataSourceProperty,
			AggPolicyTrafficData.DLPIdProperty,
			AggPolicyTrafficData.RuleIdProperty,
			CommonReportingSchema.DomainHashKeyProp,
			CommonReportingSchema.TrafficTypeProperty,
			CommonReportingSchema.DateKeyProperty,
			CommonReportingSchema.HourKeyProperty,
			CommonReportingSchema.MessageCountProperty,
			DalHelper.HashBucketProp
		};
	}
}
