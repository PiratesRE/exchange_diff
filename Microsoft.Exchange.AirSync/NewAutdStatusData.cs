using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class NewAutdStatusData : IAutdStatusData
	{
		public int? LastPingHeartbeat
		{
			get
			{
				return this.lastPingHeartbeat;
			}
			set
			{
				this.lastPingHeartbeat = value;
				this.globalStateModified = true;
			}
		}

		public Dictionary<string, PingCommand.DPFolderInfo> DPFolderList
		{
			get
			{
				return this.dpFolderList;
			}
			set
			{
				this.dpFolderList = value;
				this.deviceMetadataStateChanged = true;
			}
		}

		public static IAutdStatusData Load(GlobalInfo globalInfo, SyncStateStorage syncStateStorage)
		{
			NewAutdStatusData newAutdStatusData = new NewAutdStatusData();
			newAutdStatusData.globalInfo = globalInfo;
			newAutdStatusData.deviceMetadata = syncStateStorage.DeviceMetadata;
			if (!globalInfo.HasNewAutdData)
			{
				using (AutdStatusData autdStatusData = AutdStatusData.Load(syncStateStorage, false, false))
				{
					if (autdStatusData != null)
					{
						newAutdStatusData.lastPingHeartbeat = autdStatusData.LastPingHeartbeat;
						newAutdStatusData.DPFolderList = ((autdStatusData.DPFolderList == null) ? null : new Dictionary<string, PingCommand.DPFolderInfo>(autdStatusData.DPFolderList));
					}
				}
				newAutdStatusData.deviceMetadataStateChanged = true;
				newAutdStatusData.globalStateModified = true;
			}
			else
			{
				newAutdStatusData.lastPingHeartbeat = globalInfo.LastPingHeartbeat;
				if (newAutdStatusData.deviceMetadata.PingFolderList != null)
				{
					newAutdStatusData.DPFolderList = ((newAutdStatusData.deviceMetadata.PingFolderList == null) ? null : new Dictionary<string, PingCommand.DPFolderInfo>((Dictionary<string, PingCommand.DPFolderInfo>)newAutdStatusData.deviceMetadata.PingFolderList));
				}
				newAutdStatusData.deviceMetadataStateChanged = false;
				newAutdStatusData.globalStateModified = false;
			}
			return newAutdStatusData;
		}

		public void SaveAndDispose()
		{
			if (this.globalStateModified)
			{
				this.globalInfo.LastPingHeartbeat = this.lastPingHeartbeat;
			}
			if (this.deviceMetadataStateChanged)
			{
				this.deviceMetadata.PingFolderList = ((this.DPFolderList == null) ? null : new Dictionary<string, PingCommand.DPFolderInfo>(this.DPFolderList));
			}
			this.deviceMetadataStateChanged = false;
			this.globalStateModified = false;
		}

		private bool globalStateModified;

		private bool deviceMetadataStateChanged;

		private int? lastPingHeartbeat;

		private Dictionary<string, PingCommand.DPFolderInfo> dpFolderList;

		private GlobalInfo globalInfo;

		private DeviceSyncStateMetadata deviceMetadata;
	}
}
