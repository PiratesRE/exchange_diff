using System;
using System.Collections.Generic;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	internal class WorkDefinitionOverrideIndex : IIndexDescriptor<WorkDefinitionOverride, WorkDefinitionOverride>, IIndexDescriptor
	{
		internal WorkDefinitionOverrideIndex()
		{
		}

		public WorkDefinitionOverride Key
		{
			get
			{
				return null;
			}
		}

		public IEnumerable<WorkDefinitionOverride> GetKeyValues(WorkDefinitionOverride item)
		{
			yield return item;
			yield break;
		}

		public IDataAccessQuery<WorkDefinitionOverride> ApplyIndexRestriction(IDataAccessQuery<WorkDefinitionOverride> query)
		{
			return query;
		}
	}
}
