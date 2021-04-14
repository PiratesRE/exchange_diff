using System;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public interface IDistributedStoreBatchRequest : IDisposable
	{
		void CreateKey(string keyName);

		void DeleteKey(string keyName);

		void SetValue(string propertyName, object propertyValue, RegistryValueKind valueKind = RegistryValueKind.Unknown);

		void DeleteValue(string propertyName);

		void Execute(ReadWriteConstraints constraints = null);
	}
}
