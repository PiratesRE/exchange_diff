using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal abstract class MonitorDefinitionIndex : WorkDefinitionIndex<MonitorDefinition>
	{
		internal static IIndexDescriptor<MonitorDefinition, string> SampleMask(string sampleMask)
		{
			return new MonitorDefinitionIndex.MonitorDefinitionIndexDescriptorForSampleMask(sampleMask);
		}

		private class MonitorDefinitionIndexDescriptorForSampleMask : WorkDefinitionIndex<MonitorDefinition>.WorkDefinitionIndexBase<MonitorDefinition, string>
		{
			internal MonitorDefinitionIndexDescriptorForSampleMask(string key) : base(key)
			{
			}

			public override IEnumerable<string> GetKeyValues(MonitorDefinition item)
			{
				yield return item.SampleMask;
				yield break;
			}

			public override IDataAccessQuery<MonitorDefinition> ApplyIndexRestriction(IDataAccessQuery<MonitorDefinition> query)
			{
				IEnumerable<MonitorDefinition> enumerable = from d in query
				select d;
				if (IndexCapabilities.SupportsCaseInsensitiveStringComparison)
				{
					enumerable = from d in enumerable
					where d.SampleMask.Equals(base.Key, StringComparison.OrdinalIgnoreCase)
					select d;
				}
				else
				{
					enumerable = from d in enumerable
					where d.SampleMask.Equals(base.Key)
					select d;
				}
				return query.AsDataAccessQuery<MonitorDefinition>(enumerable);
			}
		}
	}
}
