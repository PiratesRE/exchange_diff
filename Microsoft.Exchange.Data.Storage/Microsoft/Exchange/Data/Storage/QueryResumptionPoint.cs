using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class QueryResumptionPoint<TSortKey> where TSortKey : IEquatable<TSortKey>
	{
		protected QueryResumptionPoint(byte[] instanceKey, PropertyDefinition sortKey, TSortKey sortKeyValue, bool hasSortKeyValue)
		{
			Util.ThrowOnNullArgument(sortKey, "sortKey");
			this.instanceKey = instanceKey;
			this.sortKey = sortKey;
			this.sortKeyValue = sortKeyValue;
			this.hasSortKeyValue = hasSortKeyValue;
		}

		public string Version
		{
			get
			{
				return QueryResumptionPoint<TSortKey>.GetVersion(this.MinorVersion);
			}
		}

		public virtual bool IsEmpty
		{
			get
			{
				return this.instanceKey == null;
			}
		}

		public byte[] InstanceKey
		{
			get
			{
				return this.instanceKey;
			}
		}

		public TSortKey SortKeyValue
		{
			get
			{
				return this.sortKeyValue;
			}
		}

		public bool TryResume(QueryResult result, int sortKeyIndex, SeekReference reference, int rowCountToFetch, out object[][] rows)
		{
			Util.ThrowOnNullArgument(result, "result");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(sortKeyIndex, 0, "sortKeyIndex");
			EnumValidator.ThrowIfInvalid<SeekReference>(reference, "reference");
			if (rowCountToFetch <= 0)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<int>((long)this.GetHashCode(), "QueryResumptionPoint::TryResume. Unable to seek to the resumption location. (rowCountToFetch: {0})", rowCountToFetch);
				rows = null;
				return false;
			}
			rows = null;
			bool flag = false;
			if (this.instanceKey != null)
			{
				SeekReference seekReference = reference & SeekReference.SeekBackward;
				ComparisonFilter seekFilter = null;
				if (this.hasSortKeyValue)
				{
					ComparisonOperator comparisonOperator = (seekReference == SeekReference.SeekBackward) ? ComparisonOperator.LessThanOrEqual : ComparisonOperator.GreaterThanOrEqual;
					seekFilter = new ComparisonFilter(comparisonOperator, this.sortKey, this.sortKeyValue);
				}
				if (result.SeekToCondition(reference, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InstanceKey, this.instanceKey)))
				{
					ExTraceGlobals.StorageTracer.TraceDebug<byte[]>((long)this.GetHashCode(), "QueryResumptionPoint::TryResume. Successfully seeked to the resumption location (InstanceKey: {0}).", this.instanceKey);
					rows = result.GetRows(rowCountToFetch);
					object obj = rows[0][sortKeyIndex];
					if (!this.hasSortKeyValue)
					{
						if (obj is PropertyError)
						{
							flag = true;
						}
					}
					else if (!(obj is TSortKey))
					{
						ExTraceGlobals.StorageTracer.TraceDebug<string, TSortKey>((long)this.GetHashCode(), "QueryResumptionPoint::TryResume. Unable to seek to the resumption location. Will restart from beginning. {0}: ({0})", this.sortKey.Name, this.sortKeyValue);
					}
					else
					{
						TSortKey tsortKey = (TSortKey)((object)obj);
						if (tsortKey.Equals(this.sortKeyValue))
						{
							flag = true;
						}
						else
						{
							ExTraceGlobals.StorageTracer.TraceDebug<string, TSortKey>((long)this.GetHashCode(), "QueryResumptionPoint::TryResume. Unable to seek to the resumption location. Will restart from beginning. {0}: ({0})", this.sortKey.Name, this.sortKeyValue);
						}
					}
				}
				else if (this.hasSortKeyValue)
				{
					if (result.SeekToCondition(reference, seekFilter))
					{
						rows = result.GetRows(rowCountToFetch);
						flag = true;
					}
					else
					{
						ExTraceGlobals.StorageTracer.TraceDebug<string, TSortKey>((long)this.GetHashCode(), "QueryResumptionPoint::TryResume. Unable to seek to the resumption location. Will restart from beginning. {0}: ({0})", this.sortKey.Name, this.sortKeyValue);
					}
				}
				else
				{
					ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "QueryResumptionPoint::TryResume. No sort key specified. Will restart from beginning.");
				}
			}
			if (flag)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string, TSortKey>((long)this.GetHashCode(), "QueryResumptionPoint::TryResume. Successfully seeked to the resumption location ({0}: {1}).", this.sortKey.Name, this.sortKeyValue);
			}
			else
			{
				rows = null;
			}
			return flag;
		}

		protected abstract string MinorVersion { get; }

		protected static string GetVersion(string minor)
		{
			return string.Format("{0}.{1}", "1", minor);
		}

		private const string CurrentMajorVersion = "1";

		private readonly PropertyDefinition sortKey;

		private readonly byte[] instanceKey;

		private readonly TSortKey sortKeyValue;

		private readonly bool hasSortKeyValue;
	}
}
