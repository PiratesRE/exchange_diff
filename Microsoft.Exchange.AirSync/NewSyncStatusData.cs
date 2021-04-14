using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class NewSyncStatusData : ISyncStatusData
	{
		public bool ClientCanSendUpEmptyRequests
		{
			get
			{
				return this.clientCanSendUpEmptyRequests;
			}
			set
			{
				this.clientCanSendUpEmptyRequests = value;
				this.deviceMetadataStateChanged = true;
			}
		}

		public string LastSyncRequestRandomString
		{
			get
			{
				return this.lastSyncRequestRandomString;
			}
			set
			{
				this.lastSyncRequestRandomString = value;
				this.deviceMetadataStateChanged = true;
			}
		}

		public byte[] LastCachableWbxmlDocument
		{
			get
			{
				return this.lastCachableWbxmlDocument;
			}
			set
			{
				this.lastCachableWbxmlDocument = value;
				this.deviceMetadataStateChanged = true;
			}
		}

		public ExDateTime? LastSyncAttemptTime
		{
			get
			{
				return this.lastSyncAttemptTime;
			}
			set
			{
				this.lastSyncAttemptTime = value;
				this.globalStateModified |= NewSyncStatusData.GlobalInfoStateModified.LastSyncAttemptTime;
			}
		}

		public ExDateTime? LastSyncSuccessTime
		{
			get
			{
				return this.lastSyncSuccessTime;
			}
			set
			{
				this.lastSyncSuccessTime = value;
				this.globalStateModified |= NewSyncStatusData.GlobalInfoStateModified.LastSyncSuccessTime;
			}
		}

		public string LastSyncUserAgent
		{
			get
			{
				return this.lastSyncUserAgent;
			}
			set
			{
				this.lastSyncUserAgent = value;
				this.globalStateModified |= NewSyncStatusData.GlobalInfoStateModified.LastSyncUserAgent;
			}
		}

		public void AddClientId(string clientId)
		{
			lock (this.instanceLock)
			{
				if (this.lastClientIdsSent == null)
				{
					this.lastClientIdsSent = new List<string>(1);
				}
				if (!this.lastClientIdsSent.Contains(clientId))
				{
					this.lastClientIdsSent.Add(clientId);
					while (this.lastClientIdsSent.Count > 5)
					{
						this.lastClientIdsSent.RemoveAt(0);
					}
					this.globalStateModified |= NewSyncStatusData.GlobalInfoStateModified.LastClientIdsSeen;
				}
			}
		}

		public bool ContainsClientId(string clientId)
		{
			bool result;
			lock (this.instanceLock)
			{
				if (this.lastClientIdsSent == null)
				{
					result = false;
				}
				else
				{
					result = this.lastClientIdsSent.Contains(clientId);
				}
			}
			return result;
		}

		public bool ContainsClientCategoryHash(int hashName)
		{
			bool result;
			lock (this.instanceLock)
			{
				if (this.clientCategoryList == null)
				{
					result = false;
				}
				else
				{
					result = this.clientCategoryList.Contains(hashName);
				}
			}
			return result;
		}

		public void AddClientCategoryHash(int hashName)
		{
			lock (this.instanceLock)
			{
				if (this.clientCategoryList == null)
				{
					this.clientCategoryList = new List<int>();
				}
				if (!this.clientCategoryList.Contains(hashName))
				{
					this.clientCategoryList.Add(hashName);
					this.globalStateModified |= NewSyncStatusData.GlobalInfoStateModified.ClientCategoryList;
				}
			}
		}

		public void ClearClientCategoryHash()
		{
			this.clientCategoryList = null;
			this.globalStateModified |= NewSyncStatusData.GlobalInfoStateModified.ClientCategoryList;
		}

		public static ISyncStatusData Load(GlobalInfo globalInfo, SyncStateStorage syncStateStorage)
		{
			NewSyncStatusData newSyncStatusData = new NewSyncStatusData();
			newSyncStatusData.globalInfo = globalInfo;
			newSyncStatusData.deviceMetadata = syncStateStorage.DeviceMetadata;
			if (!globalInfo.HasNewSyncData)
			{
				using (SyncStatusData syncStatusData = SyncStatusData.Load(syncStateStorage) as SyncStatusData)
				{
					newSyncStatusData.clientCategoryList = ((syncStatusData.ClientCategoryList == null) ? null : new List<int>(syncStatusData.ClientCategoryList));
					newSyncStatusData.lastClientIdsSent = ((syncStatusData.LastClientIdsSeen == null) ? null : new List<string>(syncStatusData.LastClientIdsSeen));
					newSyncStatusData.lastSyncRequestRandomString = syncStatusData.LastSyncRequestRandomString;
					newSyncStatusData.lastCachableWbxmlDocument = syncStatusData.LastCachableWbxmlDocument;
					newSyncStatusData.ClientCanSendUpEmptyRequests = syncStatusData.ClientCanSendUpEmptyRequests;
					newSyncStatusData.lastSyncAttemptTime = syncStatusData.LastSyncAttemptTime;
					newSyncStatusData.lastSyncSuccessTime = syncStatusData.LastSyncSuccessTime;
					newSyncStatusData.lastSyncUserAgent = syncStatusData.LastSyncUserAgent;
				}
				newSyncStatusData.deviceMetadataStateChanged = true;
				newSyncStatusData.globalStateModified = NewSyncStatusData.GlobalInfoStateModified.All;
			}
			else
			{
				newSyncStatusData.clientCategoryList = ((globalInfo.ClientCategoryHashList == null) ? null : new List<int>(globalInfo.ClientCategoryHashList));
				newSyncStatusData.lastClientIdsSent = ((globalInfo.LastClientIdsSeen == null) ? null : new List<string>(globalInfo.LastClientIdsSeen));
				newSyncStatusData.lastSyncRequestRandomString = newSyncStatusData.deviceMetadata.LastSyncRequestRandomString;
				newSyncStatusData.lastCachableWbxmlDocument = newSyncStatusData.deviceMetadata.LastCachedSyncRequest;
				newSyncStatusData.ClientCanSendUpEmptyRequests = newSyncStatusData.deviceMetadata.ClientCanSendUpEmptyRequests;
				newSyncStatusData.lastSyncAttemptTime = globalInfo.LastSyncAttemptTime;
				newSyncStatusData.lastSyncSuccessTime = globalInfo.LastSyncSuccessTime;
				newSyncStatusData.lastSyncUserAgent = globalInfo.LastSyncUserAgent;
				newSyncStatusData.deviceMetadataStateChanged = false;
				newSyncStatusData.globalStateModified = NewSyncStatusData.GlobalInfoStateModified.None;
			}
			return newSyncStatusData;
		}

		public void SaveAndDispose()
		{
			if (this.globalInfo == null || this.globalInfo.IsDisposed)
			{
				return;
			}
			if (this.globalStateModified != NewSyncStatusData.GlobalInfoStateModified.None)
			{
				if ((this.globalStateModified & NewSyncStatusData.GlobalInfoStateModified.ClientCategoryList) == NewSyncStatusData.GlobalInfoStateModified.ClientCategoryList)
				{
					this.globalInfo.ClientCategoryHashList = ((this.clientCategoryList == null) ? null : this.clientCategoryList.ToArray());
				}
				if ((this.globalStateModified & NewSyncStatusData.GlobalInfoStateModified.LastClientIdsSeen) == NewSyncStatusData.GlobalInfoStateModified.LastClientIdsSeen)
				{
					this.globalInfo.LastClientIdsSeen = ((this.lastClientIdsSent == null) ? null : this.lastClientIdsSent.ToArray());
				}
				if ((this.globalStateModified & NewSyncStatusData.GlobalInfoStateModified.LastSyncAttemptTime) == NewSyncStatusData.GlobalInfoStateModified.LastSyncAttemptTime)
				{
					this.globalInfo.LastSyncAttemptTime = this.lastSyncAttemptTime;
				}
				if ((this.globalStateModified & NewSyncStatusData.GlobalInfoStateModified.LastSyncSuccessTime) == NewSyncStatusData.GlobalInfoStateModified.LastSyncSuccessTime)
				{
					this.globalInfo.LastSyncSuccessTime = this.lastSyncSuccessTime;
				}
				if ((this.globalStateModified & NewSyncStatusData.GlobalInfoStateModified.LastSyncUserAgent) == NewSyncStatusData.GlobalInfoStateModified.LastSyncUserAgent)
				{
					this.globalInfo.LastSyncUserAgent = this.lastSyncUserAgent;
				}
			}
			if (this.deviceMetadataStateChanged)
			{
				this.deviceMetadata.SaveSyncStatusData(this.lastSyncRequestRandomString, this.lastCachableWbxmlDocument, this.ClientCanSendUpEmptyRequests);
			}
			this.deviceMetadataStateChanged = false;
			this.globalStateModified = NewSyncStatusData.GlobalInfoStateModified.None;
			this.globalInfo = null;
		}

		private NewSyncStatusData.GlobalInfoStateModified globalStateModified;

		private bool deviceMetadataStateChanged;

		private object instanceLock = new object();

		private List<string> lastClientIdsSent;

		private List<int> clientCategoryList;

		private string lastSyncRequestRandomString;

		private byte[] lastCachableWbxmlDocument;

		private bool clientCanSendUpEmptyRequests;

		private ExDateTime? lastSyncAttemptTime;

		private ExDateTime? lastSyncSuccessTime;

		private string lastSyncUserAgent;

		private GlobalInfo globalInfo;

		private DeviceSyncStateMetadata deviceMetadata;

		[Flags]
		private enum GlobalInfoStateModified
		{
			None = 0,
			LastSyncAttemptTime = 1,
			LastSyncSuccessTime = 2,
			LastSyncUserAgent = 4,
			ClientCategoryList = 8,
			LastClientIdsSeen = 16,
			All = 2147483647
		}
	}
}
