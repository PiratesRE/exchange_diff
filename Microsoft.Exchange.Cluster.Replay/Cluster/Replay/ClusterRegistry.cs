using System;
using System.Linq;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ClusterRegistry : RegistryManipulator
	{
		public ClusterRegistry(string root, AmClusterHandle handle) : base(root, handle)
		{
			bool flag = true;
			try
			{
				this.SetClusterHandle(handle);
				flag = false;
			}
			finally
			{
				if (flag)
				{
					base.SuppressDisposeTracker();
				}
			}
		}

		public void SetClusterHandle(AmClusterHandle handle)
		{
			if (this.rootKey != null)
			{
				this.rootKey.Dispose();
				this.rootKey = null;
			}
			this.Handle = null;
			using (IDistributedStoreKey clusterKey = DistributedStore.Instance.GetClusterKey(handle, null, null, DxStoreKeyAccessMode.Write, false))
			{
				this.rootKey = clusterKey.OpenKey(this.Root, DxStoreKeyAccessMode.CreateIfNotExist, false, null);
				this.Handle = handle;
			}
		}

		public T RunActionWithKey<T>(string keyName, Func<IDistributedStoreKey, T> action, DxStoreKeyAccessMode mode = DxStoreKeyAccessMode.Read)
		{
			IDistributedStoreKey distributedStoreKey = string.IsNullOrEmpty(keyName) ? this.rootKey : this.rootKey.OpenKey(keyName, mode, false, null);
			T result;
			try
			{
				T t = action(distributedStoreKey);
				result = t;
			}
			finally
			{
				if (distributedStoreKey != null && !object.ReferenceEquals(distributedStoreKey, this.rootKey))
				{
					distributedStoreKey.Dispose();
				}
			}
			return result;
		}

		public override string[] GetSubKeyNames(string keyName)
		{
			return this.RunActionWithKey<string[]>(keyName, (IDistributedStoreKey key) => key.GetSubkeyNames(null).ToArray<string>(), DxStoreKeyAccessMode.Read);
		}

		public override string[] GetValueNames(string keyName)
		{
			return this.RunActionWithKey<string[]>(keyName, (IDistributedStoreKey key) => key.GetValueNames(null).ToArray<string>(), DxStoreKeyAccessMode.Read);
		}

		public override void SetValue(string keyName, RegistryValue value)
		{
			this.RunActionWithKey<bool>(keyName, (IDistributedStoreKey regKey) => regKey.SetValue(value.Name, value.Value, value.Kind, false, null), DxStoreKeyAccessMode.Write);
		}

		public override RegistryValue GetValue(string keyName, string valueName)
		{
			return this.RunActionWithKey<RegistryValue>(keyName, delegate(IDistributedStoreKey regKey)
			{
				RegistryValue result = null;
				bool flag = false;
				RegistryValueKind kind;
				object value = regKey.GetValue(valueName, out flag, out kind, null);
				if (flag && value != null)
				{
					result = new RegistryValue(valueName, value, kind);
				}
				return result;
			}, DxStoreKeyAccessMode.Read);
		}

		public override void DeleteValue(string keyName, string valueName)
		{
			this.RunActionWithKey<bool>(keyName, (IDistributedStoreKey regKey) => regKey.DeleteValue(valueName, true, null), DxStoreKeyAccessMode.Write);
		}

		public override void DeleteKey(string keyName)
		{
			this.RunActionWithKey<bool>(string.Empty, (IDistributedStoreKey regKey) => regKey.DeleteKey(keyName, true, null), DxStoreKeyAccessMode.Write);
		}

		public override void CreateKey(string keyName)
		{
			using (this.rootKey.OpenKey(keyName, DxStoreKeyAccessMode.CreateIfNotExist, false, null))
			{
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ClusterRegistry>(this);
		}

		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && this.rootKey != null)
			{
				this.rootKey.Dispose();
				this.rootKey = null;
			}
			base.Dispose();
		}

		private const int BufferSize = 128;

		private IDistributedStoreKey rootKey;
	}
}
