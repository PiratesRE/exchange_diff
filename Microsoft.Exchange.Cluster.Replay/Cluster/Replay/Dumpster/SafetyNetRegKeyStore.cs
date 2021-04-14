using System;
using System.Linq;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal class SafetyNetRegKeyStore : DisposeTrackableBase
	{
		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SafetyNetRegKeyStore>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this)
			{
				if (disposing && this.m_dbRegKeyHandle != null)
				{
					this.m_dbRegKeyHandle.Dispose();
					this.m_dbRegKeyHandle = null;
				}
			}
		}

		public SafetyNetRegKeyStore(string dbGuidStr, string dbName)
		{
			this.m_dbGuidStr = dbGuidStr;
			this.m_dbName = dbName;
		}

		public string[] ReadRequestKeyNames()
		{
			this.EnsureOpen();
			return this.m_dbRegKeyHandle.GetValueNames(null).ToArray<string>();
		}

		public SafetyNetInfo ReadRequestInfo(SafetyNetRequestKey requestKey, SafetyNetInfo prevInfo)
		{
			this.EnsureOpen();
			string valueName = requestKey.ToString();
			string text = this.ReadString(valueName);
			SafetyNetInfo safetyNetInfo = null;
			if (prevInfo != null && SharedHelper.StringIEquals(text, prevInfo.GetSerializedForm()))
			{
				safetyNetInfo = prevInfo;
			}
			if (safetyNetInfo == null && !string.IsNullOrEmpty(text))
			{
				safetyNetInfo = SafetyNetInfo.Deserialize(this.m_dbName, text, ExTraceGlobals.DumpsterTracer, true);
			}
			return safetyNetInfo;
		}

		public void WriteRequestInfo(SafetyNetInfo info)
		{
			this.EnsureOpen();
			SafetyNetRequestKey safetyNetRequestKey = new SafetyNetRequestKey(info);
			string valueName = safetyNetRequestKey.ToString();
			string value = info.Serialize();
			this.WriteString(valueName, value);
		}

		public void DeleteRequest(SafetyNetInfo info)
		{
			this.EnsureOpen();
			SafetyNetRequestKey safetyNetRequestKey = new SafetyNetRequestKey(info);
			string valueName = safetyNetRequestKey.ToString();
			this.DeleteValue(valueName);
		}

		private void EnsureOpen()
		{
			if (!this.m_open)
			{
				this.Open();
				this.m_open = true;
			}
		}

		private void Open()
		{
			AmClusterHandle amClusterHandle = null;
			AmSystemManager instance = AmSystemManager.Instance;
			if (instance != null)
			{
				AmConfig config = instance.Config;
				if (config != null)
				{
					AmDagConfig dagConfig = config.DagConfig;
					if (dagConfig != null)
					{
						IAmCluster cluster = dagConfig.Cluster;
						if (cluster != null)
						{
							amClusterHandle = cluster.Handle;
						}
					}
				}
			}
			if (amClusterHandle == null || amClusterHandle.IsInvalid)
			{
				throw new AmClusterNotRunningException();
			}
			using (IDistributedStoreKey clusterKey = DistributedStore.Instance.GetClusterKey(amClusterHandle, null, null, DxStoreKeyAccessMode.Write, false))
			{
				string keyName = string.Format("{0}\\SafetyNet2\\{1}", "ExchangeActiveManager", this.m_dbGuidStr);
				this.m_dbRegKeyHandle = clusterKey.OpenKey(keyName, DxStoreKeyAccessMode.CreateIfNotExist, false, null);
			}
		}

		private void WriteString(string valueName, string value)
		{
			this.EnsureOpen();
			this.m_dbRegKeyHandle.SetValue(valueName, value, false, null);
		}

		private string ReadString(string valueName)
		{
			this.EnsureOpen();
			return this.m_dbRegKeyHandle.GetValue(valueName, null, null);
		}

		private void DeleteValue(string valueName)
		{
			this.EnsureOpen();
			this.m_dbRegKeyHandle.DeleteValue(valueName, true, null);
		}

		private const string SafetyNetKeyFormatStr = "{0}\\SafetyNet2\\{1}";

		private readonly string m_dbGuidStr;

		private readonly string m_dbName;

		private bool m_open;

		private IDistributedStoreKey m_dbRegKeyHandle;
	}
}
