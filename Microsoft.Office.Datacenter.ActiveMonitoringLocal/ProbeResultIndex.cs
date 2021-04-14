using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal abstract class ProbeResultIndex : WorkItemResultIndex<ProbeResult>
	{
		internal static IIndexDescriptor<ProbeResult, string> ScopeNameAndExecutionEndTime(string scopeName, DateTime minExecutionEndTime)
		{
			return new ProbeResultIndex.ProbeResultIndexDescriptorForScopeNameAndExecutionEndTime(scopeName, minExecutionEndTime);
		}

		private class ProbeResultIndexDescriptorForScopeNameAndExecutionEndTime : WorkItemResultIndex<ProbeResult>.WorkItemResultIndexBase<ProbeResult, string>
		{
			internal ProbeResultIndexDescriptorForScopeNameAndExecutionEndTime(string key, DateTime minExecutionEndTime) : base(key, minExecutionEndTime)
			{
			}

			public override IEnumerable<string> GetKeyValues(ProbeResult item)
			{
				yield return item.ScopeName;
				yield break;
			}

			public override IDataAccessQuery<ProbeResult> ApplyIndexRestriction(IDataAccessQuery<ProbeResult> query)
			{
				IEnumerable<ProbeResult> query2 = from r in query
				where r.ScopeName == base.Key && r.ExecutionEndTime > base.MinExecutionEndTime
				select r;
				return query.AsDataAccessQuery<ProbeResult>(query2);
			}
		}
	}
}
