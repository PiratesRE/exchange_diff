using System;
using System.Collections.Generic;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class GenericBatchRequest : IDistributedStoreBatchRequest, IDisposable
	{
		public GenericBatchRequest(IDistributedStoreKey key)
		{
			this.key = key;
			this.Commands = new List<DxStoreBatchCommand>();
		}

		public List<DxStoreBatchCommand> Commands { get; private set; }

		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			this.IsDisposed = true;
		}

		public void CreateKey(string keyName)
		{
			DxStoreBatchCommand.CreateKey item = new DxStoreBatchCommand.CreateKey
			{
				Name = keyName
			};
			this.Commands.Add(item);
		}

		public void DeleteKey(string keyName)
		{
			DxStoreBatchCommand.DeleteKey item = new DxStoreBatchCommand.DeleteKey
			{
				Name = keyName
			};
			this.Commands.Add(item);
		}

		public void SetValue(string propertyName, object propertyValue, RegistryValueKind valueKind = RegistryValueKind.Unknown)
		{
			DxStoreBatchCommand.SetProperty item = new DxStoreBatchCommand.SetProperty
			{
				Name = propertyName,
				Value = new PropertyValue(propertyValue, valueKind)
			};
			this.Commands.Add(item);
		}

		public void DeleteValue(string propertyName)
		{
			DxStoreBatchCommand.DeleteProperty item = new DxStoreBatchCommand.DeleteProperty
			{
				Name = propertyName
			};
			this.Commands.Add(item);
		}

		public void Execute(ReadWriteConstraints constraints)
		{
			this.key.ExecuteBatchRequest(this.Commands, constraints);
		}

		private IDistributedStoreKey key;
	}
}
