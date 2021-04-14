using System;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Servicelets.RPCHTTP
{
	internal sealed class VirtualDirectoryConfiguration : DisposeTrackableBase
	{
		public VirtualDirectoryConfiguration(RpcHandlerMode rpcHandlerMode)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.serverManager = new ServerManager();
				this.rpcHandlerMode = rpcHandlerMode;
				if (this.serverManager.Sites.Count == 0)
				{
					throw new WebSitesNotConfiguredException();
				}
				this.defaultWebSiteName = this.serverManager.Sites[0].Name;
				this.applicationHostConfig = this.serverManager.GetApplicationHostConfiguration();
				disposeGuard.Success();
			}
		}

		public static string BackEndWebSiteName
		{
			get
			{
				return "Exchange Back End";
			}
		}

		public string DefaultWebSiteName
		{
			get
			{
				return this.defaultWebSiteName;
			}
		}

		private bool ShouldUseCertificateSettings(string webSiteName, RpcVirtualDirectoryName rpcVirtualDirectoryName)
		{
			return string.Equals(webSiteName, this.DefaultWebSiteName, StringComparison.OrdinalIgnoreCase) && rpcVirtualDirectoryName == RpcVirtualDirectoryName.RpcWithCert;
		}

		public void ConfigureRpc(string webSiteName, RpcVirtualDirectoryName rpcVirtualDirectoryName)
		{
			string virtualDirectoryPath = VirtualDirectoryConfiguration.GetVirtualDirectoryPath(rpcVirtualDirectoryName);
			bool useCertificateSettings = this.ShouldUseCertificateSettings(webSiteName, rpcVirtualDirectoryName);
			this.FixRpcAppPool();
			this.ConfigureSiteApplication(webSiteName, virtualDirectoryPath);
			this.ConfigureLocationSettings(webSiteName, virtualDirectoryPath, useCertificateSettings);
		}

		public void ConfigureRpcSecurity(string webSiteName, RpcVirtualDirectoryName rpcVirtualDirectoryName, VirtualDirectorySecuritySettings securitySettings)
		{
			this.ConfigureSecuritySettings(webSiteName, VirtualDirectoryConfiguration.GetVirtualDirectoryPath(rpcVirtualDirectoryName), securitySettings);
		}

		public void RemoveRpc(string webSiteName, RpcVirtualDirectoryName rpcVirtualDirectoryName)
		{
			this.RemoveSiteApplication(webSiteName, VirtualDirectoryConfiguration.GetVirtualDirectoryPath(rpcVirtualDirectoryName));
			this.RemoveLocationSettings(webSiteName, VirtualDirectoryConfiguration.GetVirtualDirectoryPath(rpcVirtualDirectoryName));
		}

		public bool Commit()
		{
			bool result = this.isDirty;
			if (this.isDirty)
			{
				this.serverManager.CommitChanges();
				this.isDirty = false;
			}
			if (!this.isRpcAppPoolNew && this.recycleRpcAppPool)
			{
				string applicationPoolName = this.GetApplicationPoolName();
				ApplicationPoolCollection applicationPools = this.serverManager.ApplicationPools;
				ApplicationPool applicationPool = applicationPools[applicationPoolName];
				if (applicationPool == null)
				{
					throw new InvalidOperationException(applicationPoolName + " not found!");
				}
				applicationPool.Recycle();
				this.isRpcAppPoolNew = false;
				this.recycleRpcAppPool = false;
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				DisposeGuard.DisposeIfPresent(this.serverManager);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<VirtualDirectoryConfiguration>(this);
		}

		private string GetApplicationPhysicalPath()
		{
			switch (this.rpcHandlerMode)
			{
			case RpcHandlerMode.RpcProxy:
				return "%windir%\\System32\\RpcProxy";
			case RpcHandlerMode.HttpProxy:
				return Path.Combine(ConfigurationContext.Setup.InstallPath, "FrontEnd\\HttpProxy\\rpc");
			default:
				throw new InvalidOperationException("Unrecognized handler mode: " + this.rpcHandlerMode);
			}
		}

		private string GetClrConfigFilePath()
		{
			switch (this.rpcHandlerMode)
			{
			case RpcHandlerMode.RpcProxy:
				return Path.Combine(ConfigurationContext.Setup.InstallPath, "bin\\MSExchangeRpcProxyAppPool_CLRConfig.config");
			case RpcHandlerMode.HttpProxy:
				return Path.Combine(ConfigurationContext.Setup.InstallPath, "bin\\MSExchangeRpcProxyFrontEndAppPool_CLRConfig.config");
			default:
				throw new InvalidOperationException("Unrecognized handler mode: " + this.rpcHandlerMode);
			}
		}

		private string GetApplicationPoolName()
		{
			switch (this.rpcHandlerMode)
			{
			case RpcHandlerMode.RpcProxy:
				return "MSExchangeRpcProxyAppPool";
			case RpcHandlerMode.HttpProxy:
				return "MSExchangeRpcProxyFrontEndAppPool";
			default:
				throw new InvalidOperationException("Unrecognized handler mode: " + this.rpcHandlerMode);
			}
		}

		private static string GetVirtualDirectoryPath(RpcVirtualDirectoryName rpcVirtualDirectoryName)
		{
			switch (rpcVirtualDirectoryName)
			{
			case RpcVirtualDirectoryName.Rpc:
				return "/Rpc";
			case RpcVirtualDirectoryName.RpcWithCert:
				return "/RpcWithCert";
			default:
				throw new InvalidOperationException("Unrecognized virtual directory type: " + rpcVirtualDirectoryName);
			}
		}

		private ConfigurationElement GetSiteElement(string siteName)
		{
			ConfigurationSection section = this.applicationHostConfig.GetSection("system.applicationHost/sites");
			ConfigurationElementCollection collection = section.GetCollection();
			ConfigurationElement configurationElement = IISConfigurationUtilities.FindElement(collection, "site", "name", siteName);
			if (configurationElement == null)
			{
				throw new WebSiteNotFoundException(siteName);
			}
			return configurationElement;
		}

		private void FixRpcAppPool()
		{
			string applicationPoolName = this.GetApplicationPoolName();
			string clrConfigFilePath = this.GetClrConfigFilePath();
			bool flag = IISConfigurationUtilities.CreateOrUpdateApplicationPool(this.serverManager, applicationPoolName, clrConfigFilePath, out this.isRpcAppPoolNew);
			this.recycleRpcAppPool = (this.recycleRpcAppPool || flag);
			this.isDirty = (this.isDirty || flag);
		}

		private void ConfigureSiteApplication(string webSiteName, string path)
		{
			ConfigurationElement siteElement = this.GetSiteElement(webSiteName);
			ConfigurationElementCollection collection = siteElement.GetCollection();
			ConfigurationElement configurationElement = IISConfigurationUtilities.FindElement(collection, "application", "path", path);
			if (configurationElement == null)
			{
				configurationElement = collection.CreateElement("application");
				configurationElement["path"] = path;
				collection.Add(configurationElement);
				this.isDirty = true;
			}
			string applicationPoolName = this.GetApplicationPoolName();
			this.recycleRpcAppPool |= IISConfigurationUtilities.UpdateElementAttribute(configurationElement, "applicationPool", applicationPoolName);
			this.isDirty |= this.recycleRpcAppPool;
			ConfigurationElementCollection collection2 = configurationElement.GetCollection();
			ConfigurationElement configurationElement2 = IISConfigurationUtilities.FindElement(collection2, "virtualDirectory", "path", "/");
			if (configurationElement2 == null)
			{
				configurationElement2 = collection2.CreateElement("virtualDirectory");
				configurationElement2["path"] = "/";
				collection2.Add(configurationElement2);
				this.isDirty = true;
			}
			this.isDirty |= IISConfigurationUtilities.UpdateElementAttribute(configurationElement2, "physicalPath", this.GetApplicationPhysicalPath());
		}

		private void ConfigureLocationSettings(string webSiteName, string path, bool useCertificateSettings)
		{
			string text = webSiteName + path;
			ConfigurationSection section = this.applicationHostConfig.GetSection("system.webServer/serverRuntime", text);
			if (string.Compare(webSiteName, this.DefaultWebSiteName, true) == 0)
			{
				this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section, "appConcurrentRequestLimit", 120000);
			}
			else
			{
				this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section, "appConcurrentRequestLimit", 65535);
			}
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section, "uploadReadAheadSize", 0);
			ConfigurationSection section2 = this.applicationHostConfig.GetSection("system.webServer/directoryBrowse", text);
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section2, "enabled", false);
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section2, "showFlags", "Date, Time, Size, Extension");
			ConfigurationSection section3 = this.applicationHostConfig.GetSection("system.webServer/defaultDocument", text);
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section3, "enabled", true);
			ConfigurationSection section4 = this.applicationHostConfig.GetSection("system.webServer/security/requestFiltering", text);
			ConfigurationElement childElement = section4.GetChildElement("requestLimits");
			this.isDirty |= IISConfigurationUtilities.UpdateElementAttribute(childElement, "maxAllowedContentLength", 2147483648U);
			string text2 = null;
			if (this.rpcHandlerMode == RpcHandlerMode.RpcProxy)
			{
				text2 = "None";
			}
			if (useCertificateSettings)
			{
				text2 = "Ssl, SslNegotiateCert, SslRequireCert";
			}
			if (text2 != null)
			{
				ConfigurationSection section5 = this.applicationHostConfig.GetSection("system.webServer/security/access", text);
				this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section5, "sslFlags", text2);
			}
		}

		private void ConfigureSecuritySettings(string webSiteName, string path, VirtualDirectorySecuritySettings securitySettings)
		{
			string text = webSiteName + path;
			ConfigurationSection section = this.applicationHostConfig.GetSection("system.webServer/security/authentication/anonymousAuthentication", text);
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section, "enabled", securitySettings.Anonymous);
			ConfigurationSection section2 = this.applicationHostConfig.GetSection("system.webServer/security/authentication/basicAuthentication", text);
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section2, "enabled", securitySettings.Basic);
			ConfigurationSection section3 = this.applicationHostConfig.GetSection("system.webServer/security/authentication/clientCertificateMappingAuthentication", text);
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section3, "enabled", securitySettings.ClientCertificateMapping);
			ConfigurationSection section4 = this.applicationHostConfig.GetSection("system.webServer/security/authentication/digestAuthentication", text);
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section4, "enabled", securitySettings.Digest);
			ConfigurationSection section5 = this.applicationHostConfig.GetSection("system.webServer/security/authentication/iisClientCertificateMappingAuthentication", text);
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section5, "enabled", securitySettings.IisClientCertificateMapping);
			ConfigurationSection section6 = this.applicationHostConfig.GetSection("system.webServer/security/authentication/windowsAuthentication", text);
			this.isDirty |= IISConfigurationUtilities.UpdateSectionAttribute(section6, "enabled", securitySettings.Windows);
		}

		private void RemoveSiteApplication(string webSiteName, string path)
		{
			ConfigurationElement siteElement = this.GetSiteElement(webSiteName);
			ConfigurationElementCollection collection = siteElement.GetCollection();
			ConfigurationElement configurationElement = IISConfigurationUtilities.FindElement(collection, "application", "path", path);
			if (configurationElement != null)
			{
				collection.Remove(configurationElement);
				this.isDirty = true;
			}
		}

		private void RemoveLocationSettings(string webSiteName, string path)
		{
			string text = webSiteName + path;
			string[] locationPaths = this.applicationHostConfig.GetLocationPaths();
			if (locationPaths.Contains(text))
			{
				this.applicationHostConfig.RemoveLocationPath(text);
				this.isDirty = true;
			}
		}

		public const string RpcProxyFrontEndApplicationPoolName = "MSExchangeRpcProxyFrontEndAppPool";

		public const string RpcProxyApplicationPoolName = "MSExchangeRpcProxyAppPool";

		public const string RpcProxyClrConfigFilePath_Cafe = "bin\\MSExchangeRpcProxyFrontEndAppPool_CLRConfig.config";

		public const string RpcProxyClrConfigFilePath_Mailbox = "bin\\MSExchangeRpcProxyAppPool_CLRConfig.config";

		public const string RpcVirtualDirectoryPath = "/Rpc";

		public const string RpcWithCertVirtualDirectoryPath = "/RpcWithCert";

		public const string ExchangeBackEndWebSiteName = "Exchange Back End";

		private readonly string defaultWebSiteName;

		private readonly ServerManager serverManager;

		private readonly Configuration applicationHostConfig;

		private bool isDirty;

		private bool isRpcAppPoolNew;

		private bool recycleRpcAppPool;

		private RpcHandlerMode rpcHandlerMode;
	}
}
