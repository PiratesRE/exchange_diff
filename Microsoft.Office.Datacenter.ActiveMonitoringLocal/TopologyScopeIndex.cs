using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class TopologyScopeIndex : IIndexDescriptor<TopologyScope, TopologyScope>, IIndexDescriptor
	{
		internal TopologyScopeIndex()
		{
		}

		public TopologyScope Key
		{
			get
			{
				return null;
			}
		}

		public IEnumerable<TopologyScope> GetKeyValues(TopologyScope item)
		{
			yield return item;
			yield break;
		}

		public IDataAccessQuery<TopologyScope> ApplyIndexRestriction(IDataAccessQuery<TopologyScope> query)
		{
			return query;
		}
	}
}
