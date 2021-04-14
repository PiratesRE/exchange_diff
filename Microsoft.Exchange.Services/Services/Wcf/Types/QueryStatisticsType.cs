using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Search.Query;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "QueryStatisticsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class QueryStatisticsType
	{
		internal QueryStatisticsType()
		{
		}

		internal QueryStatisticsType(QueryStatistics queryStatistics)
		{
			this.StoreByPassed = queryStatistics.StoreBypassed;
			this.Version = queryStatistics.Version;
			this.StartTime = queryStatistics.StartTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt", CultureInfo.InvariantCulture);
			this.QueryTime = (double)((queryStatistics.EndTime.Ticks - queryStatistics.StartTime.Ticks) / 10000L);
			IReadOnlyCollection<QueryExecutionStep> steps = queryStatistics.Steps;
			if (steps != null)
			{
				this.QueryStepsCount = steps.Count;
				this.Steps = new QueryExecutionStepType[steps.Count];
				int num = 0;
				foreach (QueryExecutionStep queryExecutionStep in steps)
				{
					this.Steps[num++] = new QueryExecutionStepType(queryExecutionStep);
				}
			}
		}

		[DataMember]
		public bool StoreByPassed { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public string StartTime { get; set; }

		[DataMember]
		public double QueryTime { get; set; }

		[DataMember]
		public int QueryStepsCount { get; set; }

		[DataMember]
		public QueryExecutionStepType[] Steps { get; set; }
	}
}
