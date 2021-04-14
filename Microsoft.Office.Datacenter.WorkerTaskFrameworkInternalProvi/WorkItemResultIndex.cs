using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	internal class WorkItemResultIndex<TWorkItemResult> where TWorkItemResult : WorkItemResult
	{
		internal static IIndexDescriptor<TWorkItemResult, int> WorkItemIdAndExecutionEndTime(int workItemId, DateTime minExecutionEndTime)
		{
			return new WorkItemResultIndex<TWorkItemResult>.WorkItemResultIndexDescriptorForWorkItemIdAndExecutionEndTime<TWorkItemResult>(workItemId, minExecutionEndTime);
		}

		internal static IIndexDescriptor<TWorkItemResult, string> ResultNameAndExecutionEndTime(string resultName, DateTime minExecutionEndTime)
		{
			return new WorkItemResultIndex<TWorkItemResult>.WorkItemResultIndexDescriptorForResultNameAndExecutionEndTime<TWorkItemResult>(resultName, minExecutionEndTime);
		}

		internal static IIndexDescriptor<TWorkItemResult, string> AllResultsAndExecutionEndTime(DateTime minExecutionEndTime)
		{
			return new WorkItemResultIndex<TWorkItemResult>.WorkItemResultIndexDescriptorForALLResultsAndExecutionEndTime<TWorkItemResult>(null, minExecutionEndTime);
		}

		internal abstract class WorkItemResultIndexBase<TResult, TKey> : IIndexDescriptor<TResult, TKey>, IIndexDescriptor where TResult : WorkItemResult
		{
			protected WorkItemResultIndexBase(TKey key, DateTime minExecutionEndTime)
			{
				this.key = key;
				this.MinExecutionEndTime = minExecutionEndTime;
			}

			public DateTime MinExecutionEndTime { get; set; }

			public TKey Key
			{
				get
				{
					return this.key;
				}
			}

			public abstract IEnumerable<TKey> GetKeyValues(TResult item);

			public abstract IDataAccessQuery<TResult> ApplyIndexRestriction(IDataAccessQuery<TResult> query);

			private TKey key;
		}

		private class WorkItemResultIndexDescriptorForWorkItemIdAndExecutionEndTime<TResult> : WorkItemResultIndex<TWorkItemResult>.WorkItemResultIndexBase<TResult, int> where TResult : WorkItemResult
		{
			internal WorkItemResultIndexDescriptorForWorkItemIdAndExecutionEndTime(int key, DateTime minExecutionEndTime) : base(key, minExecutionEndTime)
			{
			}

			public override IEnumerable<int> GetKeyValues(TResult item)
			{
				yield return item.WorkItemId;
				yield break;
			}

			public override IDataAccessQuery<TResult> ApplyIndexRestriction(IDataAccessQuery<TResult> query)
			{
				IEnumerable<TResult> query2 = from r in query
				where r.WorkItemId == base.Key && r.ExecutionEndTime > base.MinExecutionEndTime
				select r;
				return query.AsDataAccessQuery<TResult>(query2);
			}
		}

		private class WorkItemResultIndexDescriptorForResultNameAndExecutionEndTime<TResult> : WorkItemResultIndex<TWorkItemResult>.WorkItemResultIndexBase<TResult, string> where TResult : WorkItemResult
		{
			internal WorkItemResultIndexDescriptorForResultNameAndExecutionEndTime(string key, DateTime minExecutionEndTime) : base(key, minExecutionEndTime)
			{
			}

			public override IEnumerable<string> GetKeyValues(TResult item)
			{
				string name = item.ResultName;
				while (!string.IsNullOrEmpty(name))
				{
					yield return name;
					int slashIndex = name.LastIndexOf('/');
					if (slashIndex <= 0)
					{
						break;
					}
					name = name.Substring(0, slashIndex);
				}
				yield break;
			}

			public override IDataAccessQuery<TResult> ApplyIndexRestriction(IDataAccessQuery<TResult> query)
			{
				IEnumerable<TResult> enumerable = from r in query
				select r;
				if (IndexCapabilities.SupportsCaseInsensitiveStringComparison)
				{
					enumerable = from r in enumerable
					where r.ResultName.StartsWith(base.Key, StringComparison.OrdinalIgnoreCase) && r.ExecutionEndTime > base.MinExecutionEndTime
					select r;
				}
				else
				{
					enumerable = from r in enumerable
					where r.ResultName.StartsWith(base.Key) && r.ExecutionEndTime > base.MinExecutionEndTime
					select r;
				}
				return query.AsDataAccessQuery<TResult>(enumerable);
			}
		}

		private class WorkItemResultIndexDescriptorForALLResultsAndExecutionEndTime<TResult> : WorkItemResultIndex<TWorkItemResult>.WorkItemResultIndexBase<TResult, string> where TResult : WorkItemResult
		{
			internal WorkItemResultIndexDescriptorForALLResultsAndExecutionEndTime(string key, DateTime minExecutionEndTime) : base(key, minExecutionEndTime)
			{
			}

			public override IEnumerable<string> GetKeyValues(TResult item)
			{
				throw new NotImplementedException();
			}

			public override IDataAccessQuery<TResult> ApplyIndexRestriction(IDataAccessQuery<TResult> query)
			{
				IEnumerable<TResult> query2 = from r in query
				where r.ExecutionEndTime > base.MinExecutionEndTime
				select r;
				return query.AsDataAccessQuery<TResult>(query2);
			}
		}
	}
}
