using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class DistributedStoreKey : IDistributedStoreKey, IDisposable
	{
		public DistributedStoreKey(string parentKeyName, string subkeyName, DxStoreKeyAccessMode mode, DistributedStore.Context context = null)
		{
			this.InstanceId = Guid.NewGuid();
			if (string.IsNullOrEmpty(parentKeyName))
			{
				parentKeyName = string.Empty;
			}
			if (string.IsNullOrEmpty(subkeyName))
			{
				subkeyName = string.Empty;
			}
			this.FullKeyName = Path.Combine(parentKeyName, subkeyName);
			this.Mode = mode;
			this.Context = context;
		}

		public string FullKeyName { get; private set; }

		public DxStoreKeyAccessMode Mode { get; private set; }

		public DistributedStore.Context Context { get; private set; }

		public bool IsClosed { get; set; }

		internal IDistributedStoreKey PrimaryStoreKey { get; set; }

		internal IDistributedStoreKey ShadowStoreKey { get; set; }

		internal Guid InstanceId { get; set; }

		internal bool IsBaseKey { get; set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IDistributedStoreKey OpenKey(string keyName, DxStoreKeyAccessMode mode, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			return this.OpenKeyInternal(keyName, mode, isIgnoreIfNotExist, constraints);
		}

		public void CloseKey()
		{
			lock (this.locker)
			{
				this.CloseKeyInternal();
			}
		}

		public bool DeleteKey(string keyName, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			return this.DeleteKeyInternal(keyName, isIgnoreIfNotExist, constraints);
		}

		public bool SetValue(string propertyName, object propertyValue, RegistryValueKind valueKind, bool isBestEffort, ReadWriteConstraints constraints)
		{
			return this.SetValueInternal(propertyName, propertyValue, valueKind, isBestEffort, constraints);
		}

		public object GetValue(string propertyName, out bool isValueExist, out RegistryValueKind valueKind, ReadWriteConstraints constraints)
		{
			return this.GetValueInternal(propertyName, out isValueExist, out valueKind, constraints);
		}

		public bool DeleteValue(string propertyName, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			return this.DeleteValueInternal(propertyName, isIgnoreIfNotExist, constraints);
		}

		public IEnumerable<string> GetSubkeyNames(ReadWriteConstraints constraints)
		{
			return this.GetSubkeyNamesInternal(constraints);
		}

		public IEnumerable<string> GetValueNames(ReadWriteConstraints constraints)
		{
			return this.GetValueNamesInternal(constraints);
		}

		public IEnumerable<Tuple<string, RegistryValueKind>> GetValueInfos(ReadWriteConstraints constraints)
		{
			return this.GetValueInfosInternal(constraints);
		}

		public IEnumerable<Tuple<string, object>> GetAllValues(ReadWriteConstraints constraints)
		{
			return this.GetAllValuesInternal(constraints);
		}

		public IDistributedStoreBatchRequest CreateBatchUpdateRequest()
		{
			return this.CreateBatchUpdateRequestInternal();
		}

		public void ExecuteBatchRequest(List<DxStoreBatchCommand> commands, ReadWriteConstraints constraints)
		{
			this.ExecuteBatchRequestInternal(commands, constraints);
		}

		public IDistributedStoreChangeNotify CreateChangeNotify(ChangeNotificationFlags flags, object context, Action callback)
		{
			throw new NotImplementedException("CreateChangeNotity not implemented yet");
		}

		protected virtual void Dispose(bool isDisposing)
		{
			lock (this.locker)
			{
				if (!this.isDisposed)
				{
					if (isDisposing)
					{
						this.CloseKey();
					}
					this.isDisposed = true;
				}
			}
		}

		private void ThrowIfKeyIsInvalid(IDistributedStoreKey key)
		{
			if (key == null)
			{
				throw new DxStoreKeyInvalidKeyException(this.FullKeyName);
			}
		}

		private IDistributedStoreKey OpenKeyInternal(string subKeyName, DxStoreKeyAccessMode mode, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			if (mode == DxStoreKeyAccessMode.CreateIfNotExist)
			{
				IDistributedStoreKey distributedStoreKey = this.OpenKeyFinal(subKeyName, DxStoreKeyAccessMode.Write, true, constraints);
				if (distributedStoreKey != null)
				{
					return distributedStoreKey;
				}
			}
			return this.OpenKeyFinal(subKeyName, mode, isIgnoreIfNotExist, constraints);
		}

		private IDistributedStoreKey OpenKeyFinal(string subKeyName, DxStoreKeyAccessMode mode, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			OperationCategory operationCategory = OperationCategory.OpenKey;
			OperationType operationType = OperationType.Read;
			if (mode == DxStoreKeyAccessMode.CreateIfNotExist)
			{
				operationCategory = OperationCategory.OpenOrCreateKey;
				operationType = OperationType.Write;
			}
			DistributedStoreKey compositeKey = new DistributedStoreKey(this.FullKeyName, subKeyName, mode, this.Context);
			IDistributedStoreKey result;
			try
			{
				result = (DistributedStore.Instance.ExecuteRequest<bool>(this, operationCategory, operationType, string.Format("SubKey: [{0}] Mode: [{1}] IsBestEffort: [{2}] IsConstrained: [{3}]", new object[]
				{
					subKeyName,
					mode,
					isIgnoreIfNotExist,
					constraints != null
				}), delegate(IDistributedStoreKey key, bool isPrimary, StoreKind storeKind)
				{
					this.ThrowIfKeyIsInvalid(key);
					IDistributedStoreKey distributedStoreKey = key.OpenKey(subKeyName, mode, isIgnoreIfNotExist, ReadWriteConstraints.Copy(constraints));
					if (distributedStoreKey != null)
					{
						DistributedStore.Instance.SetKeyByRole(compositeKey, isPrimary, distributedStoreKey);
						return true;
					}
					return false;
				}) ? compositeKey : null);
			}
			finally
			{
				if (compositeKey.PrimaryStoreKey == null)
				{
					compositeKey.CloseKey();
				}
			}
			return result;
		}

		private void CloseKeyInternal()
		{
			if (!this.IsClosed)
			{
				this.IsClosed = true;
				DistributedStore.Instance.ExecuteRequest(this, OperationCategory.CloseKey, OperationType.Read, string.Empty, delegate(IDistributedStoreKey key, bool isPrimary, StoreKind storeKind)
				{
					if (key != null)
					{
						key.CloseKey();
					}
				});
			}
		}

		private bool DeleteKeyInternal(string keyName, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			return DistributedStore.Instance.ExecuteRequest<bool>(this, OperationCategory.DeleteKey, OperationType.Write, string.Format("SubKey: [{0}] IsBestEffort: [{1}] IsConstrained: [{2}]", keyName, isIgnoreIfNotExist, constraints != null), delegate(IDistributedStoreKey key)
			{
				this.ThrowIfKeyIsInvalid(key);
				return key.DeleteKey(keyName, isIgnoreIfNotExist, ReadWriteConstraints.Copy(constraints));
			});
		}

		private bool SetValueInternal(string propertyName, object propertyValue, RegistryValueKind valueKind, bool isBestEffort, ReadWriteConstraints constraints)
		{
			return DistributedStore.Instance.ExecuteRequest<bool>(this, OperationCategory.SetValue, OperationType.Write, string.Format("Property: [{0}] PropertyKind: [{1}] IsBestEffort: [{2}] IsConstrained: [{3}]", new object[]
			{
				propertyName,
				valueKind,
				isBestEffort,
				constraints != null
			}), delegate(IDistributedStoreKey key)
			{
				this.ThrowIfKeyIsInvalid(key);
				return key.SetValue(propertyName, propertyValue, isBestEffort, constraints);
			});
		}

		private object GetValueInternal(string propertyName, out bool isValueExist, out RegistryValueKind valueKind, ReadWriteConstraints constraints)
		{
			Tuple<bool, RegistryValueKind, object> tuple = DistributedStore.Instance.ExecuteRequest<Tuple<bool, RegistryValueKind, object>>(this, OperationCategory.GetValue, OperationType.Read, string.Format("Property: [{0}]", propertyName), delegate(IDistributedStoreKey key)
			{
				this.ThrowIfKeyIsInvalid(key);
				bool item = false;
				RegistryValueKind item2 = RegistryValueKind.Unknown;
				object value = key.GetValue(propertyName, out item, out item2, ReadWriteConstraints.Copy(constraints));
				return new Tuple<bool, RegistryValueKind, object>(item, item2, value);
			});
			isValueExist = tuple.Item1;
			valueKind = tuple.Item2;
			return tuple.Item3;
		}

		public IEnumerable<Tuple<string, object>> GetAllValuesInternal(ReadWriteConstraints constraints)
		{
			return DistributedStore.Instance.ExecuteRequest<IEnumerable<Tuple<string, object>>>(this, OperationCategory.GetAllValues, OperationType.Read, string.Empty, delegate(IDistributedStoreKey key)
			{
				this.ThrowIfKeyIsInvalid(key);
				return key.GetAllValues(ReadWriteConstraints.Copy(constraints));
			});
		}

		private bool DeleteValueInternal(string propertyName, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			return DistributedStore.Instance.ExecuteRequest<bool>(this, OperationCategory.DeleteValue, OperationType.Write, string.Format("Property: [{0}] IsBestEffort: [{1}] IsConstrained: [{2}]", propertyName, isIgnoreIfNotExist, constraints != null), delegate(IDistributedStoreKey key)
			{
				this.ThrowIfKeyIsInvalid(key);
				return key.DeleteValue(propertyName, isIgnoreIfNotExist, ReadWriteConstraints.Copy(constraints));
			});
		}

		private IEnumerable<string> GetSubkeyNamesInternal(ReadWriteConstraints constraints)
		{
			return DistributedStore.Instance.ExecuteRequest<IEnumerable<string>>(this, OperationCategory.GetSubKeyNames, OperationType.Read, string.Empty, delegate(IDistributedStoreKey key)
			{
				this.ThrowIfKeyIsInvalid(key);
				return key.GetSubkeyNames(constraints);
			});
		}

		private IEnumerable<string> GetValueNamesInternal(ReadWriteConstraints constraints)
		{
			return DistributedStore.Instance.ExecuteRequest<IEnumerable<string>>(this, OperationCategory.GetValueNames, OperationType.Read, string.Empty, delegate(IDistributedStoreKey key)
			{
				this.ThrowIfKeyIsInvalid(key);
				return key.GetValueNames(constraints);
			});
		}

		private IEnumerable<Tuple<string, RegistryValueKind>> GetValueInfosInternal(ReadWriteConstraints constraints)
		{
			return DistributedStore.Instance.ExecuteRequest<IEnumerable<Tuple<string, RegistryValueKind>>>(this, OperationCategory.GetValueInfos, OperationType.Read, string.Empty, delegate(IDistributedStoreKey key)
			{
				this.ThrowIfKeyIsInvalid(key);
				return key.GetValueInfos(constraints);
			});
		}

		private IDistributedStoreBatchRequest CreateBatchUpdateRequestInternal()
		{
			return new GenericBatchRequest(this);
		}

		private void ExecuteBatchRequestInternal(List<DxStoreBatchCommand> commands, ReadWriteConstraints constraints)
		{
			DistributedStore.Instance.ExecuteRequest(this, OperationCategory.ExecuteBatch, OperationType.Write, string.Format("TotalCommands: [{0}] IsConstrained: [{1}]", commands.Count, constraints != null), delegate(IDistributedStoreKey key, bool isPrimary, StoreKind storeKind)
			{
				this.ThrowIfKeyIsInvalid(key);
				key.ExecuteBatchRequest(commands, constraints);
			});
		}

		private object locker = new object();

		private bool isDisposed;
	}
}
