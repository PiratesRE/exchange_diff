using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Hygiene.Data.Directory;
using Microsoft.Forefront.Reporting.Common;

namespace Microsoft.Forefront.Reporting.OnDemandQuery
{
	internal class RequestBatchCompiler
	{
		public RequestBatchCompiler(RequestBatch batch) : this(batch, null, null)
		{
		}

		public RequestBatchCompiler(RequestBatch batch, PIIHashingDelegate piiHashingDelegate, LSHDelegate lshDelegate)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			HashSet<DateTime> hashSet = new HashSet<DateTime>();
			foreach (IGrouping<Guid, OnDemandQueryRequest> grouping in from request in batch.GetAllRequests()
			group request by request.TenantId)
			{
				stringBuilder.AppendFormat("case \"{0}\": ", grouping.Key.ToString());
				stringBuilder2.AppendFormat("case \"{0}\": ", grouping.Key.ToString());
				foreach (OnDemandQueryRequest onDemandQueryRequest in grouping)
				{
					OnDemandQueryType queryType = onDemandQueryRequest.QueryType;
					QueryCompiler queryCompiler = new QueryCompiler(queryType, onDemandQueryRequest.QueryDefinition, piiHashingDelegate, lshDelegate);
					foreach (Tuple<DateTime, DateTime> tuple in queryCompiler.QueryTimeRange)
					{
						DateTime t = tuple.Item1;
						while (t <= tuple.Item2)
						{
							hashSet.Add(t.Date);
							t = t.AddDays(1.0);
						}
					}
					stringBuilder.Append(queryCompiler.CompiledCode);
					stringBuilder.AppendFormat(" results.Add({0});", onDemandQueryRequest.InBatchQueryId);
					stringBuilder2.Append(queryCompiler.PreFilteringCode);
					stringBuilder2.Append(" return true;");
				}
				stringBuilder.AppendLine(" break;");
				stringBuilder2.AppendLine(" break;");
			}
			this.CompiledCode = stringBuilder.ToString();
			this.PrefilteringCode = stringBuilder2.ToString();
			this.QueryDates = (from dt in hashSet
			orderby dt
			select dt).ToList<DateTime>();
		}

		internal string CompiledCode { get; private set; }

		internal string PrefilteringCode { get; private set; }

		internal List<DateTime> QueryDates { get; private set; }
	}
}
