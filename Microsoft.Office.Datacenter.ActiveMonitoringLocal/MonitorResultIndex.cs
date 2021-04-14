using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal abstract class MonitorResultIndex : WorkItemResultIndex<MonitorResult>
	{
		internal static IIndexDescriptor<MonitorResult, string> ComponentNameAndExecutionEndTime(string componentName, DateTime minExecutionEndTime)
		{
			return new MonitorResultIndex.MonitorResultIndexDescriptorForComponentNameAndExecutionEndTime(componentName, minExecutionEndTime);
		}

		internal static IIndexDescriptor<MonitorResult, DateTime> ExecutionEndTime(DateTime executionEndTime)
		{
			return new MonitorResultIndex.MonitorResultIndexDescriptorForExecutionEndTime(executionEndTime);
		}

		private class MonitorResultIndexDescriptorForComponentNameAndExecutionEndTime : WorkItemResultIndex<MonitorResult>.WorkItemResultIndexBase<MonitorResult, string>
		{
			internal MonitorResultIndexDescriptorForComponentNameAndExecutionEndTime(string key, DateTime minExecutionEndTime) : base(key, minExecutionEndTime)
			{
			}

			public override IEnumerable<string> GetKeyValues(MonitorResult item)
			{
				yield return item.ComponentName;
				yield break;
			}

			public override IDataAccessQuery<MonitorResult> ApplyIndexRestriction(IDataAccessQuery<MonitorResult> query)
			{
				IEnumerable<MonitorResult> query2 = from r in query
				where r.ComponentName == base.Key && r.ExecutionEndTime > base.MinExecutionEndTime
				select r;
				return query.AsDataAccessQuery<MonitorResult>(query2);
			}
		}

		private class MonitorResultIndexDescriptorForExecutionEndTime : WorkItemResultIndex<MonitorResult>.WorkItemResultIndexBase<MonitorResult, DateTime>
		{
			internal MonitorResultIndexDescriptorForExecutionEndTime(DateTime key) : base(key, key)
			{
			}

			public override IEnumerable<DateTime> GetKeyValues(MonitorResult item)
			{
				yield return item.ExecutionEndTime;
				yield break;
			}

			public override IDataAccessQuery<MonitorResult> ApplyIndexRestriction(IDataAccessQuery<MonitorResult> query)
			{
				IEnumerable<MonitorResult> query2 = from r in query
				where r.ExecutionEndTime > base.Key
				select r;
				return query.AsDataAccessQuery<MonitorResult>(query2);
			}
		}
	}
}
