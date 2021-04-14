using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.DxStore.Server
{
	public class SnapshotManager
	{
		public SnapshotManager(DxStoreInstance instance)
		{
			this.instance = instance;
			this.SnapshotFileNameFullPath = this.GetSnapshotFileName();
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.SnapshotTracer;
			}
		}

		public string SnapshotFileNameFullPath { get; set; }

		public bool IsInitialLoadAttempted { get; set; }

		public void InitializeDataStore()
		{
			this.instance.RunBestEffortOperation("LoadSnapshot", delegate
			{
				this.InitializeDataStoreInternal();
			}, LogOptions.LogAll, null, null, null, null);
			this.IsInitialLoadAttempted = true;
		}

		public void Start()
		{
			lock (this.locker)
			{
				if (this.timer == null)
				{
					this.timer = new GuardedTimer(delegate(object unused)
					{
						this.SaveSnapshotCallback();
					}, null, TimeSpan.Zero, this.instance.GroupConfig.Settings.SnapshotUpdateInterval);
				}
			}
		}

		public void Stop()
		{
			lock (this.locker)
			{
				if (this.timer != null)
				{
					this.timer.Dispose(true);
					this.timer = null;
				}
			}
		}

		public void ApplySnapshot(InstanceSnapshotInfo snapshotInfo, bool isTakeSnapshot = true)
		{
			lock (this.locker)
			{
				this.instance.LocalDataStore.ApplySnapshot(snapshotInfo, null);
				if (isTakeSnapshot)
				{
					this.SaveSnapshot(true);
				}
			}
		}

		public void ForceFlush()
		{
			this.SaveSnapshot(true);
		}

		public void SaveSnapshotCallback()
		{
			Utils.RunOperation(this.instance.Identity, "SaveSnapshot", delegate
			{
				this.SaveSnapshot(false);
			}, this.instance.EventLogger, LogOptions.LogException | this.instance.GroupConfig.Settings.AdditionalLogOptions, true, null, null, null, null, null);
		}

		private void InitializeDataStoreInternal()
		{
			int num = 0;
			XElement xelementFromSnapshot = this.GetXElementFromSnapshot(this.SnapshotFileNameFullPath);
			if (xelementFromSnapshot != null)
			{
				XAttribute xattribute = xelementFromSnapshot.Attribute("LastInstanceExecuted");
				if (xattribute != null)
				{
					num = int.Parse(xattribute.Value);
				}
				XElement xelement = xelementFromSnapshot.Elements().FirstOrDefault<XElement>();
				if (xelement != null)
				{
					SnapshotManager.Tracer.Information<string, int>((long)this.instance.IdentityHash, "{0}: Startup - Applying snapshot to local store (LastInstanceNumber: {1})", this.instance.Identity, num);
					this.instance.LocalDataStore.ApplySnapshotFromXElement("\\", num, xelement);
				}
			}
		}

		private void SaveSnapshot(bool isForce = false)
		{
			DataStoreStats storeStats = this.instance.LocalDataStore.GetStoreStats();
			if (isForce || this.lastRecordedStoreStats == null || storeStats.LastUpdateNumber > this.lastRecordedStoreStats.LastUpdateNumber || storeStats.LastUpdateTime > this.lastRecordedStoreStats.LastUpdateTime)
			{
				SnapshotManager.Tracer.Information<string, int, DateTimeOffset>((long)this.instance.IdentityHash, "{0}: Attempting to take snapshot (RecentUpdate: {1}, RecentUpdateTime: {2})", this.instance.Identity, storeStats.LastUpdateNumber, storeStats.LastUpdateTime);
				this.lastRecordedStoreStats = storeStats.Clone();
				this.CreateSnapshotDirectoryIfRequired();
				int num;
				XElement xelementSnapshot = this.instance.LocalDataStore.GetXElementSnapshot(null, out num);
				XElement xelement = new XElement("SnapshotRoot", new XAttribute("LastInstanceExecuted", num));
				xelement.Add(xelementSnapshot);
				string text = xelement.ToString();
				SnapshotManager.Tracer.Information<string, int, string>((long)this.instance.IdentityHash, "{0}: Writing {1} chars to {2}", this.instance.Identity, text.Length, this.SnapshotFileNameFullPath);
				File.WriteAllText(this.SnapshotFileNameFullPath, text, Encoding.UTF8);
				return;
			}
			SnapshotManager.Tracer.Information<string, int, DateTimeOffset>((long)this.instance.IdentityHash, "{0}: Skipped saving snapshot since there are no changes observed (LastUpdate: {1}, LastUpdateTime: {2})", this.instance.Identity, storeStats.LastUpdateNumber, storeStats.LastUpdateTime);
		}

		private void CreateSnapshotDirectoryIfRequired()
		{
			if (!Directory.Exists(this.instance.GroupConfig.Settings.SnapshotStorageDir))
			{
				Directory.CreateDirectory(this.instance.GroupConfig.Settings.SnapshotStorageDir);
			}
		}

		private XElement GetXElementFromSnapshot(string fileName)
		{
			Exception ex = null;
			if (!File.Exists(fileName))
			{
				return null;
			}
			XElement result = null;
			try
			{
				string text = File.ReadAllText(fileName, Encoding.UTF8);
				result = XElement.Parse(text);
			}
			catch (XmlException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				SnapshotManager.Tracer.TraceError<string, string, string>((long)this.instance.IdentityHash, "{0}: Parse/read of {1} failed with {2}", this.instance.Identity, this.SnapshotFileNameFullPath, ex.Message);
				this.instance.EventLogger.Log(DxEventSeverity.Error, 0, "{0}: Failed to read snapshot from {1} (error: {2})", new object[]
				{
					this.instance.Identity,
					this.SnapshotFileNameFullPath,
					ex.Message
				});
			}
			return result;
		}

		private string GetSnapshotFileName()
		{
			return Utils.CombinePathNullSafe(this.instance.GroupConfig.Settings.SnapshotStorageDir, this.instance.GroupConfig.Settings.DefaultSnapshotFileName);
		}

		public const string LastInstanceExecutedAttribute = "LastInstanceExecuted";

		private readonly object locker = new object();

		private readonly DxStoreInstance instance;

		private DataStoreStats lastRecordedStoreStats;

		private GuardedTimer timer;
	}
}
