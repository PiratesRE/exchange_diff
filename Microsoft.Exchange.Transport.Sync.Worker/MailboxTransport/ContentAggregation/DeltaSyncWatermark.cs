using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DeltaSyncWatermark : BaseWatermark
	{
		public DeltaSyncWatermark(SyncLogSession syncLogSession, string mailboxServerSyncWatermark) : base(syncLogSession, mailboxServerSyncWatermark, null, true)
		{
		}

		public DeltaSyncWatermark(SyncLogSession syncLogSession, ISimpleStateStorage stateStorage) : base(syncLogSession, null, stateStorage, false)
		{
		}

		public void Load(out string folderSyncKey, out string emailSyncKey)
		{
			if (base.LoadedFromMailboxServer)
			{
				this.encoder.Decode(base.MailboxServerSyncWatermark, base.SyncLogSession, out folderSyncKey, out emailSyncKey);
				return;
			}
			if (!base.StateStorage.TryGetPropertyValue("FolderSyncKey", out folderSyncKey))
			{
				folderSyncKey = DeltaSyncCommon.DefaultSyncKey;
			}
			if (!base.StateStorage.TryGetPropertyValue("EmailSyncKey", out emailSyncKey))
			{
				emailSyncKey = DeltaSyncCommon.DefaultSyncKey;
			}
			base.StateStorageEncodedSyncWatermark = this.encoder.Encode(folderSyncKey, emailSyncKey);
		}

		public void Save(string folderSyncKey, string emailSyncKey)
		{
			SyncUtilities.ThrowIfArgumentNull("folderSyncKey", folderSyncKey);
			SyncUtilities.ThrowIfArgumentNull("emailSyncKey", emailSyncKey);
			if (base.LoadedFromMailboxServer)
			{
				base.MailboxServerSyncWatermark = this.encoder.Encode(folderSyncKey, emailSyncKey);
				base.SyncLogSession.LogVerbose((TSLID)1243UL, "Saved Watermark that was loaded from Mailbox Server: {0}", new object[]
				{
					base.MailboxServerSyncWatermark
				});
				return;
			}
			if (base.StateStorage.ContainsProperty("FolderSyncKey"))
			{
				base.StateStorage.ChangePropertyValue("FolderSyncKey", folderSyncKey);
			}
			else
			{
				base.StateStorage.AddProperty("FolderSyncKey", folderSyncKey);
			}
			if (base.StateStorage.ContainsProperty("EmailSyncKey"))
			{
				base.StateStorage.ChangePropertyValue("EmailSyncKey", emailSyncKey);
			}
			else
			{
				base.StateStorage.AddProperty("EmailSyncKey", emailSyncKey);
			}
			base.StateStorageEncodedSyncWatermark = this.encoder.Encode(folderSyncKey, emailSyncKey);
		}

		private const string FolderSyncKeyPropertyName = "FolderSyncKey";

		private const string EmailSyncKeyPropertyName = "EmailSyncKey";

		private readonly DeltaSyncSyncWatermarkEncoder encoder = new DeltaSyncSyncWatermarkEncoder();
	}
}
