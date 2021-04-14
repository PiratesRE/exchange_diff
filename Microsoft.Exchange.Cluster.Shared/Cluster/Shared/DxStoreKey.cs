using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class DxStoreKey : IDistributedStoreKey, IDisposable
	{
		public DxStoreKey(string keyFullName, DxStoreKeyAccessMode mode, DxStoreKey.BaseKeyParameters baseParameters)
		{
			this.FullKeyName = keyFullName;
			this.Mode = mode;
			this.BaseParameters = baseParameters;
		}

		public string FullKeyName { get; set; }

		public DxStoreKeyAccessMode Mode { get; set; }

		public DxStoreKey.BaseKeyParameters BaseParameters { get; set; }

		public bool IsBaseKey
		{
			get
			{
				return string.IsNullOrEmpty(this.FullKeyName);
			}
		}

		public T CreateRequest<T>() where T : DxStoreAccessRequest, new()
		{
			T result = Activator.CreateInstance<T>();
			result.Initialize(this.FullKeyName, this.BaseParameters.IsPrivate, this.BaseParameters.Self);
			return result;
		}

		public IDistributedStoreKey OpenKey(string subkeyName, DxStoreKeyAccessMode mode, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			return this.BaseParameters.KeyFactory.RunOperationAndTranslateException<IDistributedStoreKey>(OperationCategory.OpenKey, Path.Combine(this.FullKeyName, subkeyName), delegate()
			{
				ReadOptions readOptions = this.GetReadOptions(constraints);
				WriteOptions writeOptions = this.GetWriteOptions(constraints);
				DxStoreAccessRequest.CheckKey checkKey = this.CreateRequest<DxStoreAccessRequest.CheckKey>();
				checkKey.IsCreateIfNotExist = (mode == DxStoreKeyAccessMode.CreateIfNotExist);
				checkKey.SubkeyName = subkeyName;
				checkKey.ReadOptions = readOptions;
				checkKey.WriteOptions = writeOptions;
				DxStoreAccessReply.CheckKey checkKey2 = this.BaseParameters.Client.CheckKey(checkKey, null);
				this.SetReadResult(constraints, checkKey2.ReadResult);
				this.SetWriteResult(constraints, checkKey2.WriteResult);
				IDistributedStoreKey result = null;
				if (!checkKey2.IsExist)
				{
					if (!isIgnoreIfNotExist)
					{
						throw new DxStoreKeyNotFoundException(subkeyName);
					}
				}
				else
				{
					result = new DxStoreKey(Path.Combine(this.FullKeyName, subkeyName), mode, this.BaseParameters);
				}
				return result;
			}, false);
		}

		public void CloseKey()
		{
			this.Dispose();
		}

		public bool DeleteKey(string keyName, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			ReadOptions readOptions = this.GetReadOptions(constraints);
			WriteOptions writeOptions = this.GetWriteOptions(constraints);
			return this.BaseParameters.KeyFactory.RunOperationAndTranslateException<bool>(OperationCategory.DeleteKey, this.FullKeyName, delegate()
			{
				DxStoreAccessRequest.DeleteKey deleteKey = this.CreateRequest<DxStoreAccessRequest.DeleteKey>();
				deleteKey.SubkeyName = keyName;
				deleteKey.ReadOptions = readOptions;
				deleteKey.WriteOptions = writeOptions;
				DxStoreAccessReply.DeleteKey deleteKey2 = this.BaseParameters.Client.DeleteKey(deleteKey, null);
				if (!deleteKey2.IsExist && !isIgnoreIfNotExist)
				{
					throw new DxStoreKeyNotFoundException(keyName);
				}
				this.SetReadResult(constraints, deleteKey2.ReadResult);
				this.SetWriteResult(constraints, deleteKey2.WriteResult);
				return deleteKey2.IsExist;
			}, false);
		}

		public bool SetValue(string propertyName, object propertyValue, RegistryValueKind valueKind, bool isBestEffort, ReadWriteConstraints constraints)
		{
			WriteOptions writeOptions = this.GetWriteOptions(constraints);
			return this.BaseParameters.KeyFactory.RunOperationAndTranslateException<bool>(OperationCategory.SetValue, this.FullKeyName, delegate()
			{
				PropertyValue value = new PropertyValue(propertyValue, valueKind);
				bool result = false;
				try
				{
					DxStoreAccessRequest.SetProperty setProperty = this.CreateRequest<DxStoreAccessRequest.SetProperty>();
					setProperty.Name = propertyName;
					setProperty.Value = value;
					setProperty.WriteOptions = writeOptions;
					DxStoreAccessReply.SetProperty setProperty2 = this.BaseParameters.Client.SetProperty(setProperty, null);
					this.SetWriteResult(constraints, setProperty2.WriteResult);
					result = true;
				}
				catch
				{
					if (!isBestEffort)
					{
						throw;
					}
				}
				return result;
			}, false);
		}

		public object GetValue(string propertyName, out bool isValueExist, out RegistryValueKind valueKind, ReadWriteConstraints constraints)
		{
			ReadOptions readOptions = this.GetReadOptions(constraints);
			isValueExist = false;
			valueKind = RegistryValueKind.Unknown;
			PropertyValue propertyValue = this.BaseParameters.KeyFactory.RunOperationAndTranslateException<PropertyValue>(OperationCategory.GetValue, this.FullKeyName, delegate()
			{
				DxStoreAccessRequest.GetProperty getProperty = this.CreateRequest<DxStoreAccessRequest.GetProperty>();
				getProperty.Name = propertyName;
				getProperty.ReadOptions = readOptions;
				DxStoreAccessReply.GetProperty property = this.BaseParameters.Client.GetProperty(getProperty, null);
				this.SetReadResult(constraints, property.ReadResult);
				return property.Value;
			}, false);
			object result = null;
			if (propertyValue != null)
			{
				isValueExist = true;
				result = propertyValue.Value;
				valueKind = (RegistryValueKind)propertyValue.Kind;
			}
			return result;
		}

		public bool DeleteValue(string propertyName, bool isIgnoreIfNotExist, ReadWriteConstraints constraints)
		{
			return this.BaseParameters.KeyFactory.RunOperationAndTranslateException<bool>(OperationCategory.DeleteValue, this.FullKeyName, delegate()
			{
				ReadOptions readOptions = this.GetReadOptions(constraints);
				WriteOptions writeOptions = this.GetWriteOptions(constraints);
				DxStoreAccessRequest.DeleteProperty deleteProperty = this.CreateRequest<DxStoreAccessRequest.DeleteProperty>();
				deleteProperty.Name = propertyName;
				deleteProperty.ReadOptions = readOptions;
				deleteProperty.WriteOptions = writeOptions;
				DxStoreAccessReply.DeleteProperty deleteProperty2 = this.BaseParameters.Client.DeleteProperty(deleteProperty, null);
				this.SetReadResult(constraints, deleteProperty2.ReadResult);
				this.SetWriteResult(constraints, deleteProperty2.WriteResult);
				if (!deleteProperty2.IsExist && !isIgnoreIfNotExist)
				{
					throw new DxStorePropertyNotFoundException(propertyName);
				}
				return deleteProperty2.IsExist;
			}, false);
		}

		public IEnumerable<string> GetSubkeyNames(ReadWriteConstraints constraints)
		{
			ReadOptions readOptions = this.GetReadOptions(constraints);
			return this.BaseParameters.KeyFactory.RunOperationAndTranslateException<string[]>(OperationCategory.GetSubKeyNames, this.FullKeyName, delegate()
			{
				DxStoreAccessRequest.GetSubkeyNames getSubkeyNames = this.CreateRequest<DxStoreAccessRequest.GetSubkeyNames>();
				getSubkeyNames.ReadOptions = readOptions;
				DxStoreAccessReply.GetSubkeyNames subkeyNames = this.BaseParameters.Client.GetSubkeyNames(getSubkeyNames, null);
				this.SetReadResult(constraints, subkeyNames.ReadResult);
				return subkeyNames.Keys;
			}, false);
		}

		public PropertyNameInfo[] GetPropertyNameInfos(ReadWriteConstraints constraints)
		{
			ReadOptions readOptions = this.GetReadOptions(constraints);
			return this.BaseParameters.KeyFactory.RunOperationAndTranslateException<PropertyNameInfo[]>(OperationCategory.GetSubKeyNames, this.FullKeyName, delegate()
			{
				DxStoreAccessRequest.GetPropertyNames getPropertyNames = this.CreateRequest<DxStoreAccessRequest.GetPropertyNames>();
				getPropertyNames.ReadOptions = readOptions;
				DxStoreAccessReply.GetPropertyNames propertyNames = this.BaseParameters.Client.GetPropertyNames(getPropertyNames, null);
				this.SetReadResult(constraints, propertyNames.ReadResult);
				return propertyNames.Infos;
			}, false);
		}

		public IEnumerable<string> GetValueNames(ReadWriteConstraints constraints)
		{
			PropertyNameInfo[] propertyNameInfos = this.GetPropertyNameInfos(constraints);
			if (propertyNameInfos != null)
			{
				return (from pni in propertyNameInfos
				select pni.Name).ToArray<string>();
			}
			return Utils.EmptyArray<string>();
		}

		public IEnumerable<Tuple<string, RegistryValueKind>> GetValueInfos(ReadWriteConstraints constraints)
		{
			PropertyNameInfo[] propertyNameInfos = this.GetPropertyNameInfos(constraints);
			if (propertyNameInfos != null)
			{
				return (from pni in propertyNameInfos
				select new Tuple<string, RegistryValueKind>(pni.Name, (RegistryValueKind)pni.Kind)).ToArray<Tuple<string, RegistryValueKind>>();
			}
			return Utils.EmptyArray<Tuple<string, RegistryValueKind>>();
		}

		public IEnumerable<Tuple<string, object>> GetAllValues(ReadWriteConstraints constraints)
		{
			List<Tuple<string, object>> list = new List<Tuple<string, object>>();
			ReadOptions readOptions = this.GetReadOptions(constraints);
			return this.BaseParameters.KeyFactory.RunOperationAndTranslateException<List<Tuple<string, object>>>(OperationCategory.GetAllValues, this.FullKeyName, delegate()
			{
				DxStoreAccessRequest.GetAllProperties getAllProperties = this.CreateRequest<DxStoreAccessRequest.GetAllProperties>();
				getAllProperties.ReadOptions = readOptions;
				DxStoreAccessReply.GetAllProperties allProperties = this.BaseParameters.Client.GetAllProperties(getAllProperties, null);
				this.SetReadResult(constraints, allProperties.ReadResult);
				foreach (Tuple<string, PropertyValue> tuple in allProperties.Values)
				{
					string item = tuple.Item1;
					object item2 = (tuple.Item2 != null) ? tuple.Item2.Value : null;
					list.Add(new Tuple<string, object>(item, item2));
				}
				return list;
			}, false);
		}

		public IDistributedStoreBatchRequest CreateBatchUpdateRequest()
		{
			return new GenericBatchRequest(this);
		}

		public void ExecuteBatchRequest(List<DxStoreBatchCommand> commands, ReadWriteConstraints constraints)
		{
			WriteOptions writeOptions = this.GetWriteOptions(constraints);
			this.BaseParameters.KeyFactory.RunOperationAndTranslateException(OperationCategory.ExecuteBatch, this.FullKeyName, delegate()
			{
				DxStoreAccessRequest.ExecuteBatch executeBatch = new DxStoreAccessRequest.ExecuteBatch
				{
					Commands = commands.ToArray(),
					WriteOptions = writeOptions
				};
				executeBatch.Initialize(this.FullKeyName, this.BaseParameters.IsPrivate, this.BaseParameters.Self);
				DxStoreAccessReply.ExecuteBatch executeBatch2 = this.BaseParameters.Client.ExecuteBatch(executeBatch, null);
				this.SetWriteResult(constraints, executeBatch2.WriteResult);
			});
		}

		public IDistributedStoreChangeNotify CreateChangeNotify(ChangeNotificationFlags flags, object context, Action callback)
		{
			this.BaseParameters.KeyFactory.RunOperationAndTranslateException(OperationCategory.CreateChangeNotify, this.FullKeyName, delegate()
			{
				throw new NotImplementedException();
			});
			return null;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
			}
		}

		private ReadOptions GetReadOptions(ReadWriteConstraints rwc)
		{
			if (rwc == null || rwc.ReadOptions == null)
			{
				return this.BaseParameters.DefaultReadOptions;
			}
			return rwc.ReadOptions;
		}

		private WriteOptions GetWriteOptions(ReadWriteConstraints rwc)
		{
			if (rwc == null || rwc.WriteOptions == null)
			{
				return this.BaseParameters.DefaultWriteOptions;
			}
			return rwc.WriteOptions;
		}

		private void SetReadResult(ReadWriteConstraints rwc, ReadResult result)
		{
			if (rwc != null)
			{
				rwc.ReadResult = result;
			}
		}

		private void SetWriteResult(ReadWriteConstraints rwc, WriteResult result)
		{
			if (rwc != null)
			{
				rwc.WriteResult = result;
			}
		}

		private bool isDisposed;

		public class BaseKeyParameters
		{
			public IDxStoreAccessClient Client { get; set; }

			public DxStoreKeyFactory KeyFactory { get; set; }

			public ReadOptions DefaultReadOptions { get; set; }

			public WriteOptions DefaultWriteOptions { get; set; }

			public string Self { get; set; }

			public bool IsPrivate { get; set; }
		}
	}
}
