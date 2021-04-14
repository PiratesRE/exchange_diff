using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	internal abstract class WorkDefinitionIndex<TWorkDefinition> where TWorkDefinition : WorkDefinition
	{
		internal static IIndexDescriptor<TWorkDefinition, int> Id(int id)
		{
			return new WorkDefinitionIndex<TWorkDefinition>.WorkDefinitionIndexDescriptorForId<TWorkDefinition>(id);
		}

		internal static IIndexDescriptor<TWorkDefinition, bool> Enabled(bool enabled)
		{
			return new WorkDefinitionIndex<TWorkDefinition>.WorkDefinitionIndexDescriptorForEnabled<TWorkDefinition>(enabled);
		}

		internal static IIndexDescriptor<TWorkDefinition, DateTime> StartTime(DateTime startTime)
		{
			return new WorkDefinitionIndex<TWorkDefinition>.WorkDefinitionIndexDescriptorForStartTime<TWorkDefinition>(startTime);
		}

		protected abstract class WorkDefinitionIndexBase<TDefinition, TKey> : IIndexDescriptor<TDefinition, TKey>, IIndexDescriptor where TDefinition : WorkDefinition
		{
			protected WorkDefinitionIndexBase(TKey key)
			{
				this.key = key;
			}

			public TKey Key
			{
				get
				{
					return this.key;
				}
			}

			public abstract IEnumerable<TKey> GetKeyValues(TDefinition item);

			public abstract IDataAccessQuery<TDefinition> ApplyIndexRestriction(IDataAccessQuery<TDefinition> query);

			private TKey key;
		}

		private class WorkDefinitionIndexDescriptorForId<TDefinition> : WorkDefinitionIndex<TWorkDefinition>.WorkDefinitionIndexBase<TDefinition, int> where TDefinition : WorkDefinition
		{
			internal WorkDefinitionIndexDescriptorForId(int key) : base(key)
			{
			}

			public override IEnumerable<int> GetKeyValues(TDefinition item)
			{
				yield return item.Id;
				yield break;
			}

			public override IDataAccessQuery<TDefinition> ApplyIndexRestriction(IDataAccessQuery<TDefinition> query)
			{
				IEnumerable<TDefinition> query2 = from d in query
				where d.Id == base.Key
				select d;
				return query.AsDataAccessQuery<TDefinition>(query2);
			}
		}

		private class WorkDefinitionIndexDescriptorForEnabled<TDefinition> : WorkDefinitionIndex<TWorkDefinition>.WorkDefinitionIndexBase<TDefinition, bool> where TDefinition : WorkDefinition
		{
			internal WorkDefinitionIndexDescriptorForEnabled(bool key) : base(key)
			{
			}

			public override IEnumerable<bool> GetKeyValues(TDefinition item)
			{
				throw new NotImplementedException();
			}

			public override IDataAccessQuery<TDefinition> ApplyIndexRestriction(IDataAccessQuery<TDefinition> query)
			{
				IEnumerable<TDefinition> query2 = from d in query
				where d.Enabled == base.Key
				select d;
				return query.AsDataAccessQuery<TDefinition>(query2);
			}
		}

		private class WorkDefinitionIndexDescriptorForStartTime<TDefinition> : WorkDefinitionIndex<TWorkDefinition>.WorkDefinitionIndexBase<TDefinition, DateTime> where TDefinition : WorkDefinition
		{
			internal WorkDefinitionIndexDescriptorForStartTime(DateTime key) : base(key)
			{
			}

			public override IEnumerable<DateTime> GetKeyValues(TDefinition item)
			{
				throw new NotImplementedException();
			}

			public override IDataAccessQuery<TDefinition> ApplyIndexRestriction(IDataAccessQuery<TDefinition> query)
			{
				IEnumerable<TDefinition> query2 = from d in query
				where d.StartTime <= base.Key
				select d;
				return query.AsDataAccessQuery<TDefinition>(query2);
			}
		}
	}
}
