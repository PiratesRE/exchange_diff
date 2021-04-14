using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationJobProcessorResponse : MigrationProcessorResponse
	{
		private MigrationJobProcessorResponse(MigrationProcessorResult result, LocalizedException error = null, string lastProcessedRow = null, string batchInputId = null, MigrationCountCache.MigrationStatusChange childStatusChanges = null) : base(result, error)
		{
			this.LastProcessedRow = lastProcessedRow;
			this.ChildStatusChanges = (childStatusChanges ?? MigrationCountCache.MigrationStatusChange.None);
			this.BatchInputId = batchInputId;
		}

		public string LastProcessedRow { get; set; }

		public string BatchInputId { get; private set; }

		public MigrationCountCache.MigrationStatusChange ChildStatusChanges { get; private set; }

		internal static MigrationJobProcessorResponse Create(MigrationProcessorResult result, TimeSpan? delayTime = null, LocalizedException error = null, string lastProcessedRow = null, string batchInputId = null, MigrationCountCache.MigrationStatusChange childStatusChanges = null)
		{
			MigrationJobProcessorResponse migrationJobProcessorResponse = new MigrationJobProcessorResponse(result, error, lastProcessedRow, batchInputId, childStatusChanges);
			if (delayTime != null)
			{
				migrationJobProcessorResponse.DelayTime = delayTime;
			}
			return migrationJobProcessorResponse;
		}

		public override void Aggregate(MigrationProcessorResponse childResponse)
		{
			MigrationJobItemProcessorResponse migrationJobItemProcessorResponse = childResponse as MigrationJobItemProcessorResponse;
			if (migrationJobItemProcessorResponse != null && migrationJobItemProcessorResponse.StatusChange != null)
			{
				this.ChildStatusChanges += migrationJobItemProcessorResponse.StatusChange;
			}
			base.Aggregate(childResponse);
		}
	}
}
