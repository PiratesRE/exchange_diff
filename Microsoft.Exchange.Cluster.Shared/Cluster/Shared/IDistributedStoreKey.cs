using System;
using System.Collections.Generic;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public interface IDistributedStoreKey : IDisposable
	{
		IDistributedStoreKey OpenKey(string keyName, DxStoreKeyAccessMode mode = DxStoreKeyAccessMode.Read, bool isIgnoreIfNotExist = false, ReadWriteConstraints constraints = null);

		void CloseKey();

		bool DeleteKey(string keyName, bool isIgnoreIfNotExist = true, ReadWriteConstraints constraints = null);

		bool SetValue(string propertyName, object propertyValue, RegistryValueKind valueKind, bool isBestEffort = false, ReadWriteConstraints constraints = null);

		object GetValue(string propertyName, out bool isValueExist, out RegistryValueKind valueKind, ReadWriteConstraints constraints = null);

		IEnumerable<Tuple<string, object>> GetAllValues(ReadWriteConstraints constraints = null);

		bool DeleteValue(string propertyValue, bool isIgnoreIfNotExist = true, ReadWriteConstraints constraints = null);

		IEnumerable<string> GetSubkeyNames(ReadWriteConstraints constraints = null);

		IEnumerable<string> GetValueNames(ReadWriteConstraints constraints = null);

		IEnumerable<Tuple<string, RegistryValueKind>> GetValueInfos(ReadWriteConstraints constraints = null);

		IDistributedStoreBatchRequest CreateBatchUpdateRequest();

		void ExecuteBatchRequest(List<DxStoreBatchCommand> commands, ReadWriteConstraints constraints);

		IDistributedStoreChangeNotify CreateChangeNotify(ChangeNotificationFlags flags, object context, Action callback);
	}
}
