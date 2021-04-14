using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.IisTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Management
{
	internal static class IISConfigurationUtilities
	{
		internal static bool UpdateSectionAttribute(ConfigurationSection section, string attribute, object value)
		{
			object obj = section[attribute];
			section[attribute] = value;
			return !obj.Equals(section[attribute]);
		}

		internal static bool UpdateElementAttribute(ConfigurationElement element, string attribute, object value)
		{
			object obj = element[attribute];
			element[attribute] = value;
			return !obj.Equals(element[attribute]);
		}

		internal static ConfigurationElement FindElement(ConfigurationElementCollection collection, string elementTagName, string attribute, string value)
		{
			foreach (ConfigurationElement configurationElement in collection)
			{
				if (string.Equals(configurationElement.ElementTagName, elementTagName, StringComparison.OrdinalIgnoreCase))
				{
					object attributeValue = configurationElement.GetAttributeValue(attribute);
					if (attributeValue != null && string.Equals(value, attributeValue.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						return configurationElement;
					}
				}
			}
			return null;
		}

		internal static bool UpdateWebServerModule(ConfigurationElementCollection modulesCollection, string moduleName, string moduleType, bool shouldBeEnabled)
		{
			bool result = false;
			ConfigurationElement configurationElement = IISConfigurationUtilities.FindElement(modulesCollection, "add", "name", moduleName);
			if (shouldBeEnabled && configurationElement == null)
			{
				configurationElement = modulesCollection.CreateElement("add");
				configurationElement["name"] = moduleName;
				configurationElement["type"] = moduleType;
				modulesCollection.Add(configurationElement);
				result = true;
			}
			else if (!shouldBeEnabled && configurationElement != null)
			{
				modulesCollection.Remove(configurationElement);
				result = true;
			}
			return result;
		}

		internal static bool UpdateAuthenticationSettings(ServerManager serverManager, string siteName, string virtualPath, MultiValuedProperty<AuthenticationMethod> authMethods)
		{
			bool flag = IISConfigurationUtilities.UpdateAuthenticationSettingsAndProviders(serverManager, siteName, virtualPath, authMethods);
			return flag | IISConfigurationUtilities.UpdateAuthenticationModules(serverManager, siteName, virtualPath, authMethods, true);
		}

		internal static bool UpdateAuthenticationSettingsAndProviders(ServerManager serverManager, string siteName, string virtualPath, MultiValuedProperty<AuthenticationMethod> authMethods)
		{
			bool flag = false;
			string text = siteName + virtualPath;
			Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
			ConfigurationSection section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/anonymousAuthentication", text);
			flag |= IISConfigurationUtilities.UpdateSectionAttribute(section, "enabled", false);
			section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/basicAuthentication", text);
			bool flag2 = authMethods.Contains(AuthenticationMethod.Basic);
			flag |= IISConfigurationUtilities.UpdateSectionAttribute(section, "enabled", flag2);
			section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/clientCertificateMappingAuthentication", text);
			flag2 = authMethods.Contains(AuthenticationMethod.Certificate);
			flag |= IISConfigurationUtilities.UpdateSectionAttribute(section, "enabled", flag2);
			section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/digestAuthentication", text);
			flag2 = authMethods.Contains(AuthenticationMethod.Digest);
			flag |= IISConfigurationUtilities.UpdateSectionAttribute(section, "enabled", flag2);
			section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/iisClientCertificateMappingAuthentication", text);
			flag |= IISConfigurationUtilities.UpdateSectionAttribute(section, "enabled", false);
			section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/windowsAuthentication", text);
			bool flag3 = authMethods.Contains(AuthenticationMethod.Negotiate);
			bool flag4 = authMethods.Contains(AuthenticationMethod.Kerberos);
			bool flag5 = authMethods.Contains(AuthenticationMethod.Ntlm);
			bool flag6 = authMethods.Contains(AuthenticationMethod.LiveIdNegotiate);
			flag2 = (flag3 || flag4 || flag5 || flag6);
			flag |= IISConfigurationUtilities.UpdateSectionAttribute(section, "enabled", flag2);
			flag |= IISConfigurationUtilities.UpdateSectionAttribute(section, "useKernelMode", false);
			if (flag2)
			{
				ConfigurationElementCollection collection = section.GetCollection("providers");
				int num = (flag3 ? 1 : 0) + (flag4 ? 1 : 0) + (flag5 ? 1 : 0);
				bool flag7 = false;
				if (collection.Count != num)
				{
					flag7 = true;
				}
				else
				{
					int num2 = 0;
					if (flag6)
					{
						flag7 |= ((string)collection[num2].Attributes["value"].Value != "Negotiate:MSOIDSSP");
						num2++;
					}
					if (flag3)
					{
						flag7 |= ((string)collection[num2].Attributes["value"].Value != "Negotiate");
						num2++;
					}
					if (flag4)
					{
						flag7 |= ((string)collection[num2].Attributes["value"].Value != "Negotiate:Kerberos");
						num2++;
					}
					if (flag5)
					{
						flag7 |= ((string)collection[num2].Attributes["value"].Value != "NTLM");
						num2++;
					}
				}
				if (flag7)
				{
					flag = true;
					collection.Clear();
					if (flag6)
					{
						ConfigurationElement configurationElement = collection.CreateElement("add");
						configurationElement["value"] = "Negotiate:MSOIDSSP";
						collection.Add(configurationElement);
					}
					if (flag3)
					{
						ConfigurationElement configurationElement2 = collection.CreateElement("add");
						configurationElement2["value"] = "Negotiate";
						collection.Add(configurationElement2);
					}
					if (flag4)
					{
						ConfigurationElement configurationElement3 = collection.CreateElement("add");
						configurationElement3["value"] = "Negotiate:Kerberos";
						collection.Add(configurationElement3);
					}
					if (flag5)
					{
						ConfigurationElement configurationElement4 = collection.CreateElement("add");
						configurationElement4["value"] = "NTLM";
						collection.Add(configurationElement4);
					}
				}
			}
			return flag;
		}

		internal static bool UpdateAuthenticationModules(ServerManager serverManager, string siteName, string virtualPath, MultiValuedProperty<AuthenticationMethod> authMethods, bool useApplicationHostConfigForModules)
		{
			bool flag = false;
			string text = siteName + virtualPath;
			ConfigurationSection section;
			if (useApplicationHostConfigForModules)
			{
				Configuration configuration = serverManager.GetApplicationHostConfiguration();
				section = configuration.GetSection("system.webServer/modules", text);
			}
			else
			{
				Configuration configuration = serverManager.Sites[siteName].Applications[virtualPath].GetWebConfiguration();
				section = configuration.GetSection("system.webServer/modules");
			}
			if (section == null)
			{
				throw new WebObjectNotFoundException(Strings.ExceptionWebObjectNotFound(siteName + "/" + virtualPath + "/system.webServer/modules"));
			}
			ConfigurationElementCollection collection = section.GetCollection();
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.WindowsLiveID.Enabled)
			{
				flag |= IISConfigurationUtilities.UpdateWebServerModule(collection, "LiveIdBasicAuthModule", "Microsoft.Exchange.Security.Authentication.LiveIdBasicAuthModule, Microsoft.Exchange.Security, Version=15.0.0.0, Culture=neutral, publicKeyToken=31bf3856ad364e35", authMethods.Contains(AuthenticationMethod.LiveIdBasic));
				flag |= IISConfigurationUtilities.UpdateWebServerModule(collection, "LiveIdNegotiateAuxiliaryModule", "Microsoft.Exchange.Security.Authentication.LiveIdNegotiateAuxiliaryModule, Microsoft.Exchange.Security, Version=15.0.0.0, Culture=neutral, publicKeyToken=31bf3856ad364e35", authMethods.Contains(AuthenticationMethod.LiveIdNegotiate));
			}
			return flag | IISConfigurationUtilities.UpdateWebServerModule(collection, "OAuthAuthModule", typeof(OAuthHttpModule).AssemblyQualifiedName, authMethods.Contains(AuthenticationMethod.OAuth));
		}

		internal static bool CreateOrUpdateApplicationPool(ServerManager serverManager, string appPoolName, string clrConfigFilePath, out bool isAppPoolNew)
		{
			bool flag = false;
			isAppPoolNew = false;
			ApplicationPoolCollection applicationPools = serverManager.ApplicationPools;
			ConfigurationElementCollection collection = applicationPools.GetCollection();
			if (IISConfigurationUtilities.FindElement(collection, "add", "name", appPoolName) == null)
			{
				flag = true;
				isAppPoolNew = true;
				applicationPools.Add(appPoolName);
			}
			ApplicationPool applicationPool = applicationPools[appPoolName];
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool, "autostart", true);
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool, "managedPipelineMode", "Integrated");
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool, "managedRuntimeVersion", "v4.0");
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool, "queueLength", 65535);
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool.Failure, "rapidFailProtection", false);
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool.ProcessModel, "identityType", "LocalSystem");
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool.ProcessModel, "loadUserProfile", true);
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool.ProcessModel, "idleTimeout", TimeSpan.FromSeconds(0.0));
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool.ProcessModel, "pingingEnabled", false);
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool.ProcessModel, "shutdownTimeLimit", TimeSpan.FromSeconds(5.0));
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool.Recycling, "disallowOverlappingRotation", true);
			flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool.Recycling.PeriodicRestart, "time", TimeSpan.FromSeconds(0.0));
			if (!string.IsNullOrEmpty(clrConfigFilePath))
			{
				flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool, "CLRConfigFile", clrConfigFilePath);
				flag |= IISConfigurationUtilities.UpdateElementAttribute(applicationPool, "managedRuntimeLoader", string.Empty);
			}
			return flag;
		}

		internal static bool ConfigureApplication(ServerManager serverManager, string webSiteName, string applicationPoolName, string virtualPath, string physicalPath, out bool recycleAppPool)
		{
			bool flag = true;
			recycleAppPool = false;
			Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
			ConfigurationSection section = applicationHostConfiguration.GetSection("system.applicationHost/sites");
			ConfigurationElementCollection collection = section.GetCollection();
			ConfigurationElement configurationElement = IISConfigurationUtilities.FindElement(collection, "site", "name", webSiteName);
			ConfigurationElementCollection collection2 = configurationElement.GetCollection();
			ConfigurationElement configurationElement2 = IISConfigurationUtilities.FindElement(collection2, "application", "path", virtualPath);
			if (configurationElement2 == null)
			{
				configurationElement2 = collection2.CreateElement("application");
				configurationElement2["path"] = virtualPath;
				collection2.Add(configurationElement2);
				flag = true;
			}
			recycleAppPool |= IISConfigurationUtilities.UpdateElementAttribute(configurationElement2, "applicationPool", applicationPoolName);
			flag |= recycleAppPool;
			ConfigurationElementCollection collection3 = configurationElement2.GetCollection();
			ConfigurationElement configurationElement3 = IISConfigurationUtilities.FindElement(collection3, "virtualDirectory", "path", "/");
			if (configurationElement3 == null)
			{
				configurationElement3 = collection3.CreateElement("virtualDirectory");
				configurationElement3["path"] = "/";
				collection3.Add(configurationElement3);
				flag = true;
			}
			return flag | IISConfigurationUtilities.UpdateElementAttribute(configurationElement3, "physicalPath", physicalPath);
		}

		internal static bool TryRecycleApplicationPool(ServerManager serverManager, string applicationPoolName)
		{
			bool result;
			try
			{
				ApplicationPoolCollection applicationPools = serverManager.ApplicationPools;
				ApplicationPool applicationPool = applicationPools[applicationPoolName];
				if (applicationPool == null)
				{
					result = false;
				}
				else
				{
					applicationPool.Recycle();
					result = true;
				}
			}
			catch (COMException)
			{
				result = false;
			}
			return result;
		}

		internal static bool SetSslFlags(ServerManager serverManager, string siteName, string virtualPath, string sslFlags)
		{
			string text = siteName + virtualPath;
			Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
			ConfigurationSection section = applicationHostConfiguration.GetSection("system.webServer/security/access", text);
			return IISConfigurationUtilities.UpdateSectionAttribute(section, "sslFlags", sslFlags);
		}

		public static void TryRunIISConfigurationOperation(IISConfigurationUtilities.IISConfigurationOperation iisConfigurationOperation, int retryCount, int sleepInterval, Task task)
		{
			if (iisConfigurationOperation == null)
			{
				throw new ArgumentNullException("iisConfigurationOperation");
			}
			if (retryCount < 0)
			{
				throw new ArgumentOutOfRangeException("retryCount", "Number of retries must be >= zero.");
			}
			if (sleepInterval < 0)
			{
				throw new ArgumentOutOfRangeException("sleepInterval", "Sleep interval must be >= zero.");
			}
			int num = sleepInterval * 1000;
			for (int i = 0; i <= retryCount; i++)
			{
				try
				{
					iisConfigurationOperation();
					break;
				}
				catch (Exception ex)
				{
					if (i == retryCount)
					{
						throw;
					}
					if (task != null)
					{
						task.WriteWarning(Strings.ErrorIisConfigurationWillRetry(ex.GetType().ToString(), ex.Message, (i + 1) * sleepInterval));
					}
				}
				if (sleepInterval > 0)
				{
					Thread.Sleep((i + 1) * num);
				}
			}
		}

		public static void CreateAndConfigureLocalApplicationPoolAndVirtualDirectory(string physicalPath, string clrConfigFilePath, string applicationPoolName, string siteName, string virtualPath, string sslFlags, MultiValuedProperty<AuthenticationMethod> iisAuthenticationMethods)
		{
			bool flag = false;
			bool flag2 = false;
			using (ServerManager serverManager = new ServerManager())
			{
				bool flag3 = false;
				flag3 |= IISConfigurationUtilities.CreateOrUpdateApplicationPool(serverManager, applicationPoolName, clrConfigFilePath, out flag2);
				flag3 |= IISConfigurationUtilities.ConfigureApplication(serverManager, siteName, applicationPoolName, virtualPath, physicalPath, out flag);
				flag3 |= IISConfigurationUtilities.UpdateAuthenticationSettingsAndProviders(serverManager, siteName, virtualPath, iisAuthenticationMethods);
				flag3 |= IISConfigurationUtilities.SetSslFlags(serverManager, siteName, virtualPath, sslFlags);
				if (flag3)
				{
					serverManager.CommitChanges();
				}
			}
			using (ServerManager serverManager2 = new ServerManager())
			{
				bool flag4 = IISConfigurationUtilities.UpdateAuthenticationModules(serverManager2, siteName, virtualPath, iisAuthenticationMethods, false);
				if (flag4)
				{
					serverManager2.CommitChanges();
				}
				if (flag)
				{
					IISConfigurationUtilities.TryRecycleApplicationPool(serverManager2, applicationPoolName);
				}
			}
		}

		public static void UpdateRemoteVirtualDirectory(string remoteServerName, string siteName, string virtualPath, MultiValuedProperty<AuthenticationMethod> iisAuthenticationMethods)
		{
			using (ServerManager serverManager = ServerManager.OpenRemote(remoteServerName))
			{
				bool flag = IISConfigurationUtilities.UpdateAuthenticationSettingsAndProviders(serverManager, siteName, virtualPath, iisAuthenticationMethods);
				if (flag)
				{
					serverManager.CommitChanges();
				}
			}
			using (ServerManager serverManager2 = ServerManager.OpenRemote(remoteServerName))
			{
				bool flag2 = IISConfigurationUtilities.UpdateAuthenticationModules(serverManager2, siteName, virtualPath, iisAuthenticationMethods, false);
				if (flag2)
				{
					serverManager2.CommitChanges();
				}
			}
		}

		public static void CreateAndConfigureLocalMapiHttpFrontEnd(MultiValuedProperty<AuthenticationMethod> iisAuthenticationMethods)
		{
			string physicalPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "FrontEnd\\HttpProxy\\mapi");
			string clrConfigFilePath = Path.Combine(ConfigurationContext.Setup.InstallPath, "bin\\MSExchangeMapiFrontEndAppPool_CLRConfig.config");
			IISConfigurationUtilities.CreateAndConfigureLocalApplicationPoolAndVirtualDirectory(physicalPath, clrConfigFilePath, "MSExchangeMapiFrontEndAppPool", "Default Web Site", "/mapi", "Ssl,Ssl128", iisAuthenticationMethods);
		}

		public static void UpdateRemoteMapiHttpFrontEnd(string remoteServerName, MultiValuedProperty<AuthenticationMethod> iisAuthenticationMethods)
		{
			IISConfigurationUtilities.UpdateRemoteVirtualDirectory(remoteServerName, "Default Web Site", "/mapi", iisAuthenticationMethods);
		}

		private const string DefaultWebSiteName = "Default Web Site";

		private const string MapiHttpVirtualPath = "/mapi";

		internal const int IISConfigurationOperationDefaultRetryTimes = 5;

		internal const int IISConfigurationOperationDefaultSleepInterval = 2;

		internal delegate void IISConfigurationOperation();
	}
}
