using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class ClusdbBatchRequest : IDisposable
	{
		public ClusdbBatchRequest(ClusterDbKey containerKey)
		{
			AmClusterBatchHandle amClusterBatchHandle = null;
			int num = ClusapiMethods.ClusterRegCreateBatch(containerKey.KeyHandle, out amClusterBatchHandle);
			if (num != 0 || amClusterBatchHandle.IsInvalid)
			{
				throw AmExceptionHelper.ConstructClusterApiException(num, "ClusterRegCreateBatch()", new object[0]);
			}
			this.batchHandle = amClusterBatchHandle;
			this.ContainerKey = containerKey;
		}

		public ClusterDbKey ContainerKey { get; private set; }

		public void CreateKey(string keyName)
		{
			int num = ClusapiMethods.ClusterRegBatchAddCommand(this.batchHandle, CLUSTER_REG_COMMAND.CLUSREG_CREATE_KEY, keyName, RegistryValueKind.Unknown, IntPtr.Zero, 0);
			if (num != 0)
			{
				throw AmExceptionHelper.ConstructClusterApiException(num, "ClusterRegBatchAddCommand(CLUSREG_CREATE_KEY)", new object[0]);
			}
			Interlocked.Increment(ref this.totalCommands);
		}

		public void DeleteKey(string keyName)
		{
			int num = ClusapiMethods.ClusterRegBatchAddCommand(this.batchHandle, CLUSTER_REG_COMMAND.CLUSREG_DELETE_KEY, keyName, RegistryValueKind.Unknown, IntPtr.Zero, 0);
			if (num != 0)
			{
				throw AmExceptionHelper.ConstructClusterApiException(num, "ClusterRegBatchAddCommand(CLUSREG_DELETE_KEY)", new object[0]);
			}
			Interlocked.Increment(ref this.totalCommands);
		}

		public void SetValue(string propertyName, object propertyValue, RegistryValueKind valueKind)
		{
			ClusdbMarshalledProperty clusdbMarshalledProperty = ClusdbMarshalledProperty.Create(propertyName, propertyValue, valueKind);
			if (clusdbMarshalledProperty == null)
			{
				throw new ClusterApiException("WfcDataStoreBatch.SetValue - property value is null", new ClusterUnsupportedRegistryTypeException("null"));
			}
			this.properties.Add(clusdbMarshalledProperty);
			int num = ClusapiMethods.ClusterRegBatchAddCommand(this.batchHandle, CLUSTER_REG_COMMAND.CLUSREG_SET_VALUE, clusdbMarshalledProperty.PropertyName, clusdbMarshalledProperty.ValueKind, clusdbMarshalledProperty.PropertyValueIntPtr, clusdbMarshalledProperty.PropertyValueSize);
			if (num != 0)
			{
				throw AmExceptionHelper.ConstructClusterApiException(num, "ClusterRegBatchAddCommand(CLUSREG_SET_VALUE)", new object[0]);
			}
			Interlocked.Increment(ref this.totalCommands);
		}

		public void DeleteValue(string propertyName)
		{
			int num = ClusapiMethods.ClusterRegBatchAddCommand(this.batchHandle, CLUSTER_REG_COMMAND.CLUSREG_DELETE_VALUE, propertyName, RegistryValueKind.Unknown, IntPtr.Zero, 0);
			if (num != 0)
			{
				throw AmExceptionHelper.ConstructClusterApiException(num, "ClusterRegBatchAddCommand(CLUSREG_DELETE_VALUE)", new object[0]);
			}
			Interlocked.Increment(ref this.totalCommands);
		}

		public void Execute(ReadWriteConstraints constraints)
		{
			bool flag = this.totalCommands > 0;
			if (flag && this.batchHandle != null && !this.batchHandle.IsInvalid)
			{
				try
				{
					int num = this.batchHandle.CommitAndClose();
					if (num != 0)
					{
						throw AmExceptionHelper.ConstructClusterApiException(num, "CommitAndClose()", new object[0]);
					}
				}
				finally
				{
					try
					{
						this.batchHandle.Dispose();
					}
					finally
					{
						this.batchHandle = null;
					}
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					try
					{
						if (this.batchHandle != null)
						{
							this.batchHandle.Dispose();
						}
					}
					finally
					{
						this.properties.ForEach(delegate(ClusdbMarshalledProperty prop)
						{
							prop.Dispose();
						});
					}
				}
				this.isDisposed = true;
			}
		}

		private AmClusterBatchHandle batchHandle;

		private int totalCommands;

		private List<ClusdbMarshalledProperty> properties = new List<ClusdbMarshalledProperty>();

		private bool isDisposed;
	}
}
