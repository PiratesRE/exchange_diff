using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MigrationJobObjectCache
	{
		public MigrationJobObjectCache(IMigrationDataProvider dataProvider)
		{
			this.dataProvider = dataProvider;
		}

		public MigrationJob GetJob(Guid jobGuid)
		{
			MigrationJobSummary jobSummary = this.GetJobSummary(new Guid?(jobGuid));
			if (jobSummary == null)
			{
				return null;
			}
			return this.GetJob(jobSummary.BatchId);
		}

		public MigrationJob GetJob(MigrationBatchId batch)
		{
			if (batch == null)
			{
				return null;
			}
			MigrationJob uniqueByBatchId;
			if (!this.jobsFromBatchId.TryGetValue(batch, out uniqueByBatchId))
			{
				uniqueByBatchId = MigrationJob.GetUniqueByBatchId(this.dataProvider, batch);
				this.jobsFromBatchId[batch] = uniqueByBatchId;
			}
			return uniqueByBatchId;
		}

		public Guid GetBatchGuidById(MigrationBatchId batchId)
		{
			MigrationJobSummary migrationJobSummary = this.FindJobSummaryById(batchId);
			if (migrationJobSummary == null)
			{
				throw new MigrationBatchNotFoundException(batchId.ToString());
			}
			Guid batchGuid = migrationJobSummary.BatchGuid;
			if (!this.jobSummaryFromGuid.ContainsKey(batchGuid))
			{
				this.jobSummaryFromGuid.Add(batchGuid, migrationJobSummary);
			}
			return batchGuid;
		}

		public MigrationJobSummary GetJobSummary(Guid? guid)
		{
			if (guid == null)
			{
				return null;
			}
			Guid value = guid.Value;
			MigrationJobSummary migrationJobSummary;
			if (this.jobSummaryFromGuid.TryGetValue(value, out migrationJobSummary))
			{
				return migrationJobSummary;
			}
			if (this.unknownBatchGuids.Contains(value))
			{
				return null;
			}
			migrationJobSummary = this.FindJobSummaryByGuid(value);
			if (migrationJobSummary == null)
			{
				this.unknownBatchGuids.Add(value);
			}
			else
			{
				this.jobSummaryFromGuid.Add(value, migrationJobSummary);
			}
			return migrationJobSummary;
		}

		private MigrationJobSummary FindJobSummaryById(MigrationBatchId id)
		{
			if (id == null)
			{
				return null;
			}
			if (id.JobId != Guid.Empty)
			{
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobClass),
					new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, id.JobId)
				});
				MigrationJobSummary migrationJobSummary = this.FindJobSummaryByFilter(filter, MigrationJobObjectCache.SortByJobId);
				if (migrationJobSummary != null)
				{
					return migrationJobSummary;
				}
			}
			QueryFilter filter2 = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobClass),
				new TextFilter(MigrationBatchMessageSchema.MigrationJobName, id.Name, MatchOptions.FullString, MatchFlags.IgnoreCase)
			});
			return this.FindJobSummaryByFilter(filter2, MigrationJobObjectCache.SortByJobName);
		}

		private MigrationJobSummary FindJobSummaryByGuid(Guid guid)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationJobClass),
				new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, guid)
			});
			return this.FindJobSummaryByFilter(filter, MigrationJobObjectCache.SortByJobId);
		}

		private MigrationJobSummary FindJobSummaryByFilter(QueryFilter filter, SortBy[] sortOrder)
		{
			MigrationJobSummary migrationJobSummary = null;
			foreach (object[] propertyValues in this.dataProvider.QueryRows(filter, sortOrder, MigrationJobSummary.PropertyDefinitions, 2))
			{
				migrationJobSummary = MigrationJobSummary.LoadFromRow(propertyValues);
			}
			if (migrationJobSummary == null)
			{
				return null;
			}
			return migrationJobSummary;
		}

		internal void PreSeed(MigrationJob job)
		{
			MigrationJobSummary migrationJobSummary = MigrationJobSummary.CreateFromJob(job);
			if (!this.jobSummaryFromGuid.ContainsKey(job.JobId))
			{
				this.jobSummaryFromGuid.Add(job.JobId, migrationJobSummary);
			}
			if (!this.jobsFromBatchId.ContainsKey(migrationJobSummary.BatchId))
			{
				this.jobsFromBatchId.Add(migrationJobSummary.BatchId, job);
			}
		}

		private static readonly SortBy[] SortByJobId = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationJobId, SortOrder.Ascending)
		};

		private static readonly SortBy[] SortByJobName = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending),
			new SortBy(MigrationBatchMessageSchema.MigrationJobName, SortOrder.Ascending)
		};

		private readonly IMigrationDataProvider dataProvider;

		private readonly Dictionary<MigrationBatchId, MigrationJob> jobsFromBatchId = new Dictionary<MigrationBatchId, MigrationJob>();

		private readonly List<Guid> unknownBatchGuids = new List<Guid>();

		private readonly Dictionary<Guid, MigrationJobSummary> jobSummaryFromGuid = new Dictionary<Guid, MigrationJobSummary>();
	}
}
