using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class StringWatermark : BaseWatermark
	{
		public StringWatermark(SyncLogSession syncLogSession, string mailboxServerSyncWatermark) : base(syncLogSession, mailboxServerSyncWatermark, null, true)
		{
		}

		public StringWatermark(SyncLogSession syncLogSession, ISimpleStateStorage stateStorage) : base(syncLogSession, null, stateStorage, false)
		{
		}

		public virtual void Load(out string watermark)
		{
			if (base.LoadedFromMailboxServer)
			{
				watermark = base.MailboxServerSyncWatermark;
				return;
			}
			if (!base.StateStorage.TryGetPropertyValue("Watermark", out watermark))
			{
				watermark = StringWatermark.DefaultWatermark;
			}
			base.StateStorageEncodedSyncWatermark = watermark;
		}

		public virtual void Save(string watermark)
		{
			SyncUtilities.ThrowIfArgumentNull("watermark", watermark);
			if (base.StateStorage.ContainsProperty("Watermark"))
			{
				base.StateStorage.ChangePropertyValue("Watermark", watermark);
			}
			else
			{
				base.StateStorage.AddProperty("Watermark", watermark);
			}
			base.StateStorageEncodedSyncWatermark = watermark;
		}

		private const string WatermarkPropertyName = "Watermark";

		private static readonly string DefaultWatermark = string.Empty;
	}
}
