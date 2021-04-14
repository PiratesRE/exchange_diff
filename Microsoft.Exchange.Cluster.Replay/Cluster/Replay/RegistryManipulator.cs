using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class RegistryManipulator : IDisposeTrackable, IDisposable
	{
		protected RegistryManipulator(string root, SafeHandle handle)
		{
			this.Root = root;
			this.Handle = handle;
			this.m_disposed = false;
			this.m_disposeTracker = this.GetDisposeTracker();
		}

		public bool IsInitialReplication
		{
			get
			{
				RegistryValue registryValue = null;
				try
				{
					registryValue = this.GetValue(string.Empty, "InitialReplicationSucceeded");
				}
				catch (AmRegistryException)
				{
				}
				catch (ClusterApiException)
				{
				}
				return registryValue == null || (int)registryValue.Value == 0;
			}
		}

		public abstract string[] GetSubKeyNames(string keyName);

		public abstract string[] GetValueNames(string keyName);

		public abstract void SetValue(string keyName, RegistryValue value);

		public abstract RegistryValue GetValue(string keyName, string valueName);

		public abstract void DeleteValue(string keyName, string valueName);

		public abstract void DeleteKey(string keyName);

		public abstract void CreateKey(string keyName);

		public void SetInitialReplication()
		{
			this.SetValue(string.Empty, new RegistryValue("InitialReplicationSucceeded", 1, RegistryValueKind.DWord));
		}

		public virtual void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RegistryManipulator>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.m_disposed && disposing && this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Dispose();
			}
			this.m_disposed = true;
		}

		protected const string FirstMountValue = "InitialReplicationSucceeded";

		public string Root;

		public SafeHandle Handle;

		private DisposeTracker m_disposeTracker;

		private bool m_disposed;
	}
}
