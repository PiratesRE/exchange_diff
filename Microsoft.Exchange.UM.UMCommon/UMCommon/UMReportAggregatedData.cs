using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.UM.UMCommon
{
	[DataContract(Name = "UMReportAggregatedData", Namespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData")]
	internal class UMReportAggregatedData : IUMAggregatedData
	{
		[DataMember(Name = "WaterMark")]
		public DateTime WaterMark { get; private set; }

		public UMReportAggregatedData()
		{
			this.cleanUpNeeded = false;
			this.aggregatedData = new AggregatedDataDictionary();
			this.WaterMark = default(DateTime);
		}

		public void AddCDR(CDRData cdrData)
		{
			ValidateArgument.NotNull(cdrData, "cdrData");
			UMReportTuple[] tuplesToAddInReport = UMReportTuple.GetTuplesToAddInReport(cdrData);
			foreach (UMReportTuple key in tuplesToAddInReport)
			{
				UMReportTupleData umreportTupleData;
				if (!this.aggregatedData.TryGetValue(key, out umreportTupleData))
				{
					umreportTupleData = new UMReportTupleData();
					this.aggregatedData.Add(key, umreportTupleData);
				}
				umreportTupleData.AddCDR(cdrData);
			}
			this.WaterMark = cdrData.CreationTime;
			this.cleanUpNeeded = true;
		}

		public UMReportRawCounters[] QueryAggregatedData(Guid dialPlanGuid, Guid gatewayGuid, GroupBy groupBy)
		{
			UMReportTuple key = new UMReportTuple(dialPlanGuid, gatewayGuid);
			UMReportTupleData umreportTupleData;
			if (this.aggregatedData.TryGetValue(key, out umreportTupleData))
			{
				return umreportTupleData.QueryReport(groupBy);
			}
			return null;
		}

		public void Cleanup(OrganizationId orgId)
		{
			if (this.aggregatedData.Count > 0)
			{
				AggregatedDataDictionary aggregatedDataDictionary = new AggregatedDataDictionary(this.aggregatedData.Count);
				foreach (UMReportTuple umreportTuple in this.aggregatedData.Keys)
				{
					if (!umreportTuple.ShouldRemoveFromReport(orgId))
					{
						UMReportTupleData umreportTupleData = this.aggregatedData[umreportTuple];
						umreportTupleData.CleanUp();
						aggregatedDataDictionary[umreportTuple] = umreportTupleData;
					}
				}
				this.aggregatedData = aggregatedDataDictionary;
			}
			this.cleanUpNeeded = false;
		}

		[OnSerializing]
		private void Cleanup(StreamingContext context)
		{
			if (this.cleanUpNeeded)
			{
				throw new InvalidOperationException("Aggregated data is being serialized without cleanup.");
			}
		}

		private bool cleanUpNeeded;

		[DataMember(Name = "AggregatedData")]
		private AggregatedDataDictionary aggregatedData;
	}
}
