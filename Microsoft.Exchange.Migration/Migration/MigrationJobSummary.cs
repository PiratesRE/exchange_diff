using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationJobSummary
	{
		public string BatchName { get; set; }

		public Guid BatchGuid { get; set; }

		public ExTimeZone UserTimeZone { get; set; }

		public MigrationBatchId BatchId
		{
			get
			{
				return new MigrationBatchId(this.BatchName, this.BatchGuid);
			}
		}

		public static MigrationJobSummary LoadFromRow(object[] propertyValues)
		{
			if (propertyValues == null)
			{
				return null;
			}
			MigrationJobSummary migrationJobSummary = new MigrationJobSummary();
			for (int i = 0; i < MigrationJobSummary.PropertyDefinitions.Length; i++)
			{
				if (propertyValues[i] != null && !(propertyValues[i] is PropertyError))
				{
					if (MigrationJobSummary.PropertyDefinitions[i] == StoreObjectSchema.ItemClass && !string.Equals((string)propertyValues[i], MigrationBatchMessageSchema.MigrationJobClass))
					{
						return null;
					}
					if (MigrationJobSummary.PropertyDefinitions[i] == MigrationBatchMessageSchema.MigrationJobName)
					{
						migrationJobSummary.BatchName = (string)propertyValues[i];
					}
					else if (MigrationJobSummary.PropertyDefinitions[i] == MigrationBatchMessageSchema.MigrationJobId)
					{
						migrationJobSummary.BatchGuid = (Guid)propertyValues[i];
					}
					else if (MigrationJobSummary.PropertyDefinitions[i] == MigrationBatchMessageSchema.MigrationJobUserTimeZone)
					{
						Exception ex = null;
						migrationJobSummary.UserTimeZone = MigrationHelper.GetExTimeZoneValue(propertyValues[i], ref ex);
					}
				}
			}
			return migrationJobSummary;
		}

		internal static MigrationJobSummary CreateFromJob(MigrationJob job)
		{
			return new MigrationJobSummary
			{
				BatchGuid = job.JobId,
				BatchName = job.JobName,
				UserTimeZone = job.UserTimeZone
			};
		}

		internal static readonly PropertyDefinition[] PropertyDefinitions = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			MigrationBatchMessageSchema.MigrationJobName,
			MigrationBatchMessageSchema.MigrationJobId,
			MigrationBatchMessageSchema.MigrationJobUserTimeZone
		};
	}
}
