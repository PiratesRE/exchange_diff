using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LegacyMigrationJobProcessorResponse : MigrationProcessorResponse
	{
		public LegacyMigrationJobProcessorResponse()
		{
			this.NumItemsProcessed = null;
			this.NumItemsOutstanding = null;
			this.NumItemsTransitioned = null;
		}

		private LegacyMigrationJobProcessorResponse(MigrationProcessorResult result) : base(result, null)
		{
			this.NumItemsProcessed = null;
			this.NumItemsOutstanding = null;
			this.NumItemsTransitioned = null;
		}

		public int? NumItemsProcessed { get; set; }

		public int? NumItemsOutstanding { get; set; }

		public int? NumItemsTransitioned { get; set; }

		public MigrationJobStatus? NextStatus { get; set; }

		internal static LegacyMigrationJobProcessorResponse Create(MigrationProcessorResult result, TimeSpan? delayTime = null)
		{
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = new LegacyMigrationJobProcessorResponse(result);
			if (delayTime != null)
			{
				legacyMigrationJobProcessorResponse.DelayTime = delayTime;
			}
			return legacyMigrationJobProcessorResponse;
		}

		protected override void Merge(MigrationProcessorResponse left, MigrationProcessorResponse right)
		{
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = left as LegacyMigrationJobProcessorResponse;
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse2 = right as LegacyMigrationJobProcessorResponse;
			foreach (LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse3 in new LegacyMigrationJobProcessorResponse[]
			{
				legacyMigrationJobProcessorResponse,
				legacyMigrationJobProcessorResponse2
			})
			{
				if (legacyMigrationJobProcessorResponse3 != null)
				{
					if (legacyMigrationJobProcessorResponse3.NumItemsOutstanding != null)
					{
						this.NumItemsOutstanding = (this.NumItemsOutstanding ?? 0) + legacyMigrationJobProcessorResponse3.NumItemsOutstanding;
					}
					if (legacyMigrationJobProcessorResponse3.NumItemsProcessed != null)
					{
						this.NumItemsProcessed = (this.NumItemsOutstanding ?? 0) + legacyMigrationJobProcessorResponse3.NumItemsOutstanding;
					}
					if (legacyMigrationJobProcessorResponse3.NumItemsTransitioned != null)
					{
						this.NumItemsTransitioned = (this.NumItemsOutstanding ?? 0) + legacyMigrationJobProcessorResponse3.NumItemsTransitioned;
					}
				}
			}
			base.Merge(left, right);
		}
	}
}
