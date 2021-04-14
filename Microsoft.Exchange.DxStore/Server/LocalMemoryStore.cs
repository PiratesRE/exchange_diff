using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	public class LocalMemoryStore : ILocalDataStore
	{
		public LocalMemoryStore(string identity)
		{
			this.LastInstanceExecuted = 0;
			this.identity = identity;
			this.identityHash = identity.GetHashCode();
			this.rootContainer = new KeyContainer("\\", null);
			this.CreateKey(null, "Private");
			this.CreateKey(null, "Public");
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LocalStoreTracer;
			}
		}

		public int LastInstanceExecuted { get; set; }

		public DateTimeOffset LastUpdateTime { get; set; }

		public static bool IsRootKeyName(string keyName)
		{
			return Utils.IsEqual(keyName, "\\", StringComparison.OrdinalIgnoreCase);
		}

		public bool CreateKey(int? instanceNumber, string keyName)
		{
			return this.locker.WithWriteLock(delegate()
			{
				this.SetStats(instanceNumber);
				int num;
				this.FindContainer(keyName, true, out num);
				return num > 0;
			});
		}

		public bool DeleteKey(int? instanceNumber, string keyName)
		{
			return this.locker.WithWriteLock(delegate()
			{
				bool result = false;
				this.SetStats(instanceNumber);
				KeyContainer keyContainer = this.FindContainer(keyName, false);
				if (keyContainer != null && keyContainer.Parent != null)
				{
					result = keyContainer.Parent.SubKeys.Remove(keyContainer.Name);
				}
				return result;
			});
		}

		public void SetProperty(int? instanceNumber, string keyName, string propertyName, PropertyValue propertyValue)
		{
			this.locker.WithWriteLock(delegate()
			{
				this.SetStats(instanceNumber);
				KeyContainer container = this.FindContainer(keyName, true);
				this.SetPropertyInternal(container, propertyName, propertyValue.Clone());
			});
		}

		public bool DeleteProperty(int? instanceNumber, string keyName, string propertyName)
		{
			return this.locker.WithWriteLock(delegate()
			{
				this.SetStats(instanceNumber);
				KeyContainer container = this.FindContainer(keyName, false);
				return this.DeletePropertyInternal(container, propertyName);
			});
		}

		public void ExecuteBatch(int? instanceNumber, string keyName, DxStoreBatchCommand[] commands)
		{
			this.locker.WithWriteLock(delegate()
			{
				this.SetStats(instanceNumber);
				KeyContainer container = this.FindContainer(keyName, true);
				this.ExecuteBatchInternal(instanceNumber, container, commands);
			});
		}

		public bool IsKeyExist(string keyName)
		{
			return this.locker.WithReadLock(() => this.FindContainer(keyName, false) != null);
		}

		public DataStoreStats GetStoreStats()
		{
			return this.locker.WithReadLock(() => new DataStoreStats
			{
				LastUpdateNumber = this.LastInstanceExecuted,
				LastUpdateTime = this.LastUpdateTime
			});
		}

		public string[] EnumSubkeyNames(string keyName)
		{
			return this.locker.WithReadLock(delegate()
			{
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, int, string>((long)this.identityHash, "{0}: [{1}] Entering EnumSubkeyNames - Key: {2}", this.identity, this.LastInstanceExecuted, keyName);
				string[] array = this.emptyStringArray;
				KeyContainer keyContainer = this.FindContainer(keyName, false);
				if (keyContainer != null)
				{
					array = keyContainer.SubKeys.Keys.ToArray<string>();
				}
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, int>((long)this.identityHash, "{0}: Exiting EnumSubkeyNames (Count: {1})", this.identity, array.Length);
				return array;
			});
		}

		public bool IsPropertyExist(string keyName, string propertyName)
		{
			return this.locker.WithReadLock(delegate()
			{
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, string, string>((long)this.identityHash, "{0}: Entering IsPropertyExist - Key: {1} PropertyName: {2}", this.identity, keyName, propertyName);
				bool flag = false;
				KeyContainer keyContainer = this.FindContainer(keyName, false);
				if (keyContainer != null)
				{
					PropertyValue propertyValue;
					flag = keyContainer.Properties.TryGetValue(propertyName, out propertyValue);
				}
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, string, bool>((long)this.identityHash, "{0}: Exiting IsPropertyExist - IsExist: {1}", this.identity, keyName, flag);
				return flag;
			});
		}

		public PropertyValue GetProperty(string keyName, string propertyName)
		{
			PropertyValue propertyValue = null;
			this.locker.WithReadLock(delegate()
			{
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, string, string>((long)this.identityHash, "{0}: Entering GetProperty - Key: {1} PropertyName: {2}", this.identity, keyName, propertyName);
				KeyContainer keyContainer = this.FindContainer(keyName, false);
				if (keyContainer != null)
				{
					keyContainer.Properties.TryGetValue(propertyName, out propertyValue);
				}
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, bool>((long)this.identityHash, "{0}: Exiting GetProperty - IsFound {1}", this.identity, propertyValue != null);
			});
			if (propertyValue != null)
			{
				propertyValue = propertyValue.Clone();
			}
			return propertyValue;
		}

		public Tuple<string, PropertyValue>[] GetAllProperties(string keyName)
		{
			Tuple<string, PropertyValue>[] properties = Utils.EmptyArray<Tuple<string, PropertyValue>>();
			this.locker.WithReadLock(delegate()
			{
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, string>((long)this.identityHash, "{0}: Entering GetAllProperties - Key: {1}", this.identity, keyName);
				KeyContainer keyContainer = this.FindContainer(keyName, false);
				if (keyContainer != null)
				{
					properties = (from prop in keyContainer.Properties
					select new Tuple<string, PropertyValue>(prop.Key, (prop.Value != null) ? prop.Value.Clone() : null)).ToArray<Tuple<string, PropertyValue>>();
				}
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, int>((long)this.identityHash, "{0}: Exiting GetAllProperties - found {1} entries", this.identity, properties.Length);
			});
			return properties;
		}

		public PropertyNameInfo[] EnumPropertyNames(string keyName)
		{
			return this.locker.WithReadLock(delegate()
			{
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, int, string>((long)this.identityHash, "{0}: [{1}] Entering EnumPropertyNames - Key: {2}", this.identity, this.LastInstanceExecuted, keyName);
				KeyContainer keyContainer = this.FindContainer(keyName, false);
				PropertyNameInfo[] array;
				if (keyContainer != null)
				{
					array = (from kv in keyContainer.Properties
					select new PropertyNameInfo
					{
						Name = kv.Key,
						Kind = kv.Value.Kind
					}).ToArray<PropertyNameInfo>();
				}
				else
				{
					array = Utils.EmptyArray<PropertyNameInfo>();
				}
				ExTraceGlobals.StoreReadTracer.TraceDebug<string, int>((long)this.identityHash, "{0}: Exiting EnumPropertyNames (Count: {1})", this.identity, array.Length);
				return array;
			});
		}

		public InstanceSnapshotInfo GetSnapshot(string keyName = null, bool isCompress = true)
		{
			keyName = this.NormalizeKeyName(keyName);
			ExTraceGlobals.StoreReadTracer.TraceDebug<string, string, bool>((long)this.identityHash, "{0}: Entering GetSnapshot - Key: {1} IsCompress: {2}", this.identity, keyName, isCompress);
			InstanceSnapshotInfo instanceSnapshotInfo = new InstanceSnapshotInfo
			{
				FullKeyName = keyName,
				RetrievalStartTime = DateTimeOffset.Now
			};
			int num;
			XElement xelementSnapshot = this.GetXElementSnapshot(keyName, out num);
			if (xelementSnapshot == null)
			{
				throw new DxStoreInstanceKeyNotFoundException(keyName);
			}
			instanceSnapshotInfo.Snapshot = xelementSnapshot.ToString();
			if (isCompress)
			{
				instanceSnapshotInfo.Compress();
			}
			instanceSnapshotInfo.LastInstanceExecuted = num;
			instanceSnapshotInfo.RetrievalFinishTime = DateTimeOffset.Now;
			ExTraceGlobals.StoreReadTracer.TraceDebug<string, int, int>((long)this.identityHash, "{0}: [1] Exiting GetSnapshot (Length: {2})", this.identity, num, instanceSnapshotInfo.Snapshot.Length);
			return instanceSnapshotInfo;
		}

		public XElement GetXElementSnapshot(string keyName, out int lastInstanceExecuted)
		{
			ExTraceGlobals.StoreReadTracer.TraceDebug<string, string>((long)this.identityHash, "{0}: Entering GetXElementSnapshot - Key: {1}", this.identity, keyName);
			keyName = this.NormalizeKeyName(keyName);
			int tmpLastInstanceExecuted = 0;
			XElement element = null;
			this.locker.WithReadLock(delegate()
			{
				KeyContainer keyContainer = this.FindContainer(keyName, false);
				if (keyContainer != null)
				{
					element = keyContainer.GetSnapshot(true);
				}
				tmpLastInstanceExecuted = this.LastInstanceExecuted;
			});
			lastInstanceExecuted = tmpLastInstanceExecuted;
			ExTraceGlobals.StoreReadTracer.TraceDebug<string, int>((long)this.identityHash, "{0}: [1] Exiting GetXElementSnapshot", this.identity, lastInstanceExecuted);
			return element;
		}

		public void ApplySnapshot(InstanceSnapshotInfo snapshotInfo, int? instanceNumber)
		{
			if (snapshotInfo == null || string.IsNullOrEmpty(snapshotInfo.Snapshot))
			{
				ExTraceGlobals.StoreWriteTracer.TraceError<string, int>((long)this.identityHash, "{0}: [1] ApplySnapshot ignored since snapshot info is not valid", this.identity, instanceNumber ?? -1);
				return;
			}
			snapshotInfo.Decompress();
			XElement rootElement = XElement.Parse(snapshotInfo.Snapshot);
			int num = (instanceNumber != null) ? instanceNumber.Value : snapshotInfo.LastInstanceExecuted;
			this.ApplySnapshotFromXElement(snapshotInfo.FullKeyName, num, rootElement);
			ExTraceGlobals.StoreWriteTracer.TraceDebug<string, int>((long)this.identityHash, "{0}: [1] Exiting ApplySnapshot", this.identity, num);
		}

		public void ApplySnapshotFromXElement(string keyName, int lastInstanceExecuted, XElement rootElement)
		{
			keyName = this.NormalizeKeyName(keyName);
			KeyContainer parent = null;
			this.locker.WithWriteLock(delegate()
			{
				ExTraceGlobals.StoreWriteTracer.TraceDebug<string, int, string>((long)this.identityHash, "{0}: [1] Entering ApplySnapshotFromXElement (Key: {2})", this.identity, this.LastInstanceExecuted, keyName);
				if (!LocalMemoryStore.IsRootKeyName(keyName))
				{
					KeyContainer keyContainer = this.FindContainer(keyName, true);
					if (keyContainer != null)
					{
						parent = keyContainer.Parent;
						parent.SubKeys.Remove(keyContainer.Name);
					}
				}
				string keyLastPart = this.GetKeyLastPart(keyName);
				rootElement.SetAttributeValue("Name", keyLastPart);
				KeyContainer keyContainer2 = KeyContainer.Create(rootElement, parent);
				if (parent == null)
				{
					this.rootContainer = keyContainer2;
				}
				this.SetStats(new int?(lastInstanceExecuted));
				ExTraceGlobals.StoreWriteTracer.TraceDebug<string, int>((long)this.identityHash, "{0}: [1] Exiting ApplySnapshotFromXElement", this.identity, lastInstanceExecuted);
			});
		}

		private string NormalizeKeyName(string keyName)
		{
			if (string.IsNullOrEmpty(keyName))
			{
				keyName = "\\";
			}
			return keyName;
		}

		private void SetStats(int? instanceNumber)
		{
			if (instanceNumber != null)
			{
				this.LastInstanceExecuted = instanceNumber.Value;
			}
			this.LastUpdateTime = DateTimeOffset.Now;
		}

		private void ExecuteBatchInternal(int? instanceNumber, KeyContainer container, DxStoreBatchCommand[] commands)
		{
			int num = -1;
			bool flag = ExTraceGlobals.StoreWriteTracer.IsTraceEnabled(TraceType.DebugTrace);
			foreach (DxStoreBatchCommand dxStoreBatchCommand in commands)
			{
				num++;
				bool flag2 = false;
				WellKnownBatchCommandName typeId = dxStoreBatchCommand.GetTypeId();
				if (flag)
				{
					ExTraceGlobals.StoreWriteTracer.TraceDebug<int, WellKnownBatchCommandName, string>((long)this.identityHash, "Executing batch operation# {0} - {1}: {2}", num, typeId, dxStoreBatchCommand.GetDebugString());
				}
				switch (typeId)
				{
				case WellKnownBatchCommandName.CreateKey:
				{
					DxStoreBatchCommand.CreateKey createKey = dxStoreBatchCommand as DxStoreBatchCommand.CreateKey;
					if (createKey != null)
					{
						flag2 = true;
						container = this.FindContainer(this.JoinKeys(container.FullName, createKey.Name), true);
					}
					break;
				}
				case WellKnownBatchCommandName.DeleteKey:
				{
					DxStoreBatchCommand.DeleteKey deleteKey = dxStoreBatchCommand as DxStoreBatchCommand.DeleteKey;
					if (deleteKey != null)
					{
						flag2 = true;
						KeyContainer keyContainer = this.FindContainer(this.JoinKeys(container.FullName, deleteKey.Name), false);
						if (instanceNumber != null && keyContainer != null && keyContainer.Parent != null)
						{
							keyContainer.Parent.SubKeys.Remove(keyContainer.Name);
						}
					}
					break;
				}
				case WellKnownBatchCommandName.SetProperty:
				{
					DxStoreBatchCommand.SetProperty setProperty = dxStoreBatchCommand as DxStoreBatchCommand.SetProperty;
					if (setProperty != null)
					{
						flag2 = true;
						this.SetPropertyInternal(container, setProperty.Name, setProperty.Value.Clone());
					}
					break;
				}
				case WellKnownBatchCommandName.DeleteProperty:
				{
					DxStoreBatchCommand.DeleteProperty deleteProperty = dxStoreBatchCommand as DxStoreBatchCommand.DeleteProperty;
					if (deleteProperty != null)
					{
						flag2 = true;
						this.DeletePropertyInternal(container, deleteProperty.Name);
					}
					break;
				}
				}
				if (!flag2 && ExTraceGlobals.StoreWriteTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.StoreWriteTracer.TraceError<int, WellKnownBatchCommandName, string>((long)this.identityHash, "Unknown batch command# {0} - {1}: {2}", num, typeId, dxStoreBatchCommand.GetType().Name);
				}
			}
		}

		private void SetPropertyInternal(KeyContainer container, string propertyName, PropertyValue propertyValue)
		{
			if (container != null)
			{
				container.Properties[propertyName] = propertyValue;
			}
		}

		private bool DeletePropertyInternal(KeyContainer container, string propertyName)
		{
			return container != null && container.Properties.Remove(propertyName);
		}

		private string JoinKeys(string key1, string key2)
		{
			return Utils.CombinePathNullSafe(key1, key2);
		}

		private KeyContainer FindContainer(string keyName, bool isAllowCreate = false)
		{
			int num;
			return this.FindContainer(keyName, isAllowCreate, out num);
		}

		private string GetKeyLastPart(string fullKeyName)
		{
			IEnumerable<string> source = from p in fullKeyName.Split(this.splitChar)
			where !string.IsNullOrEmpty(p)
			select p;
			string text = source.LastOrDefault<string>();
			if (string.IsNullOrEmpty(text))
			{
				text = "\\";
			}
			return text;
		}

		private KeyContainer FindContainer(string keyName, bool isAllowCreate, out int keysCreatedCount)
		{
			keysCreatedCount = 0;
			if (LocalMemoryStore.IsRootKeyName(keyName))
			{
				return this.rootContainer;
			}
			KeyContainer keyContainer = this.rootContainer;
			IEnumerable<string> enumerable = from p in keyName.Split(this.splitChar)
			where !string.IsNullOrEmpty(p)
			select p;
			foreach (string text in enumerable)
			{
				KeyContainer keyContainer2;
				if (keyContainer.SubKeys.TryGetValue(text, out keyContainer2))
				{
					keyContainer = keyContainer2;
				}
				else
				{
					if (!isAllowCreate)
					{
						return null;
					}
					keysCreatedCount++;
					keyContainer2 = new KeyContainer(text, keyContainer);
					keyContainer.SubKeys[text] = keyContainer2;
					keyContainer = keyContainer2;
				}
			}
			return keyContainer;
		}

		private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		private readonly string[] emptyStringArray = new string[0];

		private readonly char[] splitChar = new char[]
		{
			'\\'
		};

		private readonly string identity;

		private readonly int identityHash;

		private KeyContainer rootContainer;
	}
}
