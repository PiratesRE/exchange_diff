using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Migration
{
	internal abstract class MigrationHierarchyProcessorBase<TParent, TChild, TChildId, TResponse> : MigrationProcessorBase<TParent, TResponse> where TResponse : MigrationProcessorResponse
	{
		protected MigrationHierarchyProcessorBase(TParent migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected abstract MigrationProcessorResponse DefaultCorruptedChildResponse { get; }

		protected abstract Func<int?, IEnumerable<TChildId>>[] ProcessableChildObjectQueries { get; }

		protected abstract int? MaxChildObjectsToProcessCount { get; }

		protected override TResponse InternalProcess()
		{
			TResponse result = this.ProcessObject();
			int num = 0;
			int? maxCount = this.MaxChildObjectsToProcessCount;
			if (maxCount != null && maxCount.Value <= 0)
			{
				maxCount = null;
			}
			foreach (TChildId childId in this.GetChildObjectIds(this.ProcessableChildObjectQueries, maxCount).Distinct<TChildId>().ToArray<TChildId>())
			{
				TChild child;
				MigrationProcessorResponse childResponse;
				if (this.TryLoad(childId, out child))
				{
					childResponse = this.ProcessChild(child);
				}
				else
				{
					childResponse = this.DefaultCorruptedChildResponse;
				}
				result.Aggregate(childResponse);
				num++;
			}
			if (maxCount != null && maxCount.Value == num)
			{
				result.Aggregate(MigrationProcessorResponse.Create(MigrationProcessorResult.Working, null, null));
			}
			return result;
		}

		protected virtual IEnumerable<TChildId> GetChildObjectIds(Func<int?, IEnumerable<TChildId>>[] queries, int? maxCount = null)
		{
			int totalEncountered = 0;
			int? maxCountRemaining = maxCount;
			foreach (Func<int?, IEnumerable<TChildId>> query in queries)
			{
				if (maxCountRemaining != null && totalEncountered > 0)
				{
					maxCountRemaining = new int?(maxCountRemaining.Value - totalEncountered);
				}
				foreach (TChildId messageId in query(maxCountRemaining))
				{
					totalEncountered++;
					yield return messageId;
					if (maxCount != null && totalEncountered >= maxCount.Value)
					{
						yield break;
					}
				}
			}
			yield break;
		}

		protected abstract bool TryLoad(TChildId childId, out TChild child);

		protected abstract MigrationProcessorResponse ProcessChild(TChild child);

		protected abstract TResponse ProcessObject();
	}
}
