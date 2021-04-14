using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ServicesFolderSyncState : ServicesSyncStateBase, IFolderSyncState, ISyncState
	{
		public ServicesFolderSyncState(StoreObjectId folderId, ISyncProvider syncProvider, string base64SyncData) : base(folderId, syncProvider)
		{
			ExTraceGlobals.SynchronizationTracer.TraceDebug((long)this.GetHashCode(), "ServicesFolderSyncState constructor called");
			base.Load(base64SyncData);
			if (base.Version < 3)
			{
				ExTraceGlobals.SynchronizationTracer.TraceDebug<int, int>((long)this.GetHashCode(), "The sync state version '{0}' is less than the current version '{1}', hence upgrading it.", base.Version, 3);
				RequestDetailsLogger.Current.AppendGenericInfo("SSUpgrade_Version", string.Format("V{0}-V{1}", base.Version, 3));
				this.UpgradeSyncStateTable();
			}
			if (this.Watermark == null)
			{
				ExTraceGlobals.SynchronizationTracer.TraceDebug((long)this.GetHashCode(), "Syncstate watermark was null.  Creating a new one.");
				RequestDetailsLogger.Current.AppendGenericInfo("SSUpgrade_Watermark", "Null");
				this.Watermark = syncProvider.CreateNewWatermark();
			}
		}

		protected override void InitializeSyncState(Dictionary<string, DerivedData<ICustomSerializableBuilder>> obj)
		{
			base.InitializeSyncState(obj);
			obj.Add("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.OldestReceivedTime", new DerivedData<ICustomSerializableBuilder>(new DateTimeData(this.OldestReceivedTime)));
			obj.Add("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.LastInstanceKey", new DerivedData<ICustomSerializableBuilder>(new ByteArrayData(this.LastInstanceKey)));
			obj.Add("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.MoreItemsOnServer", new DerivedData<ICustomSerializableBuilder>(new BooleanData(this.MoreItemsOnServer)));
		}

		protected override void VerifySyncState(Dictionary<string, DerivedData<ICustomSerializableBuilder>> obj)
		{
			base.VerifySyncState(obj);
			if (base.Version >= 3)
			{
				DateTimeData dateTimeData = obj["{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.OldestReceivedTime"].Data as DateTimeData;
				ByteArrayData byteArrayData = obj["{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.LastInstanceKey"].Data as ByteArrayData;
				BooleanData booleanData = obj["{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.MoreItemsOnServer"].Data as BooleanData;
				if (dateTimeData == null || byteArrayData == null || booleanData == null)
				{
					RequestDetailsLogger.Current.AppendGenericError("VerifySyncState", "NullValuesInDict");
					throw new InvalidSyncStateDataException();
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
		}

		public ISyncWatermark Watermark
		{
			get
			{
				return (ISyncWatermark)base["{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateWatermark"];
			}
			set
			{
				if (this.Watermark != null)
				{
					base.Remove("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateWatermark");
				}
				base["{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateWatermark"] = value;
			}
		}

		public ExDateTime OldestReceivedTime
		{
			get
			{
				return this.SafeGet<ExDateTime, DateTimeData>("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.OldestReceivedTime", ExDateTime.UtcNow);
			}
			set
			{
				this.SafeSet<ExDateTime, DateTimeData>("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.OldestReceivedTime", new DateTimeData(value));
			}
		}

		public byte[] LastInstanceKey
		{
			get
			{
				return this.SafeGet<byte[], ByteArrayData>("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.LastInstanceKey", null);
			}
			set
			{
				this.SafeSet<byte[], ByteArrayData>("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.LastInstanceKey", new ByteArrayData(value));
			}
		}

		public bool MoreItemsOnServer
		{
			get
			{
				return this.SafeGet<bool, BooleanData>("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.MoreItemsOnServer", true);
			}
			set
			{
				this.SafeSet<bool, BooleanData>("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.MoreItemsOnServer", new BooleanData(value));
			}
		}

		internal override StringData SyncStateTag
		{
			get
			{
				return ServicesFolderSyncState.FolderItemsSyncTagValue;
			}
		}

		internal ReturnType SafeGet<ReturnType, DataType>(string keyName, ReturnType defaultValue) where DataType : ComponentData<ReturnType>
		{
			if (!base.Contains(keyName))
			{
				return defaultValue;
			}
			DataType dataType = (DataType)((object)base[keyName]);
			return dataType.Data;
		}

		internal void SafeSet<DataType, WrapperType>(string keyName, WrapperType wrapper) where WrapperType : ComponentData<DataType>
		{
			if (base.Contains(keyName))
			{
				base.Remove(keyName);
			}
			base[keyName] = wrapper;
		}

		private void UpgradeSyncStateTable()
		{
			ISyncWatermark syncWatermark = base.Contains(SyncStateProp.CurMaxWatermark) ? ((ISyncWatermark)base[SyncStateProp.CurMaxWatermark]) : null;
			if (syncWatermark == null)
			{
				syncWatermark = (base.Contains("{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateWatermark") ? ((ISyncWatermark)base["{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateWatermark"]) : null);
			}
			base.Version = 3;
			base.InitializeSyncStateTable();
			this.Watermark = syncWatermark;
		}

		private const string SyncStateWatermarkKeyName = "{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.SyncStateWatermark";

		private const string SyncStateOldestReceivedTimeKeyName = "{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.OldestReceivedTime";

		private const string SyncStateLastInstanceKeyKeyName = "{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.LastInstanceKey";

		private const string SyncStateMoreItemsOnServerKeyName = "{49A4350A-C7A8-4AC3-BCBC-F2B8CB7F9550}WS.MoreItemsOnServer";

		public const int E15RtmFolderSyncStateVersion = 3;

		public const int E14RtmFolderSyncStateVersion = 2;

		public const int E12RtmFolderSyncStateVersion = 1;

		public const int CurrentFolderSyncStateVersion = 3;

		internal static StringData FolderItemsSyncTagValue = new StringData("WS.FolderItemsSync");
	}
}
