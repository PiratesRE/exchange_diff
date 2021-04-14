using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class BandMailboxRebalanceData
	{
		public BandMailboxRebalanceData(LoadContainer sourceDatabase, LoadContainer targetDatabase, LoadMetricStorage rebalanceInformation)
		{
			this.SourceDatabase = sourceDatabase.GetShallowCopy();
			this.TargetDatabase = targetDatabase.GetShallowCopy();
			this.RebalanceInformation = rebalanceInformation;
		}

		[DataMember(Order = 1)]
		public string ConstraintSetIdentity { get; set; }

		[DataMember(Order = 0)]
		public string RebalanceBatchName { get; set; }

		[DataMember(Order = 0)]
		public LoadMetricStorage RebalanceInformation { get; private set; }

		[DataMember(Order = 0)]
		public LoadContainer SourceDatabase { get; private set; }

		[DataMember(Order = 0)]
		public LoadContainer TargetDatabase { get; private set; }

		public void ConvertToFromSerializationFormat()
		{
			this.RebalanceInformation.ConvertFromSerializationFormat();
			this.SourceDatabase.ConvertFromSerializationFormat();
			this.TargetDatabase.ConvertFromSerializationFormat();
		}

		public BandMailboxRebalanceData ToSerializationFormat(bool convertBandToBandData)
		{
			LoadMetricStorage loadMetricStorage = new LoadMetricStorage(this.RebalanceInformation);
			loadMetricStorage.ConvertToSerializationMetrics(convertBandToBandData);
			return new BandMailboxRebalanceData((LoadContainer)this.SourceDatabase.ToSerializationFormat(convertBandToBandData), (LoadContainer)this.TargetDatabase.ToSerializationFormat(convertBandToBandData), loadMetricStorage)
			{
				RebalanceBatchName = this.RebalanceBatchName,
				ConstraintSetIdentity = this.ConstraintSetIdentity
			};
		}

		public override string ToString()
		{
			return string.Format("Rebalance[CSET: {0}; From: {1}; To: {2}; {3}", new object[]
			{
				this.ConstraintSetIdentity,
				this.SourceDatabase.Guid,
				this.TargetDatabase.Guid,
				this.RebalanceInformation
			});
		}
	}
}
