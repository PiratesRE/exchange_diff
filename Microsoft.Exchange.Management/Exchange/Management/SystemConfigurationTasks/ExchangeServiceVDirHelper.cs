using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class ExchangeServiceVDirHelper
	{
		public static void SetIisVirtualDirectoryAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory, Task.TaskErrorLoggingDelegate errorHandler, LocalizedString errorMessage)
		{
			ExchangeServiceVDirHelper.SetSplitVirtualDirectoryAuthenticationMethods(virtualDirectory, null, errorHandler, errorMessage);
		}

		public static void SetSplitVirtualDirectoryAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory, string nego2Path, Task.TaskErrorLoggingDelegate errorHandler, LocalizedString errorMessage)
		{
			try
			{
				ExchangeServiceVDirHelper.SetIisVirtualDirectoryAuthenticationMethods(virtualDirectory);
				if (!string.IsNullOrEmpty(nego2Path))
				{
					bool? windowsAuthentication = virtualDirectory.WindowsAuthentication;
					bool? liveIdNegotiateAuthentication = virtualDirectory.LiveIdNegotiateAuthentication;
					ADExchangeServiceVirtualDirectory adexchangeServiceVirtualDirectory = (ADExchangeServiceVirtualDirectory)virtualDirectory.Clone();
					if (windowsAuthentication != null)
					{
						adexchangeServiceVirtualDirectory.WindowsAuthentication = new bool?(false);
					}
					if (liveIdNegotiateAuthentication != null)
					{
						virtualDirectory.LiveIdNegotiateAuthentication = new bool?(false);
					}
					ExchangeServiceVDirHelper.SetIisVirtualDirectoryAuthenticationMethods(virtualDirectory);
					ExchangeServiceVDirHelper.SetIisVirtualDirectoryAuthenticationMethods(adexchangeServiceVirtualDirectory, nego2Path);
				}
			}
			catch (DataSourceTransientException innerException)
			{
				errorHandler(new LocalizedException(errorMessage, innerException), ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
			catch (DataSourceOperationException innerException2)
			{
				errorHandler(new LocalizedException(errorMessage, innerException2), ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
			catch (COMException innerException3)
			{
				errorHandler(new LocalizedException(errorMessage, innerException3), ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
		}

		public static void SetIisVirtualDirectoryAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory)
		{
			ExchangeServiceVDirHelper.SetIisVirtualDirectoryAuthenticationMethods(virtualDirectory, virtualDirectory.MetabasePath);
		}

		public static void SetIisVirtualDirectoryAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory, string metabasePath)
		{
			DirectoryEntry directoryEntry2;
			DirectoryEntry directoryEntry = directoryEntry2 = IisUtility.CreateIISDirectoryEntry(metabasePath);
			try
			{
				if (virtualDirectory.BasicAuthentication != null)
				{
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic, virtualDirectory.BasicAuthentication.Value);
				}
				if (virtualDirectory.DigestAuthentication != null)
				{
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Digest, virtualDirectory.DigestAuthentication.Value);
				}
				if (virtualDirectory.WindowsAuthentication != null)
				{
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Ntlm, virtualDirectory.WindowsAuthentication.Value);
				}
				if (virtualDirectory.WSSecurityAuthentication != null)
				{
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.WSSecurity, virtualDirectory.WSSecurityAuthentication.Value);
				}
				if (virtualDirectory.LiveIdNegotiateAuthentication != null)
				{
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.LiveIdNegotiate, virtualDirectory.LiveIdNegotiateAuthentication.Value);
				}
				bool? flag = null;
				if (virtualDirectory is ADWebServicesVirtualDirectory)
				{
					ADWebServicesVirtualDirectory adwebServicesVirtualDirectory = (ADWebServicesVirtualDirectory)virtualDirectory;
					flag = adwebServicesVirtualDirectory.CertificateAuthentication;
				}
				else if (virtualDirectory is ADPowerShellCommonVirtualDirectory)
				{
					ADPowerShellCommonVirtualDirectory adpowerShellCommonVirtualDirectory = (ADPowerShellCommonVirtualDirectory)virtualDirectory;
					flag = adpowerShellCommonVirtualDirectory.CertificateAuthentication;
				}
				if (flag != null)
				{
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Certificate, flag.Value);
				}
				directoryEntry.CommitChanges();
				IisUtility.CommitMetabaseChanges((virtualDirectory.Server == null) ? null : virtualDirectory.Server.ToString());
			}
			finally
			{
				if (directoryEntry2 != null)
				{
					((IDisposable)directoryEntry2).Dispose();
				}
			}
		}

		internal static void CheckAndUpdateWindowsAuthProvidersIfNecessary(ADExchangeServiceVirtualDirectory adVDir, bool? windowsAuthentication)
		{
			if (windowsAuthentication == null || !windowsAuthentication.Value)
			{
				return;
			}
			DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(adVDir.MetabasePath);
			using (ServerManager serverManager = ServerManager.OpenRemote(IisUtility.GetHostName(adVDir.MetabasePath)))
			{
				Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
				ConfigurationSection section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/windowsAuthentication", "/" + IisUtility.GetWebSiteName(directoryEntry.Parent.Path) + "/" + directoryEntry.Name);
				ConfigurationElementCollection collection = section.GetCollection("providers");
				foreach (ConfigurationElement configurationElement in collection)
				{
					string a = configurationElement.GetAttributeValue("value") as string;
					if (string.Equals(a, "Negotiate", StringComparison.OrdinalIgnoreCase))
					{
						return;
					}
				}
				ConfigurationElement configurationElement2 = collection.CreateElement();
				configurationElement2.SetAttributeValue("value", "Negotiate");
				collection.AddAt(0, configurationElement2);
				serverManager.CommitChanges();
			}
		}

		public static bool? IsSSLRequired(ADExchangeServiceVirtualDirectory virtualDirectory, Task.TaskErrorLoggingDelegate errorHandler)
		{
			bool? result = null;
			try
			{
				using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(virtualDirectory.MetabasePath))
				{
					int? num = (int?)directoryEntry.Properties["AccessSSLFlags"].Value;
					if (num != null)
					{
						result = new bool?((num.Value & 8) == 8);
					}
				}
			}
			catch (DataSourceTransientException exception)
			{
				errorHandler(exception, ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
			catch (DataSourceOperationException exception2)
			{
				errorHandler(exception2, ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
			catch (COMException exception3)
			{
				errorHandler(exception3, ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
			return result;
		}

		public static void SetSSLRequired(ADExchangeServiceVirtualDirectory virtualDirectory, Task.TaskErrorLoggingDelegate errorHandler, LocalizedString errorMessage, bool enabled)
		{
			try
			{
				using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(virtualDirectory.MetabasePath))
				{
					if (enabled)
					{
						directoryEntry.Properties["AccessSSLFlags"].Value = (MetabasePropertyTypes.AccessSSLFlags.AccessSSL | MetabasePropertyTypes.AccessSSLFlags.AccessSSLNegotiateCert | MetabasePropertyTypes.AccessSSLFlags.AccessSSL128);
					}
					else
					{
						directoryEntry.Properties["AccessSSLFlags"].Value = MetabasePropertyTypes.AccessSSLFlags.AccessSSLNegotiateCert;
					}
					directoryEntry.CommitChanges();
					IisUtility.CommitMetabaseChanges((virtualDirectory.Server == null) ? null : virtualDirectory.Server.ToString());
				}
			}
			catch (DataSourceTransientException innerException)
			{
				errorHandler(new LocalizedException(errorMessage, innerException), ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
			catch (DataSourceOperationException innerException2)
			{
				errorHandler(new LocalizedException(errorMessage, innerException2), ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
			catch (COMException innerException3)
			{
				errorHandler(new LocalizedException(errorMessage, innerException3), ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
		}

		internal static void ForceAnonymous(string metabasePath)
		{
			ExchangeServiceVDirHelper.ConfigureAnonymousAuthentication(metabasePath, true);
		}

		internal static void ConfigureAnonymousAuthentication(string metabasePath, bool enableAnonymous)
		{
			try
			{
				DirectoryEntry directoryEntry2;
				DirectoryEntry directoryEntry = directoryEntry2 = IisUtility.CreateIISDirectoryEntry(metabasePath);
				try
				{
					IisUtility.SetAuthenticationMethod(directoryEntry, MetabasePropertyTypes.AuthFlags.Anonymous, enableAnonymous);
					directoryEntry.CommitChanges();
				}
				finally
				{
					if (directoryEntry2 != null)
					{
						((IDisposable)directoryEntry2).Dispose();
					}
				}
			}
			catch (IISGeneralCOMException ex)
			{
				if (ex.Code == -2147023174)
				{
					throw new IISNotReachableException(IisUtility.GetHostName(metabasePath), ex.Message);
				}
				throw;
			}
		}

		internal static void CheckAndUpdateLocalhostWebBindingsIfNecessary(ADExchangeServiceVirtualDirectory adVDir)
		{
			ExchangeServiceVDirHelper.CheckAndUpdateBindingsIfNecessary(adVDir, ExchangeServiceVDirHelper.LocalHostBindings);
		}

		internal static void CheckAndUpdateLocalhostNetPipeBindingsIfNecessary(ADExchangeServiceVirtualDirectory adVDir)
		{
			ExchangeServiceVDirHelper.CheckAndUpdateBindingsIfNecessary(adVDir, ExchangeServiceVDirHelper.NetPipeBindings);
		}

		private static void CheckAndUpdateBindingsIfNecessary(ADExchangeServiceVirtualDirectory adVDir, List<ExchangeServiceVDirHelper.WebBinding> bindings)
		{
			DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(adVDir.MetabasePath);
			string webSiteName = IisUtility.GetWebSiteName(directoryEntry.Parent.Path);
			using (ServerManager serverManager = ServerManager.OpenRemote(IisUtility.GetHostName(adVDir.MetabasePath)))
			{
				Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
				if (applicationHostConfiguration != null)
				{
					ConfigurationSection section = applicationHostConfiguration.GetSection("system.applicationHost/sites");
					if (section != null)
					{
						ConfigurationElementCollection collection = section.GetCollection();
						if (collection != null)
						{
							ConfigurationElement configurationElement = ExchangeServiceVDirHelper.FindSiteElement(collection, webSiteName);
							if (configurationElement != null)
							{
								ConfigurationElementCollection collection2 = configurationElement.GetCollection("bindings");
								bool flag = false;
								foreach (ExchangeServiceVDirHelper.WebBinding binding in bindings)
								{
									if (ExchangeServiceVDirHelper.FindBindingElement(collection2, binding) == null)
									{
										ExchangeServiceVDirHelper.AddBindingElement(collection2, binding);
										flag = true;
									}
								}
								if (flag)
								{
									serverManager.CommitChanges();
								}
							}
						}
					}
				}
			}
		}

		public static void ExecuteUnderVDirSpecificAssemblyResolvers(Action configurationAction)
		{
			AssemblyResolver[] resolvers = AssemblyResolver.Install(new AssemblyResolver[]
			{
				new FileSearchAssemblyResolver
				{
					FileNameFilter = new Func<string, bool>(AssemblyResolver.ExchangePrefixedAssembliesOnly),
					SearchPaths = new string[]
					{
						ExchangeSetupContext.InstallPath
					},
					Recursive = true
				}
			});
			try
			{
				configurationAction();
			}
			finally
			{
				AssemblyResolver.Uninstall(resolvers);
			}
		}

		private static void AddBindingElement(ConfigurationElementCollection bindingsCollection, ExchangeServiceVDirHelper.WebBinding binding)
		{
			ConfigurationElement configurationElement = bindingsCollection.CreateElement("binding");
			configurationElement["protocol"] = binding.Protocol;
			configurationElement["bindingInformation"] = binding.Info;
			bindingsCollection.Add(configurationElement);
		}

		private static ConfigurationElement FindBindingElement(ConfigurationElementCollection bindingsCollection, ExchangeServiceVDirHelper.WebBinding binding)
		{
			return ExchangeServiceVDirHelper.FindConfigurationElement(bindingsCollection, "binding", (ConfigurationElement e) => ExchangeServiceVDirHelper.AttributeValueMatches(e, "protocol", binding.Protocol) && ExchangeServiceVDirHelper.AttributeValueMatches(e, "bindingInformation", binding.Info));
		}

		private static ConfigurationElement FindSiteElement(ConfigurationElementCollection sitesCollection, string webSiteName)
		{
			return ExchangeServiceVDirHelper.FindConfigurationElement(sitesCollection, "site", (ConfigurationElement e) => ExchangeServiceVDirHelper.AttributeValueMatches(e, "name", webSiteName));
		}

		private static ConfigurationElement FindConfigurationElement(ConfigurationElementCollection collection, string elementTagName, Func<ConfigurationElement, bool> predicate)
		{
			foreach (ConfigurationElement configurationElement in collection)
			{
				if (string.Equals(configurationElement.ElementTagName, elementTagName, StringComparison.OrdinalIgnoreCase) && predicate(configurationElement))
				{
					return configurationElement;
				}
			}
			return null;
		}

		private static bool AttributeValueMatches(ConfigurationElement element, string attributeName, string attributeValue)
		{
			object attributeValue2 = element.GetAttributeValue(attributeName);
			return attributeValue2 != null && attributeValue2.ToString().Equals(attributeValue);
		}

		internal static void SetAuthModule(bool EnableModule, bool isChildVDirApplication, string moduleName, string moduleType, ExchangeVirtualDirectory advdir)
		{
			DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(advdir.MetabasePath);
			using (ServerManager serverManager = ServerManager.OpenRemote(IisUtility.GetHostName(advdir.MetabasePath)))
			{
				Configuration webConfiguration;
				if (isChildVDirApplication)
				{
					webConfiguration = serverManager.Sites[IisUtility.GetWebSiteName(directoryEntry.Parent.Parent.Path)].Applications[string.Format("/{0}/{1}", directoryEntry.Parent.Name, directoryEntry.Name)].GetWebConfiguration();
				}
				else
				{
					webConfiguration = serverManager.Sites[IisUtility.GetWebSiteName(directoryEntry.Parent.Path)].Applications["/" + directoryEntry.Name].GetWebConfiguration();
				}
				ConfigurationElementCollection collection = webConfiguration.GetSection("system.webServer/modules").GetCollection();
				int num = Array.IndexOf<string>(ExchangeServiceVDirHelper.OrderedModuleList, moduleName);
				int num2 = ExchangeServiceVDirHelper.OrderedModuleList.Length;
				int[] array = new int[num2];
				ConfigurationElement configurationElement = null;
				foreach (ConfigurationElement configurationElement2 in collection)
				{
					if (num != -1)
					{
						for (int i = 0; i < num2; i++)
						{
							if (string.Equals(configurationElement2.Attributes["name"].Value.ToString(), ExchangeServiceVDirHelper.OrderedModuleList[i], StringComparison.Ordinal))
							{
								array[i] = collection.IndexOf(configurationElement2);
								break;
							}
						}
					}
					if (string.Equals(configurationElement2.Attributes["name"].Value.ToString(), moduleName, StringComparison.Ordinal))
					{
						configurationElement = configurationElement2;
						break;
					}
				}
				if (configurationElement == null && EnableModule)
				{
					int j = collection.Count;
					if (num != -1)
					{
						for (int k = 0; k < num2; k++)
						{
							if (k < num && array[k] != 0)
							{
								j = array[k] + 1;
							}
							else if (k > num && array[k] != 0)
							{
								j = array[k];
								break;
							}
						}
					}
					configurationElement = collection.CreateElement();
					configurationElement.SetAttributeValue("name", moduleName);
					configurationElement.SetAttributeValue("type", moduleType);
					if (j == collection.Count || (j != 0 && collection[j - 1].IsLocallyStored))
					{
						collection.AddAt(j, configurationElement);
					}
					else
					{
						List<ConfigurationElement> list = new List<ConfigurationElement>();
						while (j < collection.Count)
						{
							ConfigurationElement configurationElement3 = collection[j];
							collection.Remove(configurationElement3);
							list.Add(configurationElement3);
						}
						collection.Add(configurationElement);
						foreach (ConfigurationElement configurationElement4 in list)
						{
							ConfigurationElement configurationElement5 = collection.CreateElement(configurationElement4.ElementTagName);
							foreach (ConfigurationAttribute configurationAttribute in configurationElement4.Attributes)
							{
								if (configurationAttribute.Value != null && !configurationAttribute.Value.ToString().Equals(string.Empty))
								{
									configurationElement5.SetAttributeValue(configurationAttribute.Name, configurationAttribute.Value);
								}
							}
							collection.Add(configurationElement5);
						}
					}
					serverManager.CommitChanges();
				}
				else if (configurationElement != null && !EnableModule)
				{
					collection.Remove(configurationElement);
					serverManager.CommitChanges();
				}
			}
		}

		internal static void SetLiveIdBasicAuthModule(bool EnableModule, bool isChildVDirApplication, ADExchangeServiceVirtualDirectory advdir)
		{
			ExchangeServiceVDirHelper.SetAuthModule(EnableModule, isChildVDirApplication, "LiveIdBasicAuthenticationModule", typeof(LiveIdBasicAuthModule).FullName, advdir);
		}

		internal static void SetOAuthAuthenticationModule(bool EnableModule, bool isChildVDirApplication, ExchangeVirtualDirectory advdir)
		{
			ExchangeServiceVDirHelper.SetAuthModule(EnableModule, isChildVDirApplication, "OAuthAuthModule", typeof(OAuthHttpModule).FullName, advdir);
		}

		internal static void SetAdfsAuthenticationModule(bool EnableModule, bool isChildVDirApplication, ExchangeVirtualDirectory advdir)
		{
			ExchangeServiceVDirHelper.SetAuthModule(EnableModule, isChildVDirApplication, "ADFSFederationAuthModule", typeof(AdfsFederationAuthModule).FullName, advdir);
			ExchangeServiceVDirHelper.SetAuthModule(EnableModule, isChildVDirApplication, "ADFSSessionAuthModule", typeof(AdfsSessionAuthModule).FullName, advdir);
		}

		internal static bool IsBackEndVirtualDirectory(ExchangeVirtualDirectory adVirtualDirectory)
		{
			return adVirtualDirectory.Name.EndsWith("(Exchange Back End)", StringComparison.OrdinalIgnoreCase);
		}

		internal static void UpdateFrontEndHttpModule(ExchangeVirtualDirectory advdir, bool enableFba)
		{
			DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(advdir.MetabasePath);
			using (ServerManager serverManager = ServerManager.OpenRemote(IisUtility.GetHostName(advdir.MetabasePath)))
			{
				Configuration webConfiguration = serverManager.Sites[IisUtility.GetWebSiteName(directoryEntry.Parent.Path)].Applications["/" + directoryEntry.Name].GetWebConfiguration();
				ConfigurationElementCollection collection = webConfiguration.GetSection("system.webServer/modules").GetCollection();
				foreach (ConfigurationElement configurationElement in collection)
				{
					if (string.Equals(configurationElement.Attributes["name"].Value.ToString(), "HttpProxy", StringComparison.OrdinalIgnoreCase))
					{
						if (enableFba)
						{
							configurationElement.SetAttributeValue("type", "Microsoft.Exchange.HttpProxy.FbaModule,Microsoft.Exchange.FrontEndHttpProxy,Version=15.0.0.0,Culture=neutral,PublicKeyToken=31bf3856ad364e35");
						}
						else
						{
							configurationElement.SetAttributeValue("type", "Microsoft.Exchange.HttpProxy.ProxyModule,Microsoft.Exchange.FrontEndHttpProxy,Version=15.0.0.0,Culture=neutral,PublicKeyToken=31bf3856ad364e35");
						}
					}
				}
				serverManager.CommitChanges();
			}
		}

		internal static void RunAppcmd(string args)
		{
			string text = Path.Combine(new string[]
			{
				Environment.GetEnvironmentVariable("windir"),
				"system32",
				"inetsrv"
			});
			string text2 = Path.Combine(text, "appcmd.exe");
			if (!File.Exists(text2))
			{
				throw new AppcmdException(Strings.AppcmdNotFoundInPath(text2));
			}
			string text3;
			string error;
			int num = ProcessRunner.Run(text2, args, -1, text, out text3, out error);
			if (num != 0)
			{
				throw new AppcmdException(Strings.AppcmdExecutionFailed(num, error));
			}
		}

		public const string BasicAuthenticationField = "BasicAuthentication";

		public const string DigestAuthenticationField = "DigestAuthentication";

		public const string WindowsAuthenticationField = "WindowsAuthentication";

		public const string LiveIdBasicAuthenticationField = "LiveIdBasicAuthentication";

		public const string LiveIdBasicAuthenticationModule = "LiveIdBasicAuthenticationModule";

		public const string LiveIdNegotiateAuthenticationField = "LiveIdNegotiateAuthentication";

		public const string OAuthAuthenticationField = "OAuthAuthentication";

		public const string LiveIdNegotiateAuxiliaryModule = "LiveIdNegotiateAuxiliaryModule";

		public const string DelegatedAuthenticationModule = "DelegatedAuthModule";

		public const string OAuthAuthenticationModule = "OAuthAuthModule";

		public const string PowerShellRequestFilterModule = "PowerShellRequestFilterModule";

		public const string CertificateHeaderAuthenticationModule = "CertificateHeaderAuthModule";

		public const string SessionKeyRedirectionModule = "SessionKeyRedirectionModule";

		public const string CertificateAuthenticationModule = "CertificateAuthModule";

		public const string LiveIdAuthenticationModule = "LiveIdAuthenticationModule";

		public const string ADFSFederationAuthModule = "ADFSFederationAuthModule";

		public const string ADFSSessionAuthModule = "ADFSSessionAuthModule";

		public const string BackendRehydrationModule = "BackendRehydrationModule";

		public const string WSSecurityAuthenticationField = "WSSecurityAuthentication";

		public const string CertificateAuthenticationField = "CertificateAuthentication";

		public const string ExtendedProtectionTokenCheckingField = "ExtendedProtectionTokenChecking";

		public const string ExtendedProtectionFlagsField = "ExtendedProtectionFlags";

		public const string ExtendedProtectionSpnListField = "ExtendedProtectionSPNList";

		public const string WindowsAuthenticationSectionInAppHostConfig = "system.webServer/security/authentication/windowsAuthentication";

		public const string BackEndWebSiteName = "Exchange Back End";

		private const string BackEndWebSiteNameInParens = "(Exchange Back End)";

		private const string ProvidersCollectionName = "providers";

		private const string NegotiateProviderName = "Negotiate";

		private const string ValueAttributeName = "value";

		private const string BindingElementName = "binding";

		private const string BindingsElementName = "bindings";

		private const string SiteElementName = "site";

		private const string BindingInformationAttributeName = "bindingInformation";

		private const string ProtocolAttributeName = "protocol";

		private const string LocalHostIPv4 = "127.0.0.1";

		private const string LocalHostIPv6 = "[::1]";

		private const string HttpProtocol = "http";

		private const string HttpPortBinding = ":80:";

		private const string HttpsProtocol = "https";

		private const string HttpsPortBinding = ":443:";

		private const string NetPipeProtocol = "net.pipe";

		private const string NetPipeBindingInfo = "*";

		public static string[] OrderedModuleList = new string[]
		{
			"CertificateAuthModule",
			"CertificateHeaderAuthModule",
			"LiveIdBasicAuthenticationModule",
			"LiveIdNegotiateAuxiliaryModule",
			"DelegatedAuthModule",
			"OAuthAuthModule",
			"SessionKeyRedirectionModule",
			"LiveIdAuthenticationModule",
			"ADFSFederationAuthModule",
			"ADFSSessionAuthModule",
			"BackendRehydrationModule"
		};

		private static List<ExchangeServiceVDirHelper.WebBinding> LocalHostBindings = new List<ExchangeServiceVDirHelper.WebBinding>
		{
			new ExchangeServiceVDirHelper.WebBinding
			{
				Protocol = "http",
				Info = "127.0.0.1:80:"
			},
			new ExchangeServiceVDirHelper.WebBinding
			{
				Protocol = "https",
				Info = "127.0.0.1:443:"
			}
		};

		private static List<ExchangeServiceVDirHelper.WebBinding> NetPipeBindings = new List<ExchangeServiceVDirHelper.WebBinding>
		{
			new ExchangeServiceVDirHelper.WebBinding
			{
				Protocol = "net.pipe",
				Info = "*"
			}
		};

		internal static class EwsAutodiscMWA
		{
			private static ConfigurationElement TryFindServiceByName(ConfigurationElementCollection services, string name)
			{
				if (services != null)
				{
					foreach (ConfigurationElement configurationElement in services)
					{
						if (string.Equals(configurationElement[ExchangeServiceVDirHelper.EwsAutodiscMWA.NameAttribute] as string, name, StringComparison.OrdinalIgnoreCase))
						{
							return configurationElement;
						}
					}
				}
				return null;
			}

			private static ConfigurationElement TryFindEndpointByBindingConfiguration(ConfigurationElementCollection endpoints, string bindingConfiguration)
			{
				foreach (ConfigurationElement configurationElement in endpoints)
				{
					if (string.Equals(configurationElement[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointBindingConfiguration] as string, bindingConfiguration, StringComparison.OrdinalIgnoreCase))
					{
						return configurationElement;
					}
				}
				return null;
			}

			private static ConfigurationElement TryFindEndpointByNameAndContract(ConfigurationElementCollection endpoints, string name, string contract)
			{
				foreach (ConfigurationElement configurationElement in endpoints)
				{
					if (string.Equals(configurationElement[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointName] as string, name, StringComparison.OrdinalIgnoreCase) && string.Equals(configurationElement[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointContract] as string, contract, StringComparison.OrdinalIgnoreCase))
					{
						return configurationElement;
					}
				}
				return null;
			}

			private static void EnableOrDisableWSSecurityEndpoint(Configuration configuration, bool enableEndpoint, Task.TaskErrorLoggingDelegate errorHandler, bool isEWS)
			{
				string name = isEWS ? ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSServiceName : ExchangeServiceVDirHelper.EwsAutodiscMWA.AutoDServiceName;
				string text = isEWS ? ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSWSSecurityHttpBinding : ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSecurityHttpBinding;
				string text2 = isEWS ? ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSWSSecurityHttpsBinding : ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSecurityHttpsBinding;
				string text3 = isEWS ? ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSWSSecuritySymmetricKeyHttpBinding : ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSecuritySymmetricKeyHttpBinding;
				string text4 = isEWS ? ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSWSSecuritySymmetricKeyHttpsBinding : ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSecuritySymmetricKeyHttpsBinding;
				string text5 = isEWS ? ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSWSSecurityX509CertHttpBinding : ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSecurityX509CertHttpBinding;
				string text6 = isEWS ? ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSWSSecurityX509CertHttpsBinding : ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSecurityX509CertHttpsBinding;
				string contract = isEWS ? ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSContract : ExchangeServiceVDirHelper.EwsAutodiscMWA.AutoDContract;
				ConfigurationSection section = configuration.GetSection(ExchangeServiceVDirHelper.EwsAutodiscMWA.ServicesSectionName);
				ConfigurationElement configurationElement = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindServiceByName(section.GetCollection(), name);
				if (configurationElement == null)
				{
					errorHandler(new LocalizedException(isEWS ? Strings.CouldNotFindEWSService : Strings.CouldNotFindAutodiscoverService, new ArgumentNullException("serviceElement")), ErrorCategory.InvalidOperation, null);
				}
				ConfigurationElementCollection collection = configurationElement.GetCollection();
				if (collection == null)
				{
					return;
				}
				ConfigurationElement configurationElement2 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByBindingConfiguration(collection, text);
				ConfigurationElement configurationElement3 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByBindingConfiguration(collection, text2);
				ConfigurationElement configurationElement4 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByBindingConfiguration(collection, text3);
				ConfigurationElement configurationElement5 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByBindingConfiguration(collection, text4);
				ConfigurationElement configurationElement6 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByBindingConfiguration(collection, text5);
				ConfigurationElement configurationElement7 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByBindingConfiguration(collection, text6);
				if (!enableEndpoint)
				{
					if (configurationElement2 != null)
					{
						collection.Remove(configurationElement2);
					}
					if (configurationElement3 != null)
					{
						collection.Remove(configurationElement3);
					}
					if (configurationElement4 != null)
					{
						collection.Remove(configurationElement4);
					}
					if (configurationElement5 != null)
					{
						collection.Remove(configurationElement5);
					}
					if (configurationElement6 != null)
					{
						collection.Remove(configurationElement6);
					}
					if (configurationElement7 != null)
					{
						collection.Remove(configurationElement7);
						return;
					}
				}
				else
				{
					if (configurationElement2 == null)
					{
						ExchangeServiceVDirHelper.EwsAutodiscMWA.AddEndpointElement(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSEndpointUri, text, contract);
					}
					if (configurationElement3 == null)
					{
						ExchangeServiceVDirHelper.EwsAutodiscMWA.AddEndpointElement(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSEndpointUri, text2, contract);
					}
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.WsSecuritySymmetricAndX509Cert.Enabled)
					{
						if (configurationElement4 == null)
						{
							ExchangeServiceVDirHelper.EwsAutodiscMWA.AddEndpointElement(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSSymmetricKeyEndpointUri, text3, contract);
						}
						if (configurationElement5 == null)
						{
							ExchangeServiceVDirHelper.EwsAutodiscMWA.AddEndpointElement(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSSymmetricKeyEndpointUri, text4, contract);
						}
						if (configurationElement6 == null)
						{
							ExchangeServiceVDirHelper.EwsAutodiscMWA.AddEndpointElement(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSX509CertEndpointUri, text5, contract);
						}
						if (configurationElement7 == null)
						{
							ExchangeServiceVDirHelper.EwsAutodiscMWA.AddEndpointElement(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.WSSX509CertEndpointUri, text6, contract);
						}
					}
				}
			}

			private static void EnableOrDisableCafeEndpoint(Configuration configuration, string endpointName, bool enableEndpoint)
			{
				string text = enableEndpoint.ToString(CultureInfo.InvariantCulture);
				ConfigurationElement configurationElement = null;
				ConfigurationSection section = configuration.GetSection("appSettings");
				ConfigurationElementCollection collection = section.GetCollection();
				if (collection != null)
				{
					foreach (ConfigurationElement configurationElement2 in collection)
					{
						if (configurationElement2["key"].ToString().Equals(endpointName, StringComparison.OrdinalIgnoreCase))
						{
							configurationElement = configurationElement2;
							break;
						}
					}
				}
				if (configurationElement == null)
				{
					configurationElement = collection.CreateElement("add");
					configurationElement["key"] = endpointName;
					configurationElement["value"] = text;
					collection.Add(configurationElement);
					return;
				}
				configurationElement["value"] = text;
			}

			private static void AddEndpointElement(ConfigurationElementCollection endpoints, Uri endpointUri, string httpsEndpointBindingConfiguration, string contract)
			{
				ConfigurationElement configurationElement = endpoints.CreateElement(ExchangeServiceVDirHelper.EwsAutodiscMWA.Endpoint);
				configurationElement[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointAddress] = endpointUri.ToString();
				configurationElement[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointBinding] = ExchangeServiceVDirHelper.EwsAutodiscMWA.CustomBindingString;
				configurationElement[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointBindingConfiguration] = httpsEndpointBindingConfiguration;
				configurationElement[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointContract] = contract;
				endpoints.Add(configurationElement);
			}

			private static bool GetAuthenticationMethodSetting(ExchangeVirtualDirectory adVirtualDirectory, AuthenticationMethodFlags authMethod)
			{
				bool result;
				using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(adVirtualDirectory.MetabasePath))
				{
					if (authMethod == AuthenticationMethodFlags.WSSecurity)
					{
						result = (adVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.WSSecurity) && IisUtility.CheckForAuthenticationMethod(directoryEntry, authMethod));
					}
					else if (authMethod == AuthenticationMethodFlags.OAuth)
					{
						result = (adVirtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.OAuth) && IisUtility.CheckForAuthenticationMethod(directoryEntry, authMethod));
					}
					else
					{
						result = IisUtility.CheckForAuthenticationMethod(directoryEntry, authMethod);
					}
				}
				return result;
			}

			internal static void OnSetManageWCFEndpoints(Task task, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol protocol, bool enableWSSecurity, ExchangeVirtualDirectory adVirtualDirectory)
			{
				try
				{
					using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(adVirtualDirectory.MetabasePath))
					{
						using (ServerManager serverManager = ServerManager.OpenRemote(IisUtility.GetHostName(adVirtualDirectory.MetabasePath)))
						{
							Configuration webConfiguration = serverManager.GetWebConfiguration(IisUtility.GetWebSiteName(directoryEntry.Parent.Path), "/" + directoryEntry.Name);
							bool flag = task.Fields["WSSecurityAuthentication"] != null;
							bool flag2 = task.Fields["OAuthAuthentication"] != null;
							if (ExchangeServiceVDirHelper.IsBackEndVirtualDirectory(adVirtualDirectory))
							{
								if (flag || flag2)
								{
									if (flag)
									{
										ExchangeServiceVDirHelper.EwsAutodiscMWA.EnableOrDisableWSSecurityEndpoint(webConfiguration, enableWSSecurity, new Task.TaskErrorLoggingDelegate(task.WriteError), protocol == ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol.Ews);
									}
									serverManager.CommitChanges();
								}
							}
							else if (flag)
							{
								ExchangeServiceVDirHelper.EwsAutodiscMWA.EnableOrDisableCafeEndpoint(webConfiguration, "WsSecurityEndpointEnabled", enableWSSecurity);
								if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.WsSecuritySymmetricAndX509Cert.Enabled)
								{
									ExchangeServiceVDirHelper.EwsAutodiscMWA.EnableOrDisableCafeEndpoint(webConfiguration, "WsSecuritySymmetricKeyEndpointEnabled", enableWSSecurity);
									ExchangeServiceVDirHelper.EwsAutodiscMWA.EnableOrDisableCafeEndpoint(webConfiguration, "WsSecurityX509CertEndpointEnabled", enableWSSecurity);
								}
								serverManager.CommitChanges();
							}
						}
					}
				}
				catch (ServerManagerException exception)
				{
					task.WriteError(exception, ErrorCategory.InvalidData, adVirtualDirectory.Identity);
				}
			}

			internal static void OnNewManageWCFEndpoints(Task task, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol protocol, bool? basicAuthentication, bool? windowsAuthentication, bool enableWSSecurity, bool enableOAuth, ExchangeVirtualDirectory adVirtualDirectory, VirtualDirectoryRole role)
			{
				try
				{
					using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(adVirtualDirectory.MetabasePath))
					{
						using (ServerManager serverManager = ServerManager.OpenRemote(IisUtility.GetHostName(adVirtualDirectory.MetabasePath)))
						{
							Configuration webConfiguration = serverManager.GetWebConfiguration(IisUtility.GetWebSiteName(directoryEntry.Parent.Path), "/" + directoryEntry.Name);
							if (role == VirtualDirectoryRole.ClientAccess)
							{
								ExchangeServiceVDirHelper.EwsAutodiscMWA.EnableOrDisableCafeEndpoint(webConfiguration, "WsSecurityEndpointEnabled", enableWSSecurity);
								if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.WsSecuritySymmetricAndX509Cert.Enabled)
								{
									ExchangeServiceVDirHelper.EwsAutodiscMWA.EnableOrDisableCafeEndpoint(webConfiguration, "WsSecuritySymmetricKeyEndpointEnabled", enableWSSecurity);
									ExchangeServiceVDirHelper.EwsAutodiscMWA.EnableOrDisableCafeEndpoint(webConfiguration, "WsSecurityX509CertEndpointEnabled", enableWSSecurity);
								}
							}
							else if (protocol != ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol.OwaEws)
							{
								ExchangeServiceVDirHelper.EwsAutodiscMWA.EnableOrDisableWSSecurityEndpoint(webConfiguration, enableWSSecurity, new Task.TaskErrorLoggingDelegate(task.WriteError), protocol == ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol.Ews);
							}
							serverManager.CommitChanges();
						}
					}
				}
				catch (ServerManagerException exception)
				{
					task.WriteError(exception, ErrorCategory.InvalidData, adVirtualDirectory.Identity);
				}
			}

			private static void UpgradeAutodiscoverEndpoints(Configuration configuration, AuthenticationSchemes schemeToEnable, Task.TaskErrorLoggingDelegate errorHandler)
			{
				ExchangeServiceVDirHelper.EwsAutodiscMWA.UpgradeAutoDiscoverEndpoints(configuration, ExchangeServiceVDirHelper.EwsAutodiscMWA.ServiceNameLegacyAutoD, ExchangeServiceVDirHelper.EwsAutodiscMWA.LegacyAutoDContract, "Autodiscover", schemeToEnable, errorHandler);
				ExchangeServiceVDirHelper.EwsAutodiscMWA.UpgradeAutoDiscoverEndpoints(configuration, ExchangeServiceVDirHelper.EwsAutodiscMWA.AutoDServiceName, ExchangeServiceVDirHelper.EwsAutodiscMWA.AutoDContract, "AutodiscoverSoap", schemeToEnable, errorHandler);
			}

			private static void UpgradeAutoDiscoverEndpoints(Configuration configuration, string serviceName, string contract, string bindingNameRoot, AuthenticationSchemes schemeToEnable, Task.TaskErrorLoggingDelegate errorHandler)
			{
				string str = schemeToEnable.ToString();
				ConfigurationElement section = configuration.GetSection(ExchangeServiceVDirHelper.EwsAutodiscMWA.ServicesSectionName);
				ConfigurationElement configurationElement = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindServiceByName(section.GetCollection(), serviceName);
				if (configurationElement == null)
				{
					errorHandler(new LocalizedException(Strings.CouldNotFindElementWithAttribute(ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointService, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointName, serviceName), new ArgumentNullException("serviceElement")), ErrorCategory.InvalidOperation, null);
				}
				ConfigurationElementCollection collection = configurationElement.GetCollection();
				if (collection == null)
				{
					return;
				}
				ConfigurationElement configurationElement2 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByNameAndContract(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpsEndpointName, contract);
				if (configurationElement2 == null)
				{
					errorHandler(new LocalizedException(Strings.CouldNotFindElementWithTwoAttributes(ExchangeServiceVDirHelper.EwsAutodiscMWA.Endpoint, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpsEndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointContract, contract)), ErrorCategory.InvalidOperation, null);
				}
				configurationElement2[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointBindingConfiguration] = bindingNameRoot + str + ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpsBindingSuffix;
				ConfigurationElement configurationElement3 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByNameAndContract(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpEndpointName, contract);
				if (configurationElement3 == null)
				{
					errorHandler(new LocalizedException(Strings.CouldNotFindElementWithTwoAttributes(ExchangeServiceVDirHelper.EwsAutodiscMWA.Endpoint, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpEndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointContract, contract)), ErrorCategory.InvalidOperation, null);
				}
				configurationElement3[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointBindingConfiguration] = bindingNameRoot + str + ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpBindingSuffix;
			}

			private static void UpdateEWSEndpoints(Configuration configuration, AuthenticationSchemes schemeToEnable, Task.TaskErrorLoggingDelegate errorHandler)
			{
				string str = "EWS";
				string str2 = schemeToEnable.ToString();
				string ewsserviceName = ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSServiceName;
				ConfigurationSection section = configuration.GetSection(ExchangeServiceVDirHelper.EwsAutodiscMWA.ServicesSectionName);
				if (section == null)
				{
					return;
				}
				ConfigurationElement configurationElement = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindServiceByName(section.GetCollection(), ewsserviceName);
				if (configurationElement == null)
				{
					errorHandler(new LocalizedException(Strings.CouldNotFindElementWithAttribute(ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointService, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointName, ewsserviceName), new ArgumentNullException("serviceElement")), ErrorCategory.InvalidOperation, null);
				}
				ConfigurationElementCollection collection = configurationElement.GetCollection();
				if (collection == null)
				{
					return;
				}
				ConfigurationElement configurationElement2 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByNameAndContract(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpsEndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSContract);
				if (configurationElement2 == null)
				{
					errorHandler(new LocalizedException(Strings.CouldNotFindElementWithTwoAttributes(ExchangeServiceVDirHelper.EwsAutodiscMWA.Endpoint, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpsEndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointContract, ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSContract)), ErrorCategory.InvalidOperation, null);
				}
				configurationElement2[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointBindingConfiguration] = str + str2 + ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpsBindingSuffix;
				ConfigurationElement configurationElement3 = ExchangeServiceVDirHelper.EwsAutodiscMWA.TryFindEndpointByNameAndContract(collection, ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpEndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSContract);
				if (configurationElement3 == null)
				{
					errorHandler(new LocalizedException(Strings.CouldNotFindElementWithTwoAttributes(ExchangeServiceVDirHelper.EwsAutodiscMWA.Endpoint, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpEndpointName, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointContract, ExchangeServiceVDirHelper.EwsAutodiscMWA.EWSContract)), ErrorCategory.InvalidOperation, null);
				}
				configurationElement3[ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointBindingConfiguration] = str + str2 + ExchangeServiceVDirHelper.EwsAutodiscMWA.HttpBindingSuffix;
			}

			private const string CertificateEndpointBindingConfiguration = "emwsCustomCertificateConfiguration";

			private const string CertificateBasicEndpointBindingConfiguration = "emwsBasicCertificateConfiguration";

			private const string WsSecurityEnabledKey = "WsSecurityEndpointEnabled";

			private const string WsSecuritySymmetricKeyEnabledKey = "WsSecuritySymmetricKeyEndpointEnabled";

			private const string WsSecurityX509CertEnabledKey = "WsSecurityX509CertEndpointEnabled";

			private static readonly Uri StandardEndpointUri = new Uri(string.Empty, UriKind.Relative);

			private static readonly Uri WSSEndpointUri = new Uri("wssecurity", UriKind.Relative);

			private static readonly Uri WSSSymmetricKeyEndpointUri = new Uri("wssecurity/symmetrickey", UriKind.Relative);

			private static readonly Uri WSSX509CertEndpointUri = new Uri("wssecurity/x509cert", UriKind.Relative);

			private static readonly string ServicesSectionName = "system.serviceModel/services";

			private static readonly string EWSWSSecurityHttpBinding = "EWSWSSecurityHttpBinding";

			private static readonly string EWSWSSecurityHttpsBinding = "EWSWSSecurityHttpsBinding";

			private static readonly string EWSWSSecuritySymmetricKeyHttpBinding = "EWSWSSecuritySymmetricKeyHttpBinding";

			private static readonly string EWSWSSecuritySymmetricKeyHttpsBinding = "EWSWSSecuritySymmetricKeyHttpsBinding";

			private static readonly string EWSWSSecurityX509CertHttpBinding = "EWSWSSecurityX509CertHttpBinding";

			private static readonly string EWSWSSecurityX509CertHttpsBinding = "EWSWSSecurityX509CertHttpsBinding";

			private static readonly string WSSecurityHttpBinding = "WSSecurityHttpBinding";

			private static readonly string WSSecurityHttpsBinding = "WSSecurityHttpsBinding";

			private static readonly string WSSecuritySymmetricKeyHttpBinding = "WSSecuritySymmetricKeyHttpBinding";

			private static readonly string WSSecuritySymmetricKeyHttpsBinding = "WSSecuritySymmetricKeyHttpsBinding";

			private static readonly string WSSecurityX509CertHttpBinding = "WSSecurityX509CertHttpBinding";

			private static readonly string WSSecurityX509CertHttpsBinding = "WSSecurityX509CertHttpsBinding";

			private static readonly string EWSServiceName = "Microsoft.Exchange.Services.Wcf.EWSService";

			private static readonly string AutoDServiceName = "Microsoft.Exchange.Autodiscover.WCF.AutodiscoverService";

			private static readonly string ServiceNameLegacyAutoD = "Microsoft.Exchange.Autodiscover.WCF.LegacyAutodiscoverService";

			private static readonly string EWSContract = "Microsoft.Exchange.Services.Wcf.IEWSContract";

			private static readonly string AutoDContract = "Microsoft.Exchange.Autodiscover.WCF.IAutodiscover";

			private static readonly string LegacyAutoDContract = "Microsoft.Exchange.Autodiscover.WCF.ILegacyAutodiscover";

			private static readonly string HttpEndpointName = "Http";

			private static readonly string HttpsEndpointName = "Https";

			private static readonly string HttpBindingSuffix = "HttpBinding";

			private static readonly string HttpsBindingSuffix = "HttpsBinding";

			private static readonly string CustomBindingString = "customBinding";

			private static readonly string Endpoint = "endpoint";

			private static readonly string EndpointBindingConfiguration = "bindingConfiguration";

			private static readonly string EndpointName = "name";

			private static readonly string EndpointContract = "contract";

			private static readonly string EndpointAddress = "address";

			private static readonly string EndpointBinding = "binding";

			private static readonly string EndpointService = "service";

			private static readonly string NameAttribute = "name";

			private static readonly Uri CertificateEndpointUri = new Uri("Certificate", UriKind.Relative);

			private static readonly Uri CertificateBasicEndpointUri = new Uri("basicHttpCertificate", UriKind.Relative);

			private static AuthenticationSchemes[] AnonymousScheme = new AuthenticationSchemes[]
			{
				AuthenticationSchemes.Anonymous
			};

			private static AuthenticationSchemes[] AnonymousBasicNegotiateSchemes = new AuthenticationSchemes[]
			{
				AuthenticationSchemes.Anonymous,
				AuthenticationSchemes.Basic,
				AuthenticationSchemes.Negotiate
			};

			internal enum EndpointProtocol
			{
				Autodiscover,
				Ews,
				OwaEws
			}
		}

		private class WebBinding
		{
			public string Protocol;

			public string Info;
		}
	}
}
