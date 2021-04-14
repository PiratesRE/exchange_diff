using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Local
{
	internal class DefinitionTable<TWorkDefinition> : SimpleTable<TWorkDefinition, int, TWorkDefinition> where TWorkDefinition : WorkDefinition
	{
		public DefinitionTable() : base(WorkDefinitionIndex<TWorkDefinition>.Id(0))
		{
		}

		protected override TWorkDefinition CreateSegment(TWorkDefinition item)
		{
			return item;
		}

		protected override bool AddToSegment(TWorkDefinition segment, TWorkDefinition item)
		{
			if (segment != item)
			{
				string message = string.Format("An item with the same primary key {0} already exists in the table. Existing definition name: {1}. New definition name: {2}", segment.Id, segment.Name, item.Name);
				throw new InvalidOperationException(message);
			}
			return true;
		}

		protected override IEnumerable<TWorkDefinition> GetItemsFromSegment<TKey>(TWorkDefinition segment, IIndexDescriptor<TWorkDefinition, TKey> indexDescriptor)
		{
			return new TWorkDefinition[]
			{
				segment
			};
		}

		protected override IEnumerable<TWorkDefinition> GetItemsFromSegments<TKey>(IEnumerable<TWorkDefinition> segments, IIndexDescriptor<TWorkDefinition, TKey> indexDescriptor)
		{
			return segments;
		}
	}
}
