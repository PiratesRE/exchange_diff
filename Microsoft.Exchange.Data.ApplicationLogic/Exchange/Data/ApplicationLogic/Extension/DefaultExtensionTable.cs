using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.ApplicationLogic.Owa;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class DefaultExtensionTable
	{
		internal DefaultExtensionTable(IExchangePrincipal exchangePrincipal, string scenario = "GetDefaultExtensions")
		{
			this.defaultExtensions = DefaultExtensionTable.ReadDefaultExtensionData(exchangePrincipal);
			DefaultExtensionTable.scenario = scenario + ".GetDefaultExtensions";
		}

		public Dictionary<string, ExtensionData> DefaultExtensions
		{
			get
			{
				return this.defaultExtensions;
			}
		}

		public static string GetInstalledOwaVersion()
		{
			if (DefaultExtensionTable.owaVersion == null)
			{
				Exception ex = null;
				try
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(OwaConstants.OwaSetupInstallKey))
					{
						DefaultExtensionTable.owaVersion = RegistryReader.Instance.GetValue<string>(registryKey, null, "OwaVersion", string.Empty);
						DefaultExtensionTable.Tracer.TraceDebug<string>(0L, "Read {0} for owaVersion", DefaultExtensionTable.owaVersion);
					}
				}
				catch (SecurityException ex2)
				{
					ex = ex2;
				}
				catch (IOException ex3)
				{
					ex = ex3;
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					DefaultExtensionTable.Tracer.TraceError<Exception>(0L, "Cannot read owa version from registry due to Exception {0}. Using empty string", ex);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_OwaVersionRetrievalFailed, ex.GetType().ToString(), new object[]
					{
						"GetInstalledOwaVersion",
						ExtensionDiagnostics.GetLoggedExceptionString(ex)
					});
					DefaultExtensionTable.owaVersion = string.Empty;
				}
			}
			return DefaultExtensionTable.owaVersion;
		}

		internal static bool TryUpdateDefaultExtensionPath(IExchangePrincipal exchangePrincipal, string urlElementName, ExtensionData extensionData, out Exception exception)
		{
			return extensionData.TryUpdateSourceLocation(exchangePrincipal, urlElementName, out exception, new ExtensionDataHelper.TryModifySourceLocationDelegate(DefaultExtensionTable.TryModifySourceLocation));
		}

		private static bool TryModifySourceLocation(IExchangePrincipal exchangePrincipal, XmlAttribute xmlAttribute, ExtensionData extensionData, out Exception exception)
		{
			string value;
			if (!DefaultExtensionTable.TryConvertToCompleteUrl(exchangePrincipal, xmlAttribute.Value, extensionData.ExtensionId, out value, out exception))
			{
				return false;
			}
			xmlAttribute.Value = value;
			return true;
		}

		internal static bool TryConvertToCompleteUrl(IExchangePrincipal exchangePrincipal, string url, string extensionId, out string completeUrl, out Exception exception)
		{
			exception = null;
			completeUrl = url;
			if (!url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("/"))
			{
				string contentDeliveryNetworkEndpoint = DefaultExtensionTable.GetContentDeliveryNetworkEndpoint();
				if (string.IsNullOrEmpty(contentDeliveryNetworkEndpoint))
				{
					string domain = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.Domain;
					string text = HttpUtility.UrlEncode(string.Format("{0}@{1}", exchangePrincipal.MailboxInfo.MailboxGuid, domain));
					if (DefaultExtensionTable.baseOwaUrl == null && !DefaultExtensionTable.TryFindOwaServiceUrl(exchangePrincipal, out exception))
					{
						return false;
					}
					completeUrl = string.Format("{0}/{1}/prem/{2}/ext/def/{3}/{4}", new object[]
					{
						DefaultExtensionTable.baseOwaUrl,
						text,
						DefaultExtensionTable.GetInstalledOwaVersion(),
						extensionId,
						url
					});
				}
				else
				{
					completeUrl = string.Format("{0}/owa/prem/{1}/ext/def/{2}/{3}", new object[]
					{
						contentDeliveryNetworkEndpoint,
						DefaultExtensionTable.GetInstalledOwaVersion(),
						extensionId,
						url
					});
				}
			}
			return true;
		}

		private static string GetContentDeliveryNetworkEndpoint()
		{
			try
			{
				string text = ConfigurationManager.AppSettings["ContentDeliveryNetworkEndpoint"] ?? string.Empty;
				return text.TrimEnd(new char[]
				{
					'/'
				});
			}
			catch (ConfigurationErrorsException ex)
			{
				DefaultExtensionTable.Tracer.TraceError<ConfigurationErrorsException>(0L, "Cannot read configuration for ContentDeliveryNetworkEndpoint: {0}", ex);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_GetContentDeliveryNetworkEndpointFailed, null, new object[]
				{
					DefaultExtensionTable.scenario,
					ExtensionDiagnostics.GetLoggedExceptionString(ex)
				});
			}
			return null;
		}

		private static bool TryFindOwaServiceUrl(IExchangePrincipal exchangePrincipal, out Exception exception)
		{
			exception = null;
			Uri frontEndOwaUrl;
			try
			{
				frontEndOwaUrl = FrontEndLocator.GetFrontEndOwaUrl(exchangePrincipal);
			}
			catch (ServerNotFoundException ex)
			{
				exception = ex;
				return false;
			}
			DefaultExtensionTable.baseOwaUrl = frontEndOwaUrl.ToString().TrimEnd(new char[]
			{
				'/'
			});
			return true;
		}

		private static Dictionary<string, ExtensionData> ReadDefaultExtensionData(IExchangePrincipal exchangePrincipal)
		{
			Dictionary<string, ExtensionData> dictionary = new Dictionary<string, ExtensionData>();
			string text = ExchangeSetupContext.InstallPath + "ClientAccess\\owa\\" + string.Format("\\prem\\{0}\\ext\\def\\", DefaultExtensionTable.GetInstalledOwaVersion());
			string[] array = null;
			try
			{
				if (!Directory.Exists(text))
				{
					DefaultExtensionTable.Tracer.TraceError<string>(0L, "Default extension path {0} does not exist", text);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_DefaultExtensionPathNotExist, null, new object[]
					{
						DefaultExtensionTable.scenario
					});
					return dictionary;
				}
				array = Directory.GetDirectories(text);
			}
			catch (Exception ex)
			{
				DefaultExtensionTable.Tracer.TraceError<Exception>(0L, "Failed to access default extension folder. ", ex);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_DefaultExtensionFolderAccessFailed, ex.GetType().ToString(), new object[]
				{
					DefaultExtensionTable.scenario,
					ExtensionDiagnostics.GetLoggedExceptionString(ex)
				});
				return dictionary;
			}
			foreach (string text2 in array)
			{
				try
				{
					string path = text2 + "\\" + "manifest.xml";
					if (File.Exists(path))
					{
						using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
						{
							ExtensionData extensionData = ExtensionData.ParseOsfManifest(fileStream, null, null, ExtensionType.Default, ExtensionInstallScope.Default, true, DisableReasonType.NotDisabled, string.Empty, null);
							extensionData.ProvidedTo = ClientExtensionProvidedTo.Everyone;
							extensionData.IsMandatory = false;
							extensionData.IsEnabledByDefault = true;
							extensionData.InstalledByVersion = ExchangeSetupContext.InstalledVersion;
							if (exchangePrincipal != null)
							{
								Exception arg = null;
								if (!DefaultExtensionTable.TryUpdateDefaultExtensionPath(exchangePrincipal, "SourceLocation", extensionData, out arg))
								{
									DefaultExtensionTable.Tracer.TraceError<Exception>(0L, "Skip one default extension because entry point path cannot be updated: {0}", arg);
									goto IL_22D;
								}
								if (!DefaultExtensionTable.TryUpdateDefaultExtensionPath(exchangePrincipal, "IconUrl", extensionData, out arg))
								{
									DefaultExtensionTable.Tracer.TraceError<Exception>(0L, "Skip one default extension because icon path cannot be updated: {0}", arg);
									goto IL_22D;
								}
								if (!DefaultExtensionTable.TryUpdateDefaultExtensionPath(exchangePrincipal, "HighResolutionIconUrl", extensionData, out arg))
								{
									DefaultExtensionTable.Tracer.TraceError<Exception>(0L, "Skip one default extension because hi-res icon path cannot be updated: {0}", arg);
									goto IL_22D;
								}
							}
							dictionary[ExtensionDataHelper.FormatExtensionId(extensionData.ExtensionId)] = extensionData;
						}
					}
				}
				catch (Exception ex2)
				{
					DefaultExtensionTable.Tracer.TraceError<Exception>(0L, "Skip one default extension because of error. {0}", ex2);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_DefaultExtensionRetrievalFailed, text2, new object[]
					{
						DefaultExtensionTable.scenario,
						text2,
						ExtensionDiagnostics.GetLoggedExceptionString(ex2)
					});
				}
				IL_22D:;
			}
			return dictionary;
		}

		public const string ManifestName = "manifest.xml";

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private static string baseOwaUrl;

		private static string scenario;

		private static string owaVersion;

		private volatile Dictionary<string, ExtensionData> defaultExtensions;
	}
}
