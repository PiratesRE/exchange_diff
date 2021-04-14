using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class SyncStatusData : DisposeTrackableBase, ISyncStatusData
	{
		public List<int> ClientCategoryList
		{
			get
			{
				return this.clientCategoryList;
			}
		}

		public List<string> LastClientIdsSeen
		{
			get
			{
				return this.lastClientIdsSent;
			}
		}

		public string LastSyncRequestRandomString
		{
			get
			{
				StringData stringData = (StringData)this.syncStatusSyncState[CustomStateDatumType.LastSyncRequestRandomNumber];
				if (stringData != null)
				{
					return stringData.Data;
				}
				return null;
			}
			set
			{
				this.syncStatusSyncState[CustomStateDatumType.LastSyncRequestRandomNumber] = new StringData(value);
				this.dirty = true;
			}
		}

		public byte[] LastCachableWbxmlDocument
		{
			get
			{
				ByteArrayData byteArrayData = (ByteArrayData)this.syncStatusSyncState[CustomStateDatumType.LastCachableWbxmlDocument];
				if (byteArrayData != null)
				{
					return byteArrayData.Data;
				}
				return null;
			}
			set
			{
				this.syncStatusSyncState[CustomStateDatumType.LastCachableWbxmlDocument] = new ByteArrayData(value);
				this.dirty = true;
			}
		}

		public ExDateTime? LastSyncAttemptTime
		{
			get
			{
				DateTimeData dateTimeData = (DateTimeData)this.syncStatusSyncState[CustomStateDatumType.LastSyncAttemptTime];
				return new ExDateTime?((dateTimeData == null) ? ExDateTime.MinValue : dateTimeData.Data);
			}
			set
			{
				if (value == null)
				{
					this.syncStatusSyncState.Remove(CustomStateDatumType.LastSyncAttemptTime);
				}
				else
				{
					this.syncStatusSyncState[CustomStateDatumType.LastSyncAttemptTime] = new DateTimeData(value.Value);
				}
				this.dirty = true;
			}
		}

		public ExDateTime? LastSyncSuccessTime
		{
			get
			{
				DateTimeData dateTimeData = (DateTimeData)this.syncStatusSyncState[CustomStateDatumType.LastSyncSuccessTime];
				return new ExDateTime?((dateTimeData == null) ? ExDateTime.MinValue : dateTimeData.Data);
			}
			set
			{
				if (value == null)
				{
					this.syncStatusSyncState.Remove(CustomStateDatumType.LastSyncAttemptTime);
				}
				else
				{
					this.syncStatusSyncState[CustomStateDatumType.LastSyncSuccessTime] = new DateTimeData(value.Value);
				}
				this.dirty = true;
			}
		}

		public string LastSyncUserAgent
		{
			get
			{
				ConstStringData constStringData = (ConstStringData)this.syncStatusSyncState[CustomStateDatumType.UserAgent];
				if (constStringData != null)
				{
					return constStringData.Data;
				}
				return null;
			}
			set
			{
				this.syncStatusSyncState[CustomStateDatumType.UserAgent] = new ConstStringData(StaticStringPool.Instance.Intern(value));
				this.dirty = true;
			}
		}

		public bool ClientCanSendUpEmptyRequests
		{
			get
			{
				BooleanData booleanData = (BooleanData)this.syncStatusSyncState[CustomStateDatumType.ClientCanSendUpEmptyRequests];
				return booleanData != null && booleanData.Data;
			}
			set
			{
				this.syncStatusSyncState[CustomStateDatumType.ClientCanSendUpEmptyRequests] = new BooleanData(value);
				this.dirty = true;
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
					this.lastClientIdsSentModified = true;
					this.dirty = true;
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
					this.clientCategoryListModified = true;
					this.dirty = true;
				}
			}
		}

		public void ClearClientCategoryHash()
		{
			this.clientCategoryList = null;
			this.clientCategoryListModified = true;
			this.dirty = true;
		}

		private List<int> GetClientCategoryList()
		{
			GenericListData<Int32Data, int> genericListData = this.syncStatusSyncState[CustomStateDatumType.ClientCategoryList] as GenericListData<Int32Data, int>;
			if (genericListData == null || genericListData.Data == null)
			{
				return null;
			}
			return genericListData.Data;
		}

		private List<string> GetLastIdsSeen()
		{
			GenericListData<StringData, string> genericListData = this.syncStatusSyncState[CustomStateDatumType.LastClientIdsSent] as GenericListData<StringData, string>;
			if (genericListData == null || genericListData.Data == null)
			{
				return null;
			}
			return genericListData.Data;
		}

		public static ISyncStatusData Load(SyncStateStorage syncStateStorage)
		{
			SyncStatusData syncStatusData = null;
			bool flag = false;
			ISyncStatusData result;
			try
			{
				syncStatusData = new SyncStatusData();
				syncStatusData.syncStatusSyncState = AirSyncUtility.GetOrCreateSyncStatusSyncState(syncStateStorage);
				syncStatusData.clientCategoryList = syncStatusData.GetClientCategoryList();
				syncStatusData.lastClientIdsSent = syncStatusData.GetLastIdsSeen();
				syncStatusData.dirty = false;
				syncStatusData.clientCategoryListModified = false;
				syncStatusData.lastClientIdsSentModified = false;
				flag = true;
				result = syncStatusData;
			}
			finally
			{
				if (!flag && syncStatusData != null)
				{
					syncStatusData.Dispose();
					syncStatusData = null;
				}
			}
			return result;
		}

		public void SaveAndDispose()
		{
			if (this.syncStatusSyncState != null && this.dirty)
			{
				try
				{
					if (this.clientCategoryListModified)
					{
						if (this.clientCategoryList == null)
						{
							this.syncStatusSyncState.Remove(CustomStateDatumType.ClientCategoryList);
						}
						else
						{
							this.syncStatusSyncState[CustomStateDatumType.ClientCategoryList] = new GenericListData<Int32Data, int>(this.clientCategoryList);
						}
					}
					if (this.lastClientIdsSentModified)
					{
						if (this.lastClientIdsSent == null)
						{
							this.syncStatusSyncState.Remove(CustomStateDatumType.LastClientIdsSent);
						}
						else
						{
							this.syncStatusSyncState[CustomStateDatumType.LastClientIdsSent] = new GenericListData<StringData, string>(this.lastClientIdsSent);
						}
					}
					this.syncStatusSyncState.Commit();
					this.dirty = false;
					this.clientCategoryListModified = false;
					this.lastClientIdsSentModified = false;
				}
				catch (LocalizedException arg)
				{
					AirSyncDiagnostics.TraceError<LocalizedException>(ExTraceGlobals.RequestsTracer, this, "[SyncData.Save] Failed to commit syncStatusSyncState. Exception: {0}", arg);
				}
				finally
				{
					this.Dispose();
				}
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.syncStatusSyncState != null)
			{
				this.syncStatusSyncState.Dispose();
				this.syncStatusSyncState = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncStatusData>(this);
		}

		private bool dirty;

		private object instanceLock = new object();

		private List<string> lastClientIdsSent;

		private bool lastClientIdsSentModified;

		private List<int> clientCategoryList;

		private bool clientCategoryListModified;

		private CustomSyncState syncStatusSyncState;
	}
}
