using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Search.Query;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[XmlType(TypeName = "QueryExecutionStepType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class QueryExecutionStepType
	{
		internal QueryExecutionStepType()
		{
		}

		internal QueryExecutionStepType(QueryExecutionStep queryExecutionStep)
		{
			this.StartTime = queryExecutionStep.StartTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt", CultureInfo.InvariantCulture);
			this.StepTime = (double)((queryExecutionStep.EndTime.Ticks - queryExecutionStep.StartTime.Ticks) / 10000L);
			this.StepType = queryExecutionStep.StepType.ToString();
			if (queryExecutionStep.AdditionalStatistics != null)
			{
				List<string> list = new List<string>(queryExecutionStep.AdditionalStatistics.Count);
				foreach (KeyValuePair<string, object> keyValuePair in queryExecutionStep.AdditionalStatistics)
				{
					if (keyValuePair.Value != null)
					{
						list.Add(string.Format("{0}=>{1}", keyValuePair.Key, keyValuePair.Value));
					}
				}
				this.AdditionalEntries = list.ToArray();
			}
		}

		[DataMember]
		public string StartTime { get; set; }

		[DataMember]
		public double StepTime { get; set; }

		[DataMember]
		public string StepType { get; set; }

		[DataMember]
		public string[] AdditionalEntries { get; set; }
	}
}
