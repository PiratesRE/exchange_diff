using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class FolderIdMapping : IdMapping
	{
		public override ushort TypeId
		{
			get
			{
				return FolderIdMapping.typeId;
			}
			set
			{
				FolderIdMapping.typeId = value;
			}
		}

		public override string this[ISyncItemId mailboxId]
		{
			get
			{
				string text = base[mailboxId];
				if (text != null && !base.IsInDeletedItemsBuffer(text))
				{
					return text;
				}
				return null;
			}
		}

		public override ISyncItemId this[string syncId]
		{
			get
			{
				if (base.IsInDeletedItemsBuffer(syncId))
				{
					return null;
				}
				return base[syncId];
			}
		}

		public string Add(ISyncItemId mailboxFolderId)
		{
			AirSyncDiagnostics.Assert(mailboxFolderId != null);
			string text;
			if (base.OldIds.ContainsKey(mailboxFolderId))
			{
				text = base.OldIds[mailboxFolderId];
			}
			else
			{
				text = base.UniqueCounter.ToString(CultureInfo.InvariantCulture);
			}
			base.Add(mailboxFolderId, text);
			return text;
		}

		public new void Add(ISyncItemId mailboxFolderId, string syncId)
		{
			AirSyncDiagnostics.Assert(mailboxFolderId != null);
			base.Add(mailboxFolderId, syncId);
		}

		public override bool Contains(ISyncItemId mailboxId)
		{
			string text = base[mailboxId];
			return text != null && !base.IsInDeletedItemsBuffer(text);
		}

		public override bool Contains(string syncId)
		{
			return base.Contains(syncId) && !base.IsInDeletedItemsBuffer(syncId);
		}

		public override ICustomSerializable BuildObject()
		{
			return new FolderIdMapping();
		}

		private static ushort typeId;
	}
}
