using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ForwardOnlyFilteredReader : IDisposeTrackable, IDisposable
	{
		protected ForwardOnlyFilteredReader(ForwardOnlyFilteredReader.PropertySetMixer propertySets, bool usePrefetchMode)
		{
			this.allPropertySets = propertySets;
			this.usePrefetchMode = usePrefetchMode;
			this.queryPropertySets = new ForwardOnlyFilteredReader.PropertySetMixer(new Predicate<PropertyDefinition>(this.ShouldIntercept));
			this.queryPropertySets.MigrateSets(this.allPropertySets, new ForwardOnlyFilteredReader.PropertySet[]
			{
				ForwardOnlyFilteredReader.PropertySet.Identification,
				ForwardOnlyFilteredReader.PropertySet.ForFilter
			});
			if (this.usePrefetchMode)
			{
				this.queryPropertySets.MigrateSets(this.allPropertySets, new ForwardOnlyFilteredReader.PropertySet[]
				{
					ForwardOnlyFilteredReader.PropertySet.Requested
				});
			}
			else
			{
				this.queryPropertySets.AddSet(ForwardOnlyFilteredReader.PropertySet.Requested, this.allPropertySets.GetSet(ForwardOnlyFilteredReader.PropertySet.Identification));
				this.populationPropertySets = new ForwardOnlyFilteredReader.PropertySetMixer(new Predicate<PropertyDefinition>(this.ShouldIntercept));
				this.populationPropertySets.MigrateSets(this.allPropertySets, new ForwardOnlyFilteredReader.PropertySet[]
				{
					ForwardOnlyFilteredReader.PropertySet.Identification,
					ForwardOnlyFilteredReader.PropertySet.Requested
				});
			}
			this.query = new LazilyInitialized<QueryResult>(() => this.MakeQuery(this.queryPropertySets.GetFilteredMergedSet()));
			this.disposeTracker = this.GetDisposeTracker();
		}

		internal virtual object[][] GetNextAsView(int rowcount)
		{
			return this.GetNextAsView(delegate(object[] transformedForFilterRow)
			{
				if (transformedForFilterRow != null)
				{
					return rowcount-- > 0;
				}
				return rowcount > 0;
			});
		}

		internal virtual object[][] GetNextAsView(ForwardOnlyFilteredReader.StatefulRowPredicate fetchWhile)
		{
			List<object[]> list = new List<object[]>();
			if (fetchWhile == null)
			{
				fetchWhile = ((object[] forFilterRow) => true);
			}
			while (this.InternalGetNextAsView(list, fetchWhile))
			{
			}
			if (!this.usePrefetchMode)
			{
				this.PopulateRows(list);
			}
			return list.ToArray();
		}

		public abstract DisposeTracker GetDisposeTracker();

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public virtual void Dispose()
		{
			if (this.query.IsInitialized)
			{
				this.query.Value.Dispose();
			}
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
		}

		protected QueryResult Query
		{
			get
			{
				return this.query;
			}
		}

		protected ForwardOnlyFilteredReader.PropertySetMixer PropertySets
		{
			get
			{
				return this.allPropertySets;
			}
		}

		protected virtual bool EvaluateFilterCriteria(object[] forFilterRow)
		{
			return true;
		}

		protected virtual object[][] TransformRow(object[] unfilteredRow)
		{
			if (!this.EvaluateFilterCriteria(this.PropertySets.FilterRow(unfilteredRow, ForwardOnlyFilteredReader.PropertySet.ForFilter)))
			{
				return null;
			}
			return new object[][]
			{
				unfilteredRow
			};
		}

		protected bool InternalGetNextAsView(IList<object[]> results, ForwardOnlyFilteredReader.StatefulRowPredicate fetchWhile)
		{
			bool flag = fetchWhile(null);
			if (!flag)
			{
				return false;
			}
			object[][] rows = this.Query.GetRows(int.MaxValue);
			int num = 0;
			if (rows.Length == 0)
			{
				return false;
			}
			foreach (object[] filteredRow in rows)
			{
				object[] unfilteredRow = this.queryPropertySets.RemitFilteredOffProperties(filteredRow);
				num++;
				foreach (object[] unfilteredRow2 in this.TransformRow(unfilteredRow) ?? Array<object[]>.Empty)
				{
					flag = fetchWhile(this.queryPropertySets.FilterRow(unfilteredRow2, ForwardOnlyFilteredReader.PropertySet.ForFilter));
					if (!flag)
					{
						break;
					}
					results.Add(this.queryPropertySets.FilterRow(unfilteredRow2, ForwardOnlyFilteredReader.PropertySet.Requested));
				}
			}
			this.Query.SeekToOffset(SeekReference.OriginCurrent, num - rows.Length);
			return flag;
		}

		protected abstract QueryResult MakeQuery(params PropertyDefinition[] propertiesToReturn);

		private void PopulateRows(IList<object[]> rows)
		{
			PropertyDefinition[] set = this.allPropertySets.GetSet(ForwardOnlyFilteredReader.PropertySet.Identification);
			using (QueryResult queryResult = this.MakeQuery(this.populationPropertySets.GetFilteredMergedSet()))
			{
				for (int i = 0; i < rows.Count; i++)
				{
					ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "PopulateRows: old row index is {0}", queryResult.CurrentRow);
					queryResult.SeekToOffset(SeekReference.OriginBeginning, 0);
					queryResult.SeekToCondition(SeekReference.OriginCurrent, new ComparisonFilter(ComparisonOperator.Equal, set[0], this.populationPropertySets.GetProperties(rows[i], new PropertyDefinition[]
					{
						set[0]
					})[0]));
					object[][] rows2 = queryResult.GetRows(1);
					if (rows2.Length == 0)
					{
						throw new ObjectNotFoundException(ServerStrings.ExItemDeletedInRace);
					}
					rows[i] = this.populationPropertySets.FilterRow(this.populationPropertySets.RemitFilteredOffProperties(rows2[0]), ForwardOnlyFilteredReader.PropertySet.Requested);
					ExTraceGlobals.StorageTracer.Information<int>((long)this.GetHashCode(), "PopulateRows: new row index is {0}", queryResult.CurrentRow);
				}
			}
		}

		protected virtual bool ShouldIntercept(PropertyDefinition property)
		{
			return property is SimpleVirtualPropertyDefinition;
		}

		private readonly ForwardOnlyFilteredReader.PropertySetMixer allPropertySets;

		private LazilyInitialized<QueryResult> query;

		private readonly ForwardOnlyFilteredReader.PropertySetMixer queryPropertySets;

		private readonly ForwardOnlyFilteredReader.PropertySetMixer populationPropertySets;

		private readonly bool usePrefetchMode;

		private readonly DisposeTracker disposeTracker;

		protected enum PropertySet
		{
			Identification,
			ForFilter,
			Requested
		}

		protected sealed class PropertySetMixer : PropertySetMixer<PropertyDefinition, ForwardOnlyFilteredReader.PropertySet>
		{
			public PropertySetMixer()
			{
			}

			public PropertySetMixer(Predicate<PropertyDefinition> shouldIntercept) : base(shouldIntercept)
			{
			}

			public void DeleteProperty(object[] unfilteredRow, PropertyDefinition propertyDefinition)
			{
				base.SetProperty(unfilteredRow, propertyDefinition, new PropertyError(propertyDefinition, PropertyErrorCode.NotFound));
			}
		}

		internal delegate bool StatefulRowPredicate(object[] transformedForFilterRow);
	}
}
