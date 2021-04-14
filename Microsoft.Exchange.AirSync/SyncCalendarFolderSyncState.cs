using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class SyncCalendarFolderSyncState : SyncCalendarSyncStateBase, IFolderSyncState, ISyncState
	{
		public SyncCalendarFolderSyncState(StoreObjectId folderId, ISyncProvider syncProvider, string base64SyncData) : base(folderId, syncProvider)
		{
			ExTraceGlobals.MethodEnterExitTracer.TraceDebug((long)this.GetHashCode(), "SyncCalendarFolderSyncState constructor called");
			base.Load(base64SyncData);
			if (base.Version < 1)
			{
				this.UpgradeSyncStateTable();
			}
			if (this.Watermark == null)
			{
				this.Watermark = syncProvider.CreateNewWatermark();
			}
		}

		public ISyncWatermark Watermark
		{
			get
			{
				return (ISyncWatermark)base["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateWatermark"];
			}
			set
			{
				if (this.Watermark != null)
				{
					base.Remove("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateWatermark");
				}
				base["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateWatermark"] = value;
			}
		}

		public ExDateTime OldestReceivedTime
		{
			get
			{
				if (base.Contains("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.OldestReceivedTime"))
				{
					return ((DateTimeData)base["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.OldestReceivedTime"]).Data;
				}
				return ExDateTime.UtcNow;
			}
			set
			{
				if (base.Contains("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.OldestReceivedTime"))
				{
					base.Remove("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.OldestReceivedTime");
				}
				base["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.OldestReceivedTime"] = new DateTimeData(value);
			}
		}

		public byte[] LastInstanceKey
		{
			get
			{
				if (base.Contains("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.LastInstanceKey"))
				{
					return ((ByteArrayData)base["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.LastInstanceKey"]).Data;
				}
				return null;
			}
			set
			{
				if (base.Contains("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.LastInstanceKey"))
				{
					base.Remove("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.LastInstanceKey");
				}
				base["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.LastInstanceKey"] = new ByteArrayData(value);
			}
		}

		public bool MoreItemsOnServer
		{
			get
			{
				return !base.Contains("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.MoreItemsOnServer") || ((BooleanData)base["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.MoreItemsOnServer"]).Data;
			}
			set
			{
				if (base.Contains("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.MoreItemsOnServer"))
				{
					base.Remove("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.MoreItemsOnServer");
				}
				base["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.MoreItemsOnServer"] = new BooleanData(value);
			}
		}

		internal override StringData SyncStateTag
		{
			get
			{
				return SyncCalendarFolderSyncState.FolderItemsSyncTagValue;
			}
		}

		protected override void InitializeSyncState(Dictionary<string, DerivedData<ICustomSerializableBuilder>> obj)
		{
			base.InitializeSyncState(obj);
			obj.Add("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.OldestReceivedTime", new DerivedData<ICustomSerializableBuilder>(new DateTimeData(this.OldestReceivedTime)));
			obj.Add("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.LastInstanceKey", new DerivedData<ICustomSerializableBuilder>(new ByteArrayData(this.LastInstanceKey)));
			obj.Add("{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.MoreItemsOnServer", new DerivedData<ICustomSerializableBuilder>(new BooleanData(this.MoreItemsOnServer)));
		}

		protected override void VerifySyncState(Dictionary<string, DerivedData<ICustomSerializableBuilder>> obj)
		{
			base.VerifySyncState(obj);
			if (base.Version >= 1)
			{
				DateTimeData dateTimeData = obj["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.OldestReceivedTime"].Data as DateTimeData;
				ByteArrayData byteArrayData = obj["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.LastInstanceKey"].Data as ByteArrayData;
				BooleanData booleanData = obj["{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.MoreItemsOnServer"].Data as BooleanData;
				if (dateTimeData == null || byteArrayData == null || booleanData == null)
				{
					throw new CorruptSyncStateException("Empty mandatory key", null);
				}
			}
		}

		public FolderSync GetFolderSync()
		{
			return this.GetFolderSync(ConflictResolutionPolicy.ServerWins);
		}

		public FolderSync GetFolderSync(ConflictResolutionPolicy policy)
		{
			return new FolderSync(this.syncProvider, this, policy, false);
		}

		public void OnCommitStateModifications(FolderSyncStateUtil.CommitStateModificationsDelegate commitStateModificationsDelegate)
		{
			throw new NotImplementedException();
		}

		private void UpgradeSyncStateTable()
		{
			ExTraceGlobals.MethodEnterExitTracer.TraceDebug((long)this.GetHashCode(), "SyncCalendarFolderSyncState.UpgradeSyncStateTable called");
			ISyncWatermark watermark = base.Contains(SyncStateProp.CurMaxWatermark) ? ((ISyncWatermark)base[SyncStateProp.CurMaxWatermark]) : null;
			base.Version = 1;
			base.InitializeSyncStateTable();
			this.Watermark = watermark;
		}

		private const string SyncStateWatermarkKeyName = "{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.SyncStateWatermark";

		private const string SyncStateOldestReceivedTimeKeyName = "{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.OldestReceivedTime";

		private const string SyncStateLastInstanceKeyKeyName = "{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.LastInstanceKey";

		private const string SyncStateMoreItemsOnServerKeyName = "{9150227d-9140-45d0-b4c2-e987f59cfc46}SyncCalendar.MoreItemsOnServer";

		public const int E15RtmFolderSyncStateVersion = 1;

		public const int CurrentFolderSyncStateVersion = 1;

		internal static StringData FolderItemsSyncTagValue = new StringData("SyncCalendar.FolderItemsSync");
	}
}
