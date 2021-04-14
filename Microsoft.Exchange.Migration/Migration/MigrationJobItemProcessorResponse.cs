using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationJobItemProcessorResponse : MigrationProcessorResponse
	{
		protected MigrationJobItemProcessorResponse(MigrationProcessorResult result, LocalizedException error = null, MailboxData mailboxData = null, IStepSnapshot stepSnapshot = null, IStepSettings stepSettings = null, bool updated = false, MigrationCountCache.MigrationStatusChange statusChange = null) : base(result, error)
		{
			this.MailboxData = mailboxData;
			this.Snapshot = stepSnapshot;
			this.Settings = stepSettings;
			this.Updated = updated;
			this.StatusChange = statusChange;
		}

		public MailboxData MailboxData { get; private set; }

		public IStepSnapshot Snapshot { get; private set; }

		public IStepSettings Settings { get; private set; }

		public bool Updated { get; private set; }

		public MigrationCountCache.MigrationStatusChange StatusChange { get; internal set; }

		public static MigrationJobItemProcessorResponse Create(MigrationProcessorResult result, TimeSpan? delayTime = null, LocalizedException error = null, MailboxData mailboxData = null, IStepSnapshot stepSnapshot = null, IStepSettings stepSettings = null, bool updated = false, MigrationCountCache.MigrationStatusChange statusChange = null)
		{
			MigrationJobItemProcessorResponse migrationJobItemProcessorResponse = new MigrationJobItemProcessorResponse(result, error, mailboxData, stepSnapshot, stepSettings, updated, statusChange);
			if (delayTime != null)
			{
				migrationJobItemProcessorResponse.DelayTime = delayTime;
			}
			return migrationJobItemProcessorResponse;
		}
	}
}
