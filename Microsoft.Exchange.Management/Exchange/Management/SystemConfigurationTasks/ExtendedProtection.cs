using System;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.ServiceModel.Configuration;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class ExtendedProtection
	{
		public static void Validate(Task task, ExchangeVirtualDirectory exchangeVirtualDirectory)
		{
			if (exchangeVirtualDirectory.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				return;
			}
			if (task.Fields.Contains("ExtendedProtectionSPNList"))
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)task.Fields["ExtendedProtectionSPNList"];
				if (multiValuedProperty != null)
				{
					foreach (string text in multiValuedProperty)
					{
						if (!text.StartsWith("HTTP/", StringComparison.InvariantCultureIgnoreCase))
						{
							task.WriteError(new ArgumentException(Strings.ErrorExtendedProtectionSPNHasToStartWithHTTP(text), "ExtendedProtectionSPNList"), ErrorCategory.InvalidArgument, exchangeVirtualDirectory.Identity);
						}
					}
				}
			}
		}

		public static void LoadFromMetabase(ExchangeVirtualDirectory exchangeVirtualDirectory, Task task)
		{
			if (exchangeVirtualDirectory.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				return;
			}
			string metabasePath = exchangeVirtualDirectory.MetabasePath;
			ExtendedProtectionTokenCheckingMode extendedProtectionTokenCheckingMode;
			MultiValuedProperty<ExtendedProtectionFlag> extendedProtectionFlags;
			MultiValuedProperty<string> extendedProtectionSPNList;
			ExtendedProtection.LoadFromMetabase(metabasePath, exchangeVirtualDirectory.Identity, task, out extendedProtectionTokenCheckingMode, out extendedProtectionFlags, out extendedProtectionSPNList);
			exchangeVirtualDirectory[ExchangeVirtualDirectorySchema.ExtendedProtectionTokenChecking] = extendedProtectionTokenCheckingMode;
			exchangeVirtualDirectory.ExtendedProtectionFlags = extendedProtectionFlags;
			exchangeVirtualDirectory.ExtendedProtectionSPNList = extendedProtectionSPNList;
		}

		public static void LoadFromMetabase(string metabasePath, ObjectId identity, Task task, out ExtendedProtectionTokenCheckingMode extendedProtectionTokenChecking, out MultiValuedProperty<ExtendedProtectionFlag> extendedProtectionFlags, out MultiValuedProperty<string> extendedProtectionSPNList)
		{
			extendedProtectionTokenChecking = ExtendedProtectionTokenCheckingMode.None;
			extendedProtectionFlags = new MultiValuedProperty<ExtendedProtectionFlag>();
			extendedProtectionSPNList = new MultiValuedProperty<string>();
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(metabasePath, (task != null) ? new Task.TaskErrorLoggingReThrowDelegate(task.WriteError) : null, identity, false))
			{
				if (directoryEntry != null)
				{
					string text;
					string str;
					string str2;
					if (ExtendedProtection.GetServerWebSiteAndPath(metabasePath, out text, out str, out str2))
					{
						using (ServerManager serverManager = ServerManager.OpenRemote(text))
						{
							Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
							if (applicationHostConfiguration != null)
							{
								ConfigurationSection section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/windowsAuthentication", "/" + str + str2);
								if (section != null)
								{
									ConfigurationElement configurationElement = section.ChildElements["extendedProtection"];
									if (configurationElement != null)
									{
										object attributeValue = configurationElement.GetAttributeValue("tokenChecking");
										if (attributeValue != null && attributeValue is int)
										{
											extendedProtectionTokenChecking = (ExtendedProtectionTokenCheckingMode)attributeValue;
										}
										object attributeValue2 = configurationElement.GetAttributeValue("flags");
										if (attributeValue2 != null && attributeValue2 is int)
										{
											extendedProtectionFlags.Add((ExtendedProtectionFlag)attributeValue2);
										}
										ConfigurationElementCollection collection = configurationElement.GetCollection();
										if (collection != null)
										{
											foreach (ConfigurationElement configurationElement2 in collection)
											{
												if (configurationElement2.Schema.Name == "spn")
												{
													string item = configurationElement2.GetAttributeValue("name").ToString();
													extendedProtectionSPNList.Add(item);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static void CommitToMetabase(ExchangeVirtualDirectory exchangeVirtualDirectory, Task task)
		{
			if (exchangeVirtualDirectory.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				return;
			}
			bool flag = task.Fields.IsModified("ExtendedProtectionTokenChecking");
			bool flag2 = task.Fields.IsModified("ExtendedProtectionFlags");
			bool flag3 = task.Fields.IsModified("ExtendedProtectionSPNList");
			if (flag || flag2 || flag3)
			{
				string metabasePath = exchangeVirtualDirectory.MetabasePath;
				using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(metabasePath, new Task.TaskErrorLoggingReThrowDelegate(task.WriteError), exchangeVirtualDirectory.Identity))
				{
					if (directoryEntry != null)
					{
						string text;
						string text2;
						string text3;
						if (ExtendedProtection.GetServerWebSiteAndPath(metabasePath, out text, out text2, out text3))
						{
							if (!ExtendedProtection.WebConfigReflectionHelper.IsExtendedProtectionSupported(task))
							{
								TaskLogger.Trace("Warning: ExtendedProtectionPolicy has not been added to HttpTransportElement of web.config.  Install the operating system update(s) specified in KB {0} onto server {1} and try again.", new object[]
								{
									"981205",
									text
								});
								task.WriteWarning(Strings.WarnExtendedProtectionIsNotEnabled(text, "981205"));
							}
							else
							{
								string text4 = "/" + text2 + text3;
								using (ServerManager serverManager = ServerManager.OpenRemote(text))
								{
									Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
									if (applicationHostConfiguration != null)
									{
										ConfigurationSection section = applicationHostConfiguration.GetSection("system.webServer/security/authentication/windowsAuthentication", text4);
										if (section != null)
										{
											ConfigurationElement configurationElement = section.ChildElements["extendedProtection"];
											if (configurationElement != null)
											{
												if (flag)
												{
													int num = (int)exchangeVirtualDirectory[ExchangeVirtualDirectorySchema.ExtendedProtectionTokenChecking];
													configurationElement.SetAttributeValue("tokenChecking", num);
												}
												if (flag2)
												{
													int num2 = (int)exchangeVirtualDirectory[ExchangeVirtualDirectorySchema.ExtendedProtectionFlags];
													configurationElement.SetAttributeValue("flags", num2);
												}
												if (flag3)
												{
													ConfigurationElementCollection collection = configurationElement.GetCollection();
													collection.Clear();
													foreach (string text5 in exchangeVirtualDirectory.ExtendedProtectionSPNList)
													{
														ConfigurationElement configurationElement2 = collection.CreateElement("spn");
														configurationElement2.SetAttributeValue("name", text5);
														collection.Add(configurationElement2);
													}
												}
												ExtendedProtection.WebConfigReflectionHelper.CommitToWebConfigMWA(exchangeVirtualDirectory, task, text3, text2, text, flag, flag3);
												serverManager.CommitChanges();
												return;
											}
											TaskLogger.Trace("Warning: Extended protection has not been enabled.  Install the operating system update specified in KB {0} onto server {1} and try again.", new object[]
											{
												"973917",
												text
											});
											task.WriteWarning(Strings.WarnExtendedProtectionIsNotEnabled(text, "973917"));
											return;
										}
									}
									TaskLogger.Trace("Error:ApplicationHost.config or {0} is not found for virtual directory with metabase path '{1}' and local path '{2}'.", new object[]
									{
										"system.webServer/security/authentication/windowsAuthentication",
										metabasePath,
										text4
									});
									task.WriteError(new ArgumentException(Strings.ErrorAppHostOrWindowsAuthenticationNotFound("system.webServer/security/authentication/windowsAuthentication", metabasePath, text4)), ErrorCategory.ObjectNotFound, exchangeVirtualDirectory.Identity);
								}
							}
						}
					}
				}
			}
		}

		private static bool GetServerWebSiteAndPath(string metabasePath, out string serverName, out string webSiteName, out string appPath)
		{
			serverName = IisUtility.GetHostName(metabasePath);
			int num = metabasePath.IndexOf("/ROOT/");
			if (num > 0)
			{
				num = metabasePath.IndexOf("/", num + 1);
			}
			if (num > 0)
			{
				string text = metabasePath.Substring(0, num);
				if (!string.IsNullOrEmpty(text))
				{
					webSiteName = IisUtility.GetWebSiteName(text);
					appPath = metabasePath.Substring(num);
					return true;
				}
			}
			serverName = string.Empty;
			webSiteName = string.Empty;
			appPath = string.Empty;
			return false;
		}

		private const string WebConfigFileName = "web.config";

		private const string AddServiceNameElement = "add";

		private const string CustomServiceNamesElement = "customServiceNames";

		private const string PolicyEnforcementAttribute = "policyEnforcement";

		private const string ExtendedProtectionPolicyElement = "extendedProtectionPolicy";

		private const string AuthenticationSchemeAttribute = "authenticationScheme";

		private const string HttpsTransportElement = "httpsTransport";

		private const string HttpTransportElement = "httpTransport";

		private const string CustomBindingElement = "customBinding";

		private const string ExtendedProtectionElement = "extendedProtection";

		private const string TokenCheckingAttribute = "tokenChecking";

		private const string FlagsAttribute = "flags";

		private const string SpnNameElement = "spn";

		private const string SpnNameAttribute = "name";

		private const string SpnNamePrefix = "HTTP/";

		private const string KbForMetabase = "973917";

		private const string KbForWebConfig = "981205";

		private static readonly string BindingsSectionName = "system.serviceModel/bindings";

		private class WebConfigReflectionHelper
		{
			internal static bool IsExtendedProtectionSupported(Task task)
			{
				if (!ExtendedProtection.WebConfigReflectionHelper.initialized)
				{
					try
					{
						ExtendedProtection.WebConfigReflectionHelper.extendedProtectionPolicyPropertyInfo = typeof(HttpTransportElement).GetProperty("ExtendedProtectionPolicy");
						if (ExtendedProtection.WebConfigReflectionHelper.extendedProtectionPolicyPropertyInfo == null)
						{
							return false;
						}
						ExtendedProtection.WebConfigReflectionHelper.extendedProtectionPolicyGetMethodInfo = ExtendedProtection.WebConfigReflectionHelper.extendedProtectionPolicyPropertyInfo.GetGetMethod();
						if (ExtendedProtection.WebConfigReflectionHelper.extendedProtectionPolicyGetMethodInfo == null)
						{
							return false;
						}
						ExtendedProtection.WebConfigReflectionHelper.policyEnforcementPropertyInfo = ExtendedProtection.WebConfigReflectionHelper.extendedProtectionPolicyPropertyInfo.PropertyType.GetProperty("PolicyEnforcement");
						if (ExtendedProtection.WebConfigReflectionHelper.policyEnforcementPropertyInfo == null)
						{
							return false;
						}
						ExtendedProtection.WebConfigReflectionHelper.never = new object[]
						{
							Enum.Parse(ExtendedProtection.WebConfigReflectionHelper.policyEnforcementPropertyInfo.PropertyType, "Never")
						};
						ExtendedProtection.WebConfigReflectionHelper.whenSupported = new object[]
						{
							Enum.Parse(ExtendedProtection.WebConfigReflectionHelper.policyEnforcementPropertyInfo.PropertyType, "WhenSupported")
						};
						ExtendedProtection.WebConfigReflectionHelper.always = new object[]
						{
							Enum.Parse(ExtendedProtection.WebConfigReflectionHelper.policyEnforcementPropertyInfo.PropertyType, "Always")
						};
						ExtendedProtection.WebConfigReflectionHelper.policyEnforcementSetMethodInfo = ExtendedProtection.WebConfigReflectionHelper.policyEnforcementPropertyInfo.GetSetMethod();
						if (ExtendedProtection.WebConfigReflectionHelper.policyEnforcementSetMethodInfo == null)
						{
							return false;
						}
						ExtendedProtection.WebConfigReflectionHelper.customServiceNamesPropertyInfo = ExtendedProtection.WebConfigReflectionHelper.extendedProtectionPolicyPropertyInfo.PropertyType.GetProperty("CustomServiceNames");
						if (ExtendedProtection.WebConfigReflectionHelper.customServiceNamesPropertyInfo == null)
						{
							return false;
						}
						ExtendedProtection.WebConfigReflectionHelper.customServiceNamesGetMethodInfo = ExtendedProtection.WebConfigReflectionHelper.customServiceNamesPropertyInfo.GetGetMethod();
						bool flag = false;
						bool flag2 = false;
						foreach (MethodInfo methodInfo in ExtendedProtection.WebConfigReflectionHelper.customServiceNamesPropertyInfo.PropertyType.GetMethods())
						{
							ParameterInfo[] parameters = methodInfo.GetParameters();
							if (methodInfo.Name == "Clear" && parameters.Length == 0)
							{
								ExtendedProtection.WebConfigReflectionHelper.customServiceNamesClearMethodInfo = methodInfo;
								flag2 = true;
							}
							else if (methodInfo.Name == "Add" && parameters.Length == 1 && parameters[0].ParameterType.Name == "ServiceNameElement")
							{
								ExtendedProtection.WebConfigReflectionHelper.customServiceNamesAddMethodInfo = methodInfo;
								ExtendedProtection.WebConfigReflectionHelper.serviceNameElementNamePropertyInfo = parameters[0].ParameterType.GetProperty("Name");
								if (ExtendedProtection.WebConfigReflectionHelper.serviceNameElementNamePropertyInfo == null)
								{
									return false;
								}
								ExtendedProtection.WebConfigReflectionHelper.serviceNameElementNameSetMethodInfo = ExtendedProtection.WebConfigReflectionHelper.serviceNameElementNamePropertyInfo.GetSetMethod();
								if (ExtendedProtection.WebConfigReflectionHelper.serviceNameElementNameSetMethodInfo == null)
								{
									return false;
								}
								ExtendedProtection.WebConfigReflectionHelper.serviceNameElementConstructorInfo = parameters[0].ParameterType.GetConstructor(new Type[0]);
								if (ExtendedProtection.WebConfigReflectionHelper.serviceNameElementConstructorInfo == null)
								{
									return false;
								}
								flag = true;
							}
						}
						ExtendedProtection.WebConfigReflectionHelper.isExtendedProtectionSupported = (flag2 && flag);
						ExtendedProtection.WebConfigReflectionHelper.initialized = true;
					}
					catch (Exception ex)
					{
						TaskLogger.Trace("Error: Unexpected exception on accessing ExtendedProtection properties: {0}, ExtendedProtection would not be handled", new object[]
						{
							ex.ToString()
						});
						task.WriteWarning(Strings.ExceptionOccured(ex.ToString()));
					}
				}
				return ExtendedProtection.WebConfigReflectionHelper.isExtendedProtectionSupported;
			}

			private static void SetPolicyEnforcement(object extendedProtectionPolicyProperty, ExtendedProtectionTokenCheckingMode tokenCheckingMode)
			{
				object[] parameters = (tokenCheckingMode == ExtendedProtectionTokenCheckingMode.Allow) ? ExtendedProtection.WebConfigReflectionHelper.whenSupported : ((tokenCheckingMode == ExtendedProtectionTokenCheckingMode.Require) ? ExtendedProtection.WebConfigReflectionHelper.always : ExtendedProtection.WebConfigReflectionHelper.never);
				ExtendedProtection.WebConfigReflectionHelper.policyEnforcementSetMethodInfo.Invoke(extendedProtectionPolicyProperty, parameters);
			}

			private static void SetServiceNames(object extendedProtectionPolicyProperty, MultiValuedProperty<string> spnList)
			{
				object obj = ExtendedProtection.WebConfigReflectionHelper.customServiceNamesGetMethodInfo.Invoke(extendedProtectionPolicyProperty, null);
				ExtendedProtection.WebConfigReflectionHelper.customServiceNamesClearMethodInfo.Invoke(obj, null);
				foreach (string text in spnList)
				{
					object obj2 = ExtendedProtection.WebConfigReflectionHelper.serviceNameElementConstructorInfo.Invoke(null);
					ExtendedProtection.WebConfigReflectionHelper.serviceNameElementNameSetMethodInfo.Invoke(obj2, new object[]
					{
						text
					});
					ExtendedProtection.WebConfigReflectionHelper.customServiceNamesAddMethodInfo.Invoke(obj, new object[]
					{
						obj2
					});
				}
			}

			internal static void CommitToWebConfigMWA(ExchangeVirtualDirectory exchangeVirtualDirectory, Task task, string path, string site, string server, bool isTokenCheckingSpecified, bool isSpnListSpecified)
			{
				if (ExtendedProtection.WebConfigReflectionHelper.isExtendedProtectionSupported && (isTokenCheckingSpecified || isSpnListSpecified))
				{
					using (ServerManager serverManager = ServerManager.OpenRemote(server))
					{
						Configuration webConfiguration = serverManager.GetWebConfiguration(site, path);
						string configurationFilePath = ExtendedProtection.WebConfigReflectionHelper.GetConfigurationFilePath(serverManager, site, path);
						if (!string.IsNullOrEmpty(configurationFilePath) && File.Exists(configurationFilePath))
						{
							ConfigurationSection section = webConfiguration.GetSection(ExtendedProtection.BindingsSectionName);
							if (section != null)
							{
								ConfigurationElement configurationElement = section.ChildElements["customBinding"];
								if (configurationElement != null)
								{
									ConfigurationElementCollection collection = configurationElement.GetCollection();
									if (collection != null)
									{
										bool flag = false;
										foreach (ConfigurationElement configurationElement2 in collection)
										{
											ConfigurationElement configurationElement3 = configurationElement2.ChildElements["httpsTransport"];
											ConfigurationAttribute attribute = configurationElement3.GetAttribute("authenticationScheme");
											if (attribute.IsInheritedFromDefaultValue)
											{
												configurationElement3 = configurationElement2.ChildElements["httpTransport"];
												attribute = configurationElement3.GetAttribute("authenticationScheme");
											}
											if (!attribute.IsInheritedFromDefaultValue && (int)attribute.Value == 2)
											{
												ConfigurationElement configurationElement4 = configurationElement3.ChildElements["extendedProtectionPolicy"];
												if (configurationElement4 != null)
												{
													if (isTokenCheckingSpecified)
													{
														ExtendedProtection.WebConfigReflectionHelper.SetPolicyEnforcementMWA(configurationElement4, exchangeVirtualDirectory.ExtendedProtectionTokenChecking);
													}
													if (isSpnListSpecified)
													{
														ExtendedProtection.WebConfigReflectionHelper.SetServiceNamesMWA(configurationElement4, exchangeVirtualDirectory.ExtendedProtectionSPNList);
													}
													flag = true;
												}
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
			}

			private static void SetPolicyEnforcementMWA(ConfigurationElement extendedProtectionPolicyElement, ExtendedProtectionTokenCheckingMode tokenCheckingMode)
			{
				object[] array = (tokenCheckingMode == ExtendedProtectionTokenCheckingMode.Allow) ? ExtendedProtection.WebConfigReflectionHelper.whenSupported : ((tokenCheckingMode == ExtendedProtectionTokenCheckingMode.Require) ? ExtendedProtection.WebConfigReflectionHelper.always : ExtendedProtection.WebConfigReflectionHelper.never);
				extendedProtectionPolicyElement["policyEnforcement"] = array[0];
			}

			private static void SetServiceNamesMWA(ConfigurationElement extendedProtectionPolicyElement, MultiValuedProperty<string> spnList)
			{
				ConfigurationElement configurationElement = extendedProtectionPolicyElement.ChildElements["customServiceNames"];
				if (configurationElement == null)
				{
					return;
				}
				ConfigurationElementCollection collection = configurationElement.GetCollection();
				if (collection == null)
				{
					return;
				}
				collection.Clear();
				foreach (string text in spnList)
				{
					ConfigurationElement configurationElement2 = collection.CreateElement("add");
					configurationElement2["name"] = text;
					collection.Add(configurationElement2);
				}
			}

			private static string GetConfigurationFilePath(ServerManager serverManager, string site, string path)
			{
				if (serverManager.Sites == null || serverManager.Sites.Count == 0)
				{
					return null;
				}
				Site site2 = serverManager.Sites[site];
				if (site2 == null || site2.Applications == null || site2.Applications.Count == 0)
				{
					return null;
				}
				Application application = site2.Applications[path];
				if (application == null || application.VirtualDirectories == null || application.VirtualDirectories.Count == 0)
				{
					return null;
				}
				return Path.Combine(Environment.ExpandEnvironmentVariables(application.VirtualDirectories[0].PhysicalPath), "web.config");
			}

			private static bool isExtendedProtectionSupported = false;

			private static PropertyInfo extendedProtectionPolicyPropertyInfo;

			private static MethodInfo extendedProtectionPolicyGetMethodInfo;

			private static PropertyInfo policyEnforcementPropertyInfo;

			private static MethodInfo policyEnforcementSetMethodInfo;

			private static PropertyInfo customServiceNamesPropertyInfo;

			private static MethodInfo customServiceNamesGetMethodInfo;

			private static MethodInfo customServiceNamesClearMethodInfo;

			private static MethodInfo customServiceNamesAddMethodInfo;

			private static ConstructorInfo serviceNameElementConstructorInfo;

			private static PropertyInfo serviceNameElementNamePropertyInfo;

			private static MethodInfo serviceNameElementNameSetMethodInfo;

			private static object[] whenSupported;

			private static object[] always;

			private static object[] never;

			private static bool initialized = false;
		}
	}
}
