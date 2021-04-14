using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol.EDiscovery
{
	internal class SearchResult : ResultBase
	{
		static SearchResult()
		{
			SearchResult.description.ComplianceStructureId = 99;
			SearchResult.description.RegisterIntegerPropertyGetterAndSetter(0, (SearchResult item) => item.PageSize, delegate(SearchResult item, int value)
			{
				item.PageSize = value;
			});
			SearchResult.description.RegisterLongPropertyGetterAndSetter(0, (SearchResult item) => item.TotalCount, delegate(SearchResult item, long value)
			{
				item.totalCount = value;
			});
			SearchResult.description.RegisterLongPropertyGetterAndSetter(1, (SearchResult item) => item.TotalSize, delegate(SearchResult item, long value)
			{
				item.totalSize = value;
			});
			SearchResult.description.RegisterComplexCollectionAccessor<SearchResult.TargetSearchResult>(0, (SearchResult item) => item.Results.Count, (SearchResult item, int index) => item.Results.ToList<SearchResult.TargetSearchResult>()[index], delegate(SearchResult item, SearchResult.TargetSearchResult value, int index)
			{
				item.Results.Add(value);
			}, SearchResult.TargetSearchResult.Description);
			SearchResult.description.RegisterComplexCollectionAccessor<FaultRecord>(1, (SearchResult item) => item.Faults.Count, (SearchResult item, int index) => item.Faults.ToList<FaultRecord>()[index], delegate(SearchResult item, FaultRecord value, int index)
			{
				item.Faults.TryAdd(value);
			}, FaultRecord.Description);
		}

		public static ComplianceSerializationDescription<SearchResult> Description
		{
			get
			{
				return SearchResult.description;
			}
		}

		public override int SerializationVersion
		{
			get
			{
				return 1;
			}
		}

		public long TotalSize
		{
			get
			{
				return this.totalSize;
			}
		}

		public long TotalCount
		{
			get
			{
				return this.totalCount;
			}
		}

		public int PageSize { get; set; }

		public ConcurrentBag<SearchResult.TargetSearchResult> Results
		{
			get
			{
				return this.results;
			}
		}

		public override byte[] GetSerializedResult()
		{
			return ComplianceSerializer.Serialize<SearchResult>(SearchResult.Description, this);
		}

		public void UpdateTotalSize(long size)
		{
			long num;
			long value;
			do
			{
				num = this.totalSize;
				value = num + size;
			}
			while (num != Interlocked.CompareExchange(ref this.totalSize, value, num));
		}

		public void UpdateTotalCount(long count)
		{
			long num;
			long value;
			do
			{
				num = this.totalCount;
				value = num + count;
			}
			while (num != Interlocked.CompareExchange(ref this.totalCount, value, num));
		}

		private static ComplianceSerializationDescription<SearchResult> description = new ComplianceSerializationDescription<SearchResult>();

		private ConcurrentBag<SearchResult.TargetSearchResult> results = new ConcurrentBag<SearchResult.TargetSearchResult>();

		private long totalCount;

		private long totalSize;

		public class TargetSearchResult
		{
			static TargetSearchResult()
			{
				SearchResult.TargetSearchResult.description.ComplianceStructureId = 99;
				SearchResult.TargetSearchResult.description.RegisterLongPropertyGetterAndSetter(0, (SearchResult.TargetSearchResult item) => item.Count, delegate(SearchResult.TargetSearchResult item, long value)
				{
					item.Count = value;
				});
				SearchResult.TargetSearchResult.description.RegisterLongPropertyGetterAndSetter(1, (SearchResult.TargetSearchResult item) => item.Size, delegate(SearchResult.TargetSearchResult item, long value)
				{
					item.Size = value;
				});
				SearchResult.TargetSearchResult.description.RegisterComplexPropertyAsBlobGetterAndSetter<Target>(0, (SearchResult.TargetSearchResult item) => item.Target, delegate(SearchResult.TargetSearchResult item, Target value)
				{
					item.Target = value;
				}, Target.Description);
			}

			public static ComplianceSerializationDescription<SearchResult.TargetSearchResult> Description
			{
				get
				{
					return SearchResult.TargetSearchResult.description;
				}
			}

			public long Size { get; set; }

			public long Count { get; set; }

			public Target Target { get; set; }

			public override string ToString()
			{
				return string.Format("Binding: {0}, Item count: {1}, Total size: {2}", this.Target.Identifier, this.Count, this.Size);
			}

			private static ComplianceSerializationDescription<SearchResult.TargetSearchResult> description = new ComplianceSerializationDescription<SearchResult.TargetSearchResult>();
		}
	}
}
