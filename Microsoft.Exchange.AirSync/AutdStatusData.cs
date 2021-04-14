using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class AutdStatusData : DisposeTrackableBase, IAutdStatusData
	{
		public int? LastPingHeartbeat
		{
			get
			{
				UInt32Data uint32Data = (UInt32Data)this.autdStatusSyncState[CustomStateDatumType.LastPingHeartbeat];
				if (uint32Data != null)
				{
					return new int?((int)uint32Data.Data);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.autdStatusSyncState.Remove(CustomStateDatumType.LastPingHeartbeat);
				}
				else
				{
					this.autdStatusSyncState[CustomStateDatumType.LastPingHeartbeat] = new UInt32Data((uint)value.Value);
				}
				this.dirty = true;
			}
		}

		public Dictionary<string, PingCommand.DPFolderInfo> DPFolderList
		{
			get
			{
				GenericDictionaryData<StringData, string, PingCommand.DPFolderInfo> genericDictionaryData = (GenericDictionaryData<StringData, string, PingCommand.DPFolderInfo>)this.autdStatusSyncState[CustomStateDatumType.DPFolderList];
				if (genericDictionaryData != null)
				{
					return genericDictionaryData.Data;
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.autdStatusSyncState.Remove(CustomStateDatumType.DPFolderList);
				}
				else
				{
					this.autdStatusSyncState[CustomStateDatumType.DPFolderList] = new GenericDictionaryData<StringData, string, PingCommand.DPFolderInfo>(value);
				}
				this.dirty = true;
			}
		}

		public static AutdStatusData Load(SyncStateStorage syncStateStorage, bool readOnly, bool createIfMissing)
		{
			AutdStatusData autdStatusData = null;
			bool flag = false;
			try
			{
				autdStatusData = new AutdStatusData();
				AutdSyncStateInfo autdSyncStateInfo = new AutdSyncStateInfo();
				autdSyncStateInfo.ReadOnly = readOnly;
				autdStatusData.autdStatusSyncState = syncStateStorage.GetCustomSyncState(autdSyncStateInfo, new PropertyDefinition[0]);
				if (autdStatusData.autdStatusSyncState == null)
				{
					if (!createIfMissing)
					{
						return null;
					}
					autdStatusData.autdStatusSyncState = syncStateStorage.CreateCustomSyncState(autdSyncStateInfo);
				}
				autdStatusData.dirty = false;
				flag = true;
			}
			finally
			{
				if (!flag && autdStatusData != null)
				{
					autdStatusData.Dispose();
					autdStatusData = null;
				}
			}
			return autdStatusData;
		}

		public void SaveAndDispose()
		{
			if (this.autdStatusSyncState != null && this.dirty)
			{
				try
				{
					this.autdStatusSyncState.Commit();
					this.dirty = false;
				}
				catch (LocalizedException arg)
				{
					AirSyncDiagnostics.TraceError<LocalizedException>(ExTraceGlobals.RequestsTracer, this, "[SyncData.Save] Failed to commit autdStatusSyncState. Exception: {0}", arg);
				}
				finally
				{
					this.Dispose();
				}
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.autdStatusSyncState != null)
			{
				this.autdStatusSyncState.Dispose();
				this.autdStatusSyncState = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AutdStatusData>(this);
		}

		private bool dirty;

		private CustomSyncState autdStatusSyncState;
	}
}
