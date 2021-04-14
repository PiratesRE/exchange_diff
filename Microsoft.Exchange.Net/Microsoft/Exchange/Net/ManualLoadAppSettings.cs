using System;
using System.Configuration;
using System.DirectoryServices;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Configuration;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal class ManualLoadAppSettings : IAppSettings
	{
		internal ManualLoadAppSettings(string connectionUri)
		{
			if (string.IsNullOrWhiteSpace(connectionUri))
			{
				throw new ArgumentException("connectionUri is null/Whitespace");
			}
			this.LoadWebConfig(connectionUri);
		}

		public string PodRedirectTemplate { get; private set; }

		public string SiteRedirectTemplate { get; private set; }

		public bool TenantRedirectionEnabled { get; private set; }

		public bool RedirectionEnabled { get; private set; }

		public int MaxPowershellAppPoolConnections { get; private set; }

		public string ProvisioningCacheIdentification { get; private set; }

		public string DedicatedMailboxPlansCustomAttributeName { get; private set; }

		public bool DedicatedMailboxPlansEnabled { get; private set; }

		public bool ShouldShowFismaBanner { get; private set; }

		public int ThreadPoolMaxThreads { get; private set; }

		public int ThreadPoolMaxCompletionPorts { get; private set; }

		public PSLanguageMode PSLanguageMode { get; private set; }

		public string SupportedEMCVersions { get; set; }

		public bool FailFastEnabled { get; private set; }

		public int PSMaximumReceivedObjectSizeMB { get; private set; }

		public int PSMaximumReceivedDataSizePerCommandMB { get; private set; }

		public string VDirName { get; private set; }

		public string WebSiteName { get; private set; }

		public string ConfigurationFilePath { get; private set; }

		public string LogSubFolderName { get; private set; }

		public bool LogEnabled { get; private set; }

		public string LogDirectoryPath { get; private set; }

		public TimeSpan LogFileAgeInDays { get; private set; }

		public int MaxLogDirectorySizeInGB { get; private set; }

		public int MaxLogFileSizeInMB { get; private set; }

		public int ThresholdToLogActivityLatency { get; private set; }

		public int LogCPUMemoryIntervalInMinutes { get; private set; }

		public TimeSpan SidsCacheTimeoutInHours { get; private set; }

		public int ClientAccessRulesLimit { get; private set; }

		public int MaxCmdletRetryCnt { get; private set; }

		private static string GetConfigurationElementStringValue(KeyValueConfigurationElement valueElement, string defaultValue)
		{
			if (valueElement == null)
			{
				return defaultValue;
			}
			return valueElement.Value;
		}

		private static TimeSpan GetConfigurationElementTimeSpanValue(KeyValueConfigurationElement valueElement, TimeSpan defaultValue, TimeSpanUnit unit)
		{
			if (valueElement == null)
			{
				return defaultValue;
			}
			TimeSpan result = defaultValue;
			string value = valueElement.Value;
			int num;
			if (int.TryParse(value, out num))
			{
				switch (unit)
				{
				case TimeSpanUnit.Seconds:
					result = TimeSpan.FromSeconds((double)num);
					break;
				case TimeSpanUnit.Minutes:
					result = TimeSpan.FromMinutes((double)num);
					break;
				case TimeSpanUnit.Hours:
					result = TimeSpan.FromHours((double)num);
					break;
				case TimeSpanUnit.Days:
					result = TimeSpan.FromDays((double)num);
					break;
				}
			}
			return result;
		}

		private static T GetConfigurationElementValue<T>(KeyValueConfigurationElement valueElement, T defaultValue, ManualLoadAppSettings.TryParseDelegate<T> tryParseDelegate)
		{
			if (tryParseDelegate == null)
			{
				throw new ArgumentNullException("tryParseDelegate can't be null.");
			}
			if (valueElement == null)
			{
				return defaultValue;
			}
			T result;
			if (tryParseDelegate(valueElement.Value, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private void LoadWebConfig(string connectionUrl)
		{
			Diagnostics.ExecuteAndLog("ManualLoadAppSettings.LoadWebConfig", true, null, EventLogConstants.NetEventLogger, CommonEventLogConstants.Tuple_UnhandledException, ExTraceGlobals.AppSettingsTracer, null, delegate(Exception ex)
			{
				AuthZLogger.SafeAppendGenericError("LoadWebConfig", ex.ToString(), true);
			}, delegate()
			{
				ExTraceGlobals.AppSettingsTracer.TraceDebug<string>((long)this.GetHashCode(), "Load web.config for Url {0}.", connectionUrl);
				Uri uri = new Uri(connectionUrl, UriKind.Absolute);
				string vdirPathFromUriLocalPath = ManualLoadAppSettings.GetVDirPathFromUriLocalPath(uri);
				string host = uri.Host;
				int port = uri.Port;
				string text;
				Configuration configuration = this.OpenWebConfig(host, port, vdirPathFromUriLocalPath, out text);
				ExTraceGlobals.AppSettingsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "webSiteName = {0}, vdirPath = {1}.", text, vdirPathFromUriLocalPath);
				this.VDirName = vdirPathFromUriLocalPath;
				this.ConfigurationFilePath = configuration.FilePath;
				this.WebSiteName = text;
				this.ReadRemotePSMaxLimitParameters(configuration, vdirPathFromUriLocalPath);
				KeyValueConfigurationCollection keyValueConfigurationCollection = (configuration == null) ? new KeyValueConfigurationCollection() : configuration.AppSettings.Settings;
				this.PodRedirectTemplate = ManualLoadAppSettings.GetConfigurationElementStringValue(keyValueConfigurationCollection["PodRedirectTemplate"], null);
				this.SiteRedirectTemplate = ManualLoadAppSettings.GetConfigurationElementStringValue(keyValueConfigurationCollection["SiteRedirectTemplate"], null);
				this.TenantRedirectionEnabled = ManualLoadAppSettings.GetConfigurationElementValue<bool>(keyValueConfigurationCollection["TenantRedirectionEnabled"], false, new ManualLoadAppSettings.TryParseDelegate<bool>(bool.TryParse));
				this.RedirectionEnabled = ManualLoadAppSettings.GetConfigurationElementValue<bool>(keyValueConfigurationCollection["RedirectionEnabled"], true, new ManualLoadAppSettings.TryParseDelegate<bool>(bool.TryParse));
				this.MaxPowershellAppPoolConnections = ManualLoadAppSettings.GetConfigurationElementValue<int>(keyValueConfigurationCollection["MaxPowershellAppPoolConnections"], 0, new ManualLoadAppSettings.TryParseDelegate<int>(int.TryParse));
				this.ProvisioningCacheIdentification = ManualLoadAppSettings.GetConfigurationElementStringValue(keyValueConfigurationCollection["ProvisioningCacheIdentification"], null);
				this.DedicatedMailboxPlansCustomAttributeName = ManualLoadAppSettings.GetConfigurationElementStringValue(keyValueConfigurationCollection["DedicatedMailboxPlansCustomAttributeName"], null);
				this.DedicatedMailboxPlansEnabled = ManualLoadAppSettings.GetConfigurationElementValue<bool>(keyValueConfigurationCollection["DedicatedMailboxPlansEnabled"], false, new ManualLoadAppSettings.TryParseDelegate<bool>(bool.TryParse));
				this.ShouldShowFismaBanner = ManualLoadAppSettings.GetConfigurationElementValue<bool>(keyValueConfigurationCollection["ShouldShowFismaBanner"], false, new ManualLoadAppSettings.TryParseDelegate<bool>(bool.TryParse));
				this.ThreadPoolMaxThreads = ManualLoadAppSettings.GetConfigurationElementValue<int>(keyValueConfigurationCollection["ThreadPool.MaxWorkerThreads"], AppSettings.DefaultThreadPoolMaxThreads, new ManualLoadAppSettings.TryParseDelegate<int>(int.TryParse));
				this.ThreadPoolMaxCompletionPorts = ManualLoadAppSettings.GetConfigurationElementValue<int>(keyValueConfigurationCollection["ThreadPool.MaxCompletionPortThreads"], AppSettings.DefaultThreadPoolMaxCompletionPorts, new ManualLoadAppSettings.TryParseDelegate<int>(int.TryParse));
				this.PSLanguageMode = ManualLoadAppSettings.GetConfigurationElementValue<PSLanguageMode>(keyValueConfigurationCollection["PSLanguageMode"], PSLanguageMode.NoLanguage, new ManualLoadAppSettings.TryParseDelegate<PSLanguageMode>(Enum.TryParse<PSLanguageMode>));
				this.SupportedEMCVersions = ManualLoadAppSettings.GetConfigurationElementStringValue(keyValueConfigurationCollection["SupportedEMCVersions"], null);
				this.FailFastEnabled = ManualLoadAppSettings.GetConfigurationElementValue<bool>(keyValueConfigurationCollection["FailFastEnabled"], false, new ManualLoadAppSettings.TryParseDelegate<bool>(bool.TryParse));
				this.LogSubFolderName = ManualLoadAppSettings.GetConfigurationElementStringValue(keyValueConfigurationCollection["LogSubFolderName"], "Others");
				this.LogEnabled = ManualLoadAppSettings.GetConfigurationElementValue<bool>(keyValueConfigurationCollection["LogEnabled"], true, new ManualLoadAppSettings.TryParseDelegate<bool>(bool.TryParse));
				this.LogDirectoryPath = ManualLoadAppSettings.GetConfigurationElementStringValue(keyValueConfigurationCollection["LogDirectoryPath"], null);
				this.LogFileAgeInDays = ManualLoadAppSettings.GetConfigurationElementTimeSpanValue(keyValueConfigurationCollection["LogFileAgeInDays"], AppSettings.DefaultLogFileAgeInDays, TimeSpanUnit.Days);
				this.MaxLogDirectorySizeInGB = ManualLoadAppSettings.GetConfigurationElementValue<int>(keyValueConfigurationCollection["MaxLogDirectorySizeInGB"], 1, new ManualLoadAppSettings.TryParseDelegate<int>(int.TryParse));
				this.MaxLogFileSizeInMB = ManualLoadAppSettings.GetConfigurationElementValue<int>(keyValueConfigurationCollection["MaxLogFileSizeInMB"], 10, new ManualLoadAppSettings.TryParseDelegate<int>(int.TryParse));
				this.ThresholdToLogActivityLatency = ManualLoadAppSettings.GetConfigurationElementValue<int>(keyValueConfigurationCollection["ThresholdToLogActivityLatency"], 1000, new ManualLoadAppSettings.TryParseDelegate<int>(int.TryParse));
				this.LogCPUMemoryIntervalInMinutes = ManualLoadAppSettings.GetConfigurationElementValue<int>(keyValueConfigurationCollection["LogCPUMemoryIntervalInMinutes"], AppSettings.DefaultLogCPUMemoryIntervalInMinutes, new ManualLoadAppSettings.TryParseDelegate<int>(int.TryParse));
				this.SidsCacheTimeoutInHours = ManualLoadAppSettings.GetConfigurationElementTimeSpanValue(keyValueConfigurationCollection["SidsCacheTimeoutInHours"], AppSettings.DefaultSidsCacheTimeoutInHours, TimeSpanUnit.Hours);
				this.ClientAccessRulesLimit = ManualLoadAppSettings.GetConfigurationElementValue<int>(keyValueConfigurationCollection["ClientAccessRulesLimit"], AppSettings.DefaultClientAccessRulesLimit, new ManualLoadAppSettings.TryParseDelegate<int>(int.TryParse));
				this.MaxCmdletRetryCnt = ManualLoadAppSettings.GetConfigurationElementValue<int>(keyValueConfigurationCollection["MaxCmdletRetryCnt"], AppSettings.DefaultMaxCmdletRetryCnt, new ManualLoadAppSettings.TryParseDelegate<int>(int.TryParse));
				if (this.MaxCmdletRetryCnt < 0)
				{
					this.MaxCmdletRetryCnt = 0;
				}
				ExTraceGlobals.AppSettingsTracer.TraceDebug((long)this.GetHashCode(), "this.PodRedirectTemplate = {0}, this.SiteRedirectTemplate = {1}, this.TenantRedirectionEnabled = {2}, this.RedirectionEnabled = {3}, this.MaxPowershellAppPoolConnections = {4}, this.ProvisioningCacheIdentification = {5}, this.ShouldShowFismaBanner = {6}, this.ThreadPoolMaxThreads = {7}, this.ThreadPoolMaxCompletionPorts  = {8}, this.PSLanguageMode = {9}, this.SupportedEMCVersions = {10}, this.FailFastEnabled = {11}, this.LogSubFolderName = {12}, this.LogEnabled = {13}, this.LogDirectoryPath = {14}, this.LogFileAgeInDays = {15}, this.MaxLogDirectorySizeInGB = {16}, this.MaxLogFileSizeInMB = {17}, this.ThresholdToLogActivityLatency = {18}, this.DedicatedMailboxPlansCustomAttributeName = {19}, this.DedicatedMailboxPlansEnabled = {20}, this.LogCPUMemoryIntervalInMinutes = {21}, this.SidsCacheTimeoutInHours={22} this.ClientAccessRulesLimit={23}, this.MaxCmdletRetryCnt={24}", new object[]
				{
					this.PodRedirectTemplate,
					this.SiteRedirectTemplate,
					this.TenantRedirectionEnabled,
					this.RedirectionEnabled,
					this.MaxPowershellAppPoolConnections,
					this.ProvisioningCacheIdentification,
					this.ShouldShowFismaBanner,
					this.ThreadPoolMaxThreads,
					this.ThreadPoolMaxCompletionPorts,
					this.PSLanguageMode,
					this.SupportedEMCVersions,
					this.FailFastEnabled,
					this.LogSubFolderName,
					this.SupportedEMCVersions,
					this.FailFastEnabled,
					this.LogSubFolderName,
					this.LogEnabled,
					this.LogDirectoryPath,
					this.LogFileAgeInDays,
					this.MaxLogDirectorySizeInGB,
					this.MaxLogFileSizeInMB,
					this.ThresholdToLogActivityLatency,
					this.DedicatedMailboxPlansCustomAttributeName,
					this.DedicatedMailboxPlansEnabled,
					this.LogCPUMemoryIntervalInMinutes,
					this.SidsCacheTimeoutInHours,
					this.ClientAccessRulesLimit,
					this.MaxCmdletRetryCnt
				});
			});
		}

		private Configuration OpenWebConfig(string host, int port, string vdirPath, out string webSiteName)
		{
			ExTraceGlobals.AppSettingsTracer.TraceDebug<string, int, string>((long)this.GetHashCode(), "Host = {0}, Port = {1}, vdirPath = {2}.", host, port, vdirPath);
			Configuration result = null;
			webSiteName = this.FindWebSite(host, port, vdirPath);
			try
			{
				if (string.IsNullOrEmpty(webSiteName))
				{
					result = WebConfigurationManager.OpenWebConfiguration(vdirPath);
				}
				else
				{
					result = WebConfigurationManager.OpenWebConfiguration(vdirPath, webSiteName);
				}
			}
			catch (InvalidOperationException ex)
			{
				this.LogIisHierarchy(host, port, vdirPath, webSiteName, ex.ToString());
			}
			return result;
		}

		private void ReadRemotePSMaxLimitParameters(Configuration configuration, string vdirPath)
		{
			this.PSMaximumReceivedObjectSizeMB = AppSettings.DefaultPSMaximumReceivedObjectSizeByte;
			this.PSMaximumReceivedDataSizePerCommandMB = AppSettings.DefaultPSMaximumReceivedDataSizePerCommandByte;
			string configSection = configuration.GetSection("system.webServer").SectionInformation.GetRawXml();
			if (string.IsNullOrEmpty(configSection))
			{
				return;
			}
			SafeXmlDocument xmlDoc = new SafeXmlDocument();
			Diagnostics.ExecuteAndLog("ManualLoadAppSettings.ReadRemotePSMaxLimitParameters.xmldoc.LoadXml", false, null, EventLogConstants.NetEventLogger, CommonEventLogConstants.Tuple_NonCrashingException, ExTraceGlobals.AppSettingsTracer, null, delegate(Exception ex)
			{
				AuthZLogger.SafeAppendGenericError("ReadRemotePSMaxLimitParameters.xmldoc.LoadXml", ex.ToString(), true);
			}, delegate()
			{
				xmlDoc.LoadXml(configSection);
			});
			using (XmlNodeList xmlNodeList = xmlDoc.SelectNodes("system.webServer/system.management.wsmanagement.config/PluginModules/OperationsPlugins/Plugin/InitializationParameters/Param"))
			{
				if (xmlNodeList != null)
				{
					foreach (object obj in xmlNodeList)
					{
						XmlElement xmlElement = (XmlElement)obj;
						double num2;
						if (xmlElement.GetAttribute("Name").Equals("PSMaximumReceivedObjectSizeMB"))
						{
							double num;
							if (double.TryParse(xmlElement.GetAttribute("Value"), out num) || double.TryParse(xmlElement.GetAttribute("value"), out num))
							{
								this.PSMaximumReceivedObjectSizeMB = (int)num * 1048576;
							}
						}
						else if (xmlElement.GetAttribute("Name").Equals("PSMaximumReceivedDataSizePerCommandMB") && (double.TryParse(xmlElement.GetAttribute("Value"), out num2) || double.TryParse(xmlElement.GetAttribute("value"), out num2)))
						{
							this.PSMaximumReceivedDataSizePerCommandMB = (int)num2 * 1048576;
						}
					}
					ExTraceGlobals.AppSettingsTracer.TraceDebug<int, int>((long)this.GetHashCode(), "PSMaximumReceivedObjectSizeMB = {0}, PSMaximumReceivedDataSizePerCommandMB = {1}.", this.PSMaximumReceivedObjectSizeMB, this.PSMaximumReceivedDataSizePerCommandMB);
				}
			}
		}

		private string FindWebSite(string hostName, int port, string vdirPath)
		{
			string text = (port == 444) ? "Exchange Back End" : string.Empty;
			text = ((port == 446) ? "Ucc Web Site" : text);
			return Diagnostics.ExecuteAndLog<string>("ManualLoadAppSettings.FindWebSite", false, null, EventLogConstants.NetEventLogger, CommonEventLogConstants.Tuple_NonCrashingException, ExTraceGlobals.AppSettingsTracer, (object ex) => !(ex is COMException), delegate(Exception ex)
			{
				AuthZLogger.SafeAppendGenericError("FindWebSite", ex.ToString(), true);
			}, text, delegate()
			{
				string iisServiceRoot = this.GetIisServiceRoot(hostName);
				using (DirectoryEntry directoryEntry = new DirectoryEntry(iisServiceRoot))
				{
					foreach (object obj in directoryEntry.Children)
					{
						using (DirectoryEntry directoryEntry2 = (DirectoryEntry)obj)
						{
							if (string.CompareOrdinal(directoryEntry2.SchemaClassName, "IIsWebServer") == 0)
							{
								string text2 = (string)directoryEntry2.Properties["ServerComment"].Value;
								string path = string.Format("{0}/ROOT{1}", directoryEntry2.Path, vdirPath);
								if (DirectoryEntry.Exists(path))
								{
									bool flag = text2.Equals("Exchange Back End", StringComparison.OrdinalIgnoreCase);
									bool flag2 = text2.Equals("Ucc Web Site", StringComparison.OrdinalIgnoreCase);
									if ((port == 444 && flag) || (port == 446 && flag2) || (port != 444 && !flag && port != 446 && !flag2))
									{
										return text2;
									}
								}
							}
						}
					}
				}
				return string.Empty;
			});
		}

		private string GetIisServiceRoot(string hostName)
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
			string text = string.Empty;
			foreach (IPAddress ipaddress in hostEntry.AddressList)
			{
				if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					text = ipaddress.ToString();
					break;
				}
			}
			string result;
			if (string.IsNullOrEmpty(text))
			{
				result = string.Format("IIS://{0}/W3SVC", hostName);
			}
			else
			{
				result = string.Format("IIS://{0}/W3SVC", text);
			}
			return result;
		}

		private void LogIisHierarchy(string hostName, int port, string vdirPath, string websiteName, string errorMsg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("LoadWebConfigFailed {0}.", errorMsg);
			stringBuilder.AppendFormat("HostName:{0} Port:{1} VirtualDirectoryPath:{2} WebSiteName:{3}.", new object[]
			{
				hostName,
				port,
				vdirPath,
				websiteName
			});
			string iisServiceRoot = this.GetIisServiceRoot(hostName);
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			if (!string.IsNullOrEmpty(iisServiceRoot))
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(iisServiceRoot))
				{
					foreach (object obj in directoryEntry.Children)
					{
						DirectoryEntry directoryEntry2 = (DirectoryEntry)obj;
						if (directoryEntry2.SchemaClassName.Equals("IIsWebServer", StringComparison.OrdinalIgnoreCase))
						{
							string text = (string)directoryEntry2.Properties["ServerComment"].Value;
							using (DirectoryEntry directoryEntry3 = new DirectoryEntry(directoryEntry2.Path + "/ROOT"))
							{
								foreach (object obj2 in directoryEntry3.Children)
								{
									DirectoryEntry directoryEntry4 = (DirectoryEntry)obj2;
									if (text.Equals("Default Web Site", StringComparison.OrdinalIgnoreCase))
									{
										stringBuilder3.AppendFormat("{0}  ", directoryEntry4.Name);
									}
									else
									{
										stringBuilder2.AppendFormat("{0}  ", directoryEntry4.Name);
									}
								}
							}
						}
					}
					goto IL_198;
				}
			}
			stringBuilder.Append("HostNameNotFound");
			IL_198:
			stringBuilder.AppendFormat("DefaultWebSite:'{0}' ExchangeBackEnd:'{1}'", stringBuilder3.ToString(), stringBuilder2.ToString());
			EventLogConstants.NetEventLogger.LogEvent(CommonEventLogConstants.Tuple_AppSettingLoadException, null, new string[]
			{
				stringBuilder.ToString()
			});
		}

		private static string GetVDirPathFromUriLocalPath(Uri uri)
		{
			string localPath = uri.LocalPath;
			if (string.IsNullOrEmpty(localPath) || localPath[0] != '/')
			{
				return localPath;
			}
			int num = localPath.IndexOf('/', 1);
			if (num == -1)
			{
				return localPath;
			}
			return localPath.Substring(0, num);
		}

		private const string ConfigInitializationParameterPath = "system.webServer/system.management.wsmanagement.config/PluginModules/OperationsPlugins/Plugin/InitializationParameters/Param";

		private delegate bool TryParseDelegate<T>(string value, out T parsedValue);
	}
}
