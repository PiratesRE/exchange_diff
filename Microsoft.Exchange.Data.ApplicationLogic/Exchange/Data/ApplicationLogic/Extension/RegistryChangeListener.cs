using System;
using System.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class RegistryChangeListener : DisposeTrackableBase
	{
		public RegistryChangeListener(string registryKey, EventArrivedEventHandler eventArrivedEventHandler)
		{
			registryKey = registryKey.Replace("\\", "\\\\");
			string text = string.Format("SELECT * FROM RegistryKeyChangeEvent WHERE Hive = 'HKEY_LOCAL_MACHINE' AND KeyPath = '{0}'", registryKey);
			try
			{
				WqlEventQuery query = new WqlEventQuery(text);
				this.watcher = new ManagementEventWatcher(query);
				this.watcher.EventArrived += eventArrivedEventHandler;
				this.watcher.Start();
				RegistryChangeListener.Tracer.TraceDebug<string>(0L, "Registry Watcher Started With Query: {0}", text);
			}
			catch (ManagementException ex)
			{
				RegistryChangeListener.Tracer.TraceDebug<string, string>(0L, "An error occurred when setting registry listener. Query: {0}, Message: {1} ", text, ex.Message);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RegistryChangeListener>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (this.watcher == null)
			{
				return;
			}
			this.watcher.Stop();
			this.watcher.Dispose();
			this.watcher = null;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private ManagementEventWatcher watcher;
	}
}
