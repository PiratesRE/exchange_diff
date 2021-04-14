using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class SyncStateDataInfo : DisposeTrackableBase
	{
		public SyncStateDataInfo(CustomSyncState wrappedSyncState)
		{
			this.customSyncState = wrappedSyncState;
		}

		protected internal bool IsDirty { get; set; }

		public int? BackendVersion
		{
			get
			{
				int? backendVersion;
				lock (this.lockObject)
				{
					backendVersion = this.customSyncState.BackendVersion;
				}
				return backendVersion;
			}
		}

		public int Version
		{
			get
			{
				int version;
				lock (this.lockObject)
				{
					version = this.customSyncState.Version;
				}
				return version;
			}
		}

		public void SaveToMailbox()
		{
			if (!this.IsDirty)
			{
				return;
			}
			lock (this.lockObject)
			{
				this.customSyncState.Commit();
				this.IsDirty = false;
			}
		}

		public object TryGetProperty(StorePropertyDefinition propDef)
		{
			object result;
			lock (this.lockObject)
			{
				result = this.customSyncState.StoreObject.TryGetProperty(propDef);
			}
			return result;
		}

		public bool TryGetProperty<T>(StorePropertyDefinition propDef, out T propertyValue)
		{
			bool result;
			lock (this.lockObject)
			{
				result = AirSyncUtility.TryGetPropertyFromBag<T>(this.customSyncState.StoreObject, propDef, out propertyValue);
			}
			return result;
		}

		public void DeleteProperty(StorePropertyDefinition propDef)
		{
			lock (this.lockObject)
			{
				this.customSyncState.StoreObject.Delete(propDef);
			}
		}

		public void SetProperty(StorePropertyDefinition propDef, object value)
		{
			lock (this.lockObject)
			{
				this.customSyncState.StoreObject[propDef] = value;
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				lock (this.lockObject)
				{
					if (this.customSyncState != null)
					{
						this.customSyncState.Dispose();
						this.customSyncState = null;
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncStateDataInfo>(this);
		}

		protected void Assign<T, RawT>(string name, RawT value) where T : ComponentData<RawT>, new()
		{
			lock (this.lockObject)
			{
				object obj2 = this.customSyncState[name];
				if (obj2 != null && obj2 is T)
				{
					T t = (T)((object)obj2);
					RawT data = t.Data;
					if ((data != null && data.Equals(value)) || (data == null && value == null))
					{
						return;
					}
				}
				T t2 = Activator.CreateInstance<T>();
				t2.Data = value;
				this.customSyncState[name] = t2;
				this.IsDirty = true;
			}
		}

		protected RawT Fetch<T, RawT>(string name, RawT valueIfNotSet) where T : ComponentData<RawT>, new()
		{
			RawT result;
			lock (this.lockObject)
			{
				RawT data = this.customSyncState.GetData<T, RawT>(name, valueIfNotSet);
				result = data;
			}
			return result;
		}

		protected ExDateTime? FetchDateTime(string name)
		{
			ExDateTime? result;
			lock (this.lockObject)
			{
				object obj2 = this.customSyncState[name];
				if (obj2 == null)
				{
					result = null;
				}
				else if (obj2 is NullableData<DateTimeData, ExDateTime>)
				{
					ExDateTime? data = ((NullableData<DateTimeData, ExDateTime>)obj2).Data;
					result = data;
				}
				else
				{
					if (!(obj2 is DateTimeData))
					{
						throw new AirSyncPermanentException(false)
						{
							ErrorStringForProtocolLogger = "CorruptDateTimeObjectInSyncState"
						};
					}
					ExDateTime? exDateTime = new ExDateTime?(((DateTimeData)obj2).Data);
					result = exDateTime;
				}
			}
			return result;
		}

		private CustomSyncState customSyncState;

		private object lockObject = new object();
	}
}
