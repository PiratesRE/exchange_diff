using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.ResourceMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal sealed class ResourceManagerComponent : ITransportComponent, IDiagnosable
	{
		public ResourceManagerComponent(ResourceManagerResources resourcesToMonitor)
		{
			this.resourcesToMonitor = resourcesToMonitor;
		}

		public ResourceManager ResourceManager
		{
			get
			{
				if (this.resourceManager == null)
				{
					lock (this.syncLoad)
					{
						if (this.resourceManager == null)
						{
							ResourceManagerConfiguration resourceManagerConfiguration = new ResourceManagerConfiguration();
							resourceManagerConfiguration.Load();
							ResourceLog resourceLog = this.CreateResourceLog();
							this.resourceManager = new ResourceManager(resourceManagerConfiguration, new ResourceMonitorFactory(resourceManagerConfiguration), new ResourceManagerEventLogger(), new ResourceManagerComponentsAdapter(), this.resourcesToMonitor, resourceLog);
						}
					}
				}
				return this.resourceManager;
			}
		}

		void ITransportComponent.Load()
		{
			this.ResourceManager.Load();
		}

		void ITransportComponent.Unload()
		{
			this.ThrowIfNotLoaded();
			this.resourceManager = null;
		}

		string ITransportComponent.OnUnhandledException(Exception e)
		{
			if (this.resourceManager != null)
			{
				return this.resourceManager.ToString();
			}
			return "ResourceManagerComponent is not loaded.";
		}

		private void ThrowIfNotLoaded()
		{
			if (this.resourceManager == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Attempt to retrieve ResourceManager instance before ResourceManagerComponent is loaded.", new object[0]));
			}
		}

		private ResourceLog CreateResourceLog()
		{
			Server transportServer = Components.Configuration.LocalServer.TransportServer;
			bool enabled = transportServer.ResourceLogEnabled;
			string logDirectory = string.Empty;
			if (transportServer.ResourceLogPath == null || string.IsNullOrEmpty(transportServer.ResourceLogPath.PathName))
			{
				enabled = false;
			}
			else
			{
				logDirectory = transportServer.ResourceLogPath.PathName;
			}
			ResourceMeteringConfig resourceMeteringConfig = new ResourceMeteringConfig(8000, null);
			return new ResourceLog(enabled, "ResourceManager", logDirectory, transportServer.ResourceLogMaxAge, resourceMeteringConfig.ResourceLogFlushInterval, resourceMeteringConfig.ResourceLogBackgroundWriteInterval, (long)(transportServer.ResourceLogMaxDirectorySize.IsUnlimited ? 0UL : transportServer.ResourceLogMaxDirectorySize.Value.ToBytes()), (long)(transportServer.ResourceLogMaxFileSize.IsUnlimited ? 0UL : transportServer.ResourceLogMaxDirectorySize.Value.ToBytes()), resourceMeteringConfig.ResourceLogBufferSize);
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "ResourceManager";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = flag || parameters.Argument.IndexOf("basic", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = flag2 || parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag4 = !flag3 || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			string diagnosticComponentName = ((IDiagnosable)this).GetDiagnosticComponentName();
			XElement xelement = new XElement(diagnosticComponentName);
			if (flag4)
			{
				xelement.Add(new XElement("help", "Supported arguments: config, basic, verbose, help."));
			}
			if (flag3)
			{
				this.ResourceManager.AddDiagnosticInfoTo(xelement, flag2, flag);
			}
			return xelement;
		}

		private ResourceManager resourceManager;

		private object syncLoad = new object();

		private ResourceManagerResources resourcesToMonitor;
	}
}
