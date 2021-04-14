using System;
using System.Configuration;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Win32;

namespace Microsoft.Exchange.CommonHelpProvider
{
	public static class HelpProvider
	{
		public static void Initialize(HelpProvider.HelpAppName appName)
		{
			HelpProvider.LoadBaseUrl(appName);
			HelpProvider.initializedViaCmdlet = false;
		}

		public static void InitializeViaCmdlet(HelpProvider.HelpAppName appName, RunspaceServerSettingsPresentationObject runspaceServerSettings, MonadConnectionInfo monadConnectionInfo)
		{
			HelpProvider.LoadBaseUrlViaCmdlet(appName, runspaceServerSettings, monadConnectionInfo);
			HelpProvider.initializedViaCmdlet = true;
		}

		public static Uri ConstructHelpRenderingUrl()
		{
			return HelpProvider.UrlConstructHelper("default", new string[]
			{
				HelpProvider.GetLoginInfo()
			});
		}

		public static Uri ConstructHelpRenderingUrl(string helpAttributeId)
		{
			string contentId = HelpProvider.GetAppQualifier() + helpAttributeId;
			return HelpProvider.UrlConstructHelper(contentId, new string[]
			{
				HelpProvider.GetLoginInfo()
			});
		}

		public static Uri ConstructHelpRenderingUrl(ErrorRecord errorRecord)
		{
			string text = "e=" + "ms.exch.err." + errorRecord.Exception.GetType();
			return HelpProvider.UrlConstructHelper("ms.exch.err.default", new string[]
			{
				text,
				HelpProvider.GetLoginInfo()
			});
		}

		public static Uri ConstructHelpRenderingUrl(int lcid, HelpProvider.OwaHelpExperience owaExp, string helpId, HelpProvider.RenderingMode mode, string optionalServerParams, OrganizationProperties organizationProperties)
		{
			Uri owaBaseUrl = HelpProvider.GetOwaBaseUrl(owaExp);
			string owaAppQualifier = HelpProvider.GetOwaAppQualifier(owaExp);
			string text = optionalServerParams;
			if (!string.IsNullOrEmpty(text) && !text.StartsWith("&"))
			{
				text = "&" + text;
			}
			string text2 = HelpProvider.OwaLightNamespace;
			if (owaExp != HelpProvider.OwaHelpExperience.Light)
			{
				if (organizationProperties == null || string.IsNullOrEmpty(organizationProperties.ServicePlan))
				{
					text2 = HelpProvider.OwaPremiumNamespace;
				}
				else if (organizationProperties.ServicePlan.StartsWith(HelpProvider.OwaMsoProfessionalServicePlanPrefix, StringComparison.InvariantCultureIgnoreCase))
				{
					text2 = (HelpProvider.IsGallatin() ? HelpProvider.OwaMsoProfessionalGallatinNamespace : HelpProvider.OwaMsoProfessionalNamespace);
				}
				else
				{
					text2 = (HelpProvider.IsGallatin() ? HelpProvider.OwaMsoEnterpriseGallatinNamespace : HelpProvider.OwaMsoEnterpriseNamespace);
				}
			}
			else
			{
				text = string.Empty;
			}
			string text3 = string.Empty;
			if (helpId != null)
			{
				text3 = "&helpid=" + owaAppQualifier + (helpId.Equals(string.Empty) ? "{0}" : helpId);
			}
			string text4 = "15";
			int num = HelpProvider.applicationVersion.IndexOf('.');
			if (num > 0)
			{
				text4 = HelpProvider.applicationVersion.Substring(0, num);
			}
			string uriString = string.Format("{0}?p1={1}&clid={2}&ver={3}&v={4}&mode={5}{6}{7}", new object[]
			{
				owaBaseUrl.ToString(),
				text2,
				lcid.ToString(),
				text4,
				HelpProvider.applicationVersion,
				HelpProvider.officeRedirModes[(int)mode],
				text3,
				text
			});
			return new Uri(uriString);
		}

		public static Uri ConstructHelpRenderingUrlWithQualifierHelpId(string appQualifier, string helpId)
		{
			if (string.IsNullOrEmpty(appQualifier))
			{
				return null;
			}
			if (string.IsNullOrEmpty(helpId))
			{
				return null;
			}
			return new Uri(string.Format(HelpProvider.GetBaseUrl().ToString(), appQualifier, helpId));
		}

		public static Uri ConstructTenantEventUrl(string source, string eventId)
		{
			return HelpProvider.UrlConstructHelper("ms.exch.evt.default", new string[]
			{
				HelpProvider.GetEventParam(source, string.Empty, eventId),
				HelpProvider.GetLoginInfo()
			});
		}

		public static Uri ConstructTenantEventUrl(string source, string category, string eventId)
		{
			return HelpProvider.UrlConstructHelper("ms.exch.evt.default", new string[]
			{
				HelpProvider.GetEventParam(source, category, eventId),
				HelpProvider.GetLoginInfo()
			});
		}

		public static Uri ConstructTenantEventUrl(string source, string category, string eventId, string locale)
		{
			return HelpProvider.ConstructFinalHelpUrl(HelpProvider.baseUrl, "ms.exch.evt.default", locale, new string[]
			{
				HelpProvider.GetEventParam(source, category, eventId),
				HelpProvider.GetLoginInfo()
			});
		}

		public static bool TryGetErrorAssistanceUrl(LocalizedException exception, out Uri helpUrl)
		{
			helpUrl = null;
			if (!HelpProvider.ShouldConstructHelpUrlForException(exception))
			{
				return false;
			}
			helpUrl = HelpProvider.UrlConstructHelper("ms.exch.err.default", new string[]
			{
				HelpProvider.GetErrorParam(exception),
				HelpProvider.GetLoginInfo()
			});
			return true;
		}

		public static Uri GetBaseUrl()
		{
			return HelpProvider.baseUrl;
		}

		public static Uri GetManagementConsoleFeedbackUrl()
		{
			return HelpProvider.managementConsoleFeedbackUrl;
		}

		public static Uri GetPrivacyStatementUrl()
		{
			return HelpProvider.privacyStatementUrl;
		}

		public static Uri GetPrivacyStatementUrl(OrganizationId organizationId)
		{
			if (organizationId == OrganizationId.ForestWideOrgId)
			{
				return HelpProvider.privacyStatementUrl;
			}
			Uri result = null;
			HelpProviderCache.Item item = HelpProviderCache.Instance.Get(organizationId);
			if (item != null)
			{
				result = item.PrivacyStatementUrl;
			}
			return result;
		}

		public static Uri GetExchange2013PrivacyStatementUrl()
		{
			return HelpProvider.privacyStatementUrlExchange2013;
		}

		public static Uri GetMSOnlinePrivacyStatementUrl()
		{
			return HelpProvider.privacyStatementUrlMSOnline;
		}

		public static bool? GetPrivacyLinkDisplayEnabled(OrganizationId organizationId)
		{
			if (organizationId == OrganizationId.ForestWideOrgId)
			{
				return new bool?(true);
			}
			bool? result = null;
			HelpProviderCache.Item item = HelpProviderCache.Instance.Get(organizationId);
			if (item != null)
			{
				result = item.PrivacyLinkDisplayEnabled;
			}
			return result;
		}

		public static bool TryGetPrivacyStatementUrl(OrganizationId organizationId, out Uri orgPrivacyStatementUrl)
		{
			orgPrivacyStatementUrl = HelpProvider.GetPrivacyStatementUrl(organizationId);
			return orgPrivacyStatementUrl != null;
		}

		public static Uri GetLiveAccountUrl()
		{
			return HelpProvider.windowsLiveAccountUrl;
		}

		public static Uri GetCommunityUrl()
		{
			return HelpProvider.communityUrl;
		}

		public static Uri GetCommunityUrl(OrganizationId organizationId)
		{
			if (organizationId == OrganizationId.ForestWideOrgId)
			{
				return HelpProvider.communityUrl;
			}
			Uri result = null;
			HelpProviderCache.Item item = HelpProviderCache.Instance.Get(organizationId);
			if (item != null)
			{
				result = item.CommunityUrl;
			}
			return result;
		}

		public static bool TryGetCommunityUrl(OrganizationId organizationId, out Uri orgCommunityUrl)
		{
			orgCommunityUrl = HelpProvider.GetCommunityUrl(organizationId);
			return orgCommunityUrl != null;
		}

		public static Uri GetProductFeedbackUrl()
		{
			if (HelpProvider.initializedViaCmdlet)
			{
				return HelpProvider.managementConsoleFeedbackUrl;
			}
			HelpProvider.HelpAppName helpAppName = HelpProvider.callingAppName;
			if (helpAppName == HelpProvider.HelpAppName.Ecp)
			{
				return HelpProvider.controlPanelFeedbackUrl;
			}
			throw new InvalidOperationException(string.Format("callingAppName {0} is not a valid HelpAppName enum. Check caller of Initialize how we get this value.", HelpProvider.callingAppName.ToString()));
		}

		public static Uri GetProductFeedbackUrl(HelpProvider.OwaHelpExperience owaExp)
		{
			switch (owaExp)
			{
			case HelpProvider.OwaHelpExperience.Light:
				return HelpProvider.owaLightFeedbackUrl;
			case HelpProvider.OwaHelpExperience.Premium:
				return HelpProvider.owaPremiumFeedbackUrl;
			default:
				throw new InvalidOperationException("owaExp is not a valid OwaHelpExperience enum. Check caller of Initialize how we get this value.");
			}
		}

		internal static Uri ConstructHelpRenderingUrl(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			return HelpProvider.UrlConstructHelper("default", new string[]
			{
				HelpProvider.GetLoginInfo(),
				HelpProvider.GetServicePlanInfo(rbacConfiguration)
			});
		}

		internal static Uri ConstructHelpRenderingUrl(string helpAttributeId, ExchangeRunspaceConfiguration rbacConfiguration)
		{
			string contentId = HelpProvider.GetAppQualifier() + helpAttributeId;
			return HelpProvider.UrlConstructHelper(contentId, new string[]
			{
				HelpProvider.GetLoginInfo(),
				HelpProvider.GetServicePlanInfo(rbacConfiguration)
			});
		}

		internal static Uri ConstructHelpRenderingUrl(ErrorRecord errorRecord, ExchangeRunspaceConfiguration rbacConfiguration)
		{
			string text = "e=" + "ms.exch.err." + errorRecord.Exception.GetType();
			return HelpProvider.UrlConstructHelper("ms.exch.err.default", new string[]
			{
				text,
				HelpProvider.GetLoginInfo(),
				HelpProvider.GetServicePlanInfo(rbacConfiguration)
			});
		}

		internal static bool TryGetErrorAssistanceUrl(LocalizedException exception, ExchangeRunspaceConfiguration rbacConfiguration, out Uri helpUrl)
		{
			helpUrl = null;
			if (!HelpProvider.ShouldConstructHelpUrlForException(exception))
			{
				return false;
			}
			helpUrl = HelpProvider.UrlConstructHelper("ms.exch.err.default", new string[]
			{
				HelpProvider.GetErrorParam(exception),
				HelpProvider.GetLoginInfo(),
				HelpProvider.GetServicePlanInfo(rbacConfiguration)
			});
			return true;
		}

		internal static bool TryGetErrorAssistanceUrl(LocalizedException exception, OrganizationProperties organizationProperties, out Uri helpUrl)
		{
			helpUrl = null;
			if (!HelpProvider.ShouldConstructHelpUrlForException(exception))
			{
				return false;
			}
			helpUrl = HelpProvider.UrlConstructHelper("ms.exch.err.default", new string[]
			{
				HelpProvider.GetErrorParam(exception),
				HelpProvider.GetLoginInfo(),
				(organizationProperties == null) ? string.Empty : HelpProvider.ConstructServicePlanInfo(organizationProperties.ServicePlan)
			});
			return true;
		}

		internal static ExchangeAssistance GetExchangeAssistanceObjectFromAD(OrganizationId organizationId)
		{
			ExchangeAssistance result = null;
			try
			{
				IConfigurationSession configurationSession;
				ADObjectId adobjectId;
				if (organizationId == OrganizationId.ForestWideOrgId)
				{
					configurationSession = HelpProvider.GetOrganizationConfigurationSession(organizationId);
					adobjectId = configurationSession.GetOrgContainerId();
				}
				else
				{
					SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(organizationId);
					if (sharedConfiguration != null)
					{
						adobjectId = sharedConfiguration.SharedConfigurationCU.Id;
						configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sharedConfiguration.GetSharedConfigurationSessionSettings(), 955, "GetExchangeAssistanceObjectFromAD", "f:\\15.00.1497\\sources\\dev\\UA\\src\\HelpProvider\\HelpProvider.cs");
					}
					else
					{
						adobjectId = organizationId.ConfigurationUnit;
						configurationSession = HelpProvider.GetOrganizationConfigurationSession(organizationId);
					}
				}
				ADObjectId childId = adobjectId.GetChildId("ExchangeAssistance").GetChildId(HelpProvider.CurrentVersionExchangeAssistanceContainerName);
				result = configurationSession.Read<ExchangeAssistance>(childId);
			}
			catch (ADTransientException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "ADTransient Exception in LoadBaseURL: {0}", ex.Message);
			}
			catch (ADOperationException ex2)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "ADOperationException in LoadBaseURL: {0}", ex2.Message);
			}
			return result;
		}

		internal static void LoadSetupBaseUrlFromRegistry()
		{
			string baseUrlFromRegistry = HelpProvider.GetBaseUrlFromRegistry(HelpProvider.baseSetupUrlFromRegistryPath, "baseSetupHelpUrl");
			string arg = string.IsNullOrEmpty(baseUrlFromRegistry) ? "http://technet.microsoft.com/library(EXCHG.150)" : baseUrlFromRegistry;
			HelpProvider.baseUrl = new Uri(string.Format("{0}/{{0}}{{1}}.aspx", arg));
		}

		internal static void LoadBpaBaseUrlFromRegistry()
		{
			string baseUrlFromRegistry = HelpProvider.GetBaseUrlFromRegistry(HelpProvider.baseBpaUrlFromRegistryPath, "baseBpaHelpUrl");
			string uriString = string.IsNullOrEmpty(baseUrlFromRegistry) ? "http://technet.microsoft.com/library(EXCHG.150)" : baseUrlFromRegistry;
			HelpProvider.baseUrl = Utilities.NormalizeUrl(new Uri(uriString));
		}

		internal static void LoadComplianceBaseUrl()
		{
			HelpProvider.baseUrl = Utilities.NormalizeUrl(new Uri("http://technet.microsoft.com/library"));
		}

		internal static string GetBaseUrlFromRegistry(string localMachinePath, string keyName)
		{
			if (string.IsNullOrEmpty(localMachinePath) || string.IsNullOrEmpty(keyName))
			{
				return null;
			}
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(localMachinePath))
				{
					if (registryKey != null)
					{
						return (string)registryKey.GetValue(keyName);
					}
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		private static IConfigurationSession GetOrganizationConfigurationSession(OrganizationId organizationId)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, organizationId, false), 1064, "GetOrganizationConfigurationSession", "f:\\15.00.1497\\sources\\dev\\UA\\src\\HelpProvider\\HelpProvider.cs");
			tenantOrTopologyConfigurationSession.SessionSettings.IsSharedConfigChecked = true;
			return tenantOrTopologyConfigurationSession;
		}

		private static string GetAppQualifier()
		{
			switch (HelpProvider.callingAppName)
			{
			case HelpProvider.HelpAppName.Ecp:
				return "ms.exch.eac.";
			case HelpProvider.HelpAppName.Toolbox:
				return "ms.exch.toolbox.";
			case HelpProvider.HelpAppName.TenantMonitoring:
				return "ms.exch.evt.";
			case HelpProvider.HelpAppName.Eap:
				return "ms.exch.err.";
			case HelpProvider.HelpAppName.Bpa:
				return "ms.o365.bpa.";
			case HelpProvider.HelpAppName.Compliance:
				return "ms.o365.cc.";
			}
			throw new InvalidOperationException(string.Format("callingAppName {0} is not a valid HelpAppName enum. Check caller of Initialize how we get this value.", HelpProvider.callingAppName.ToString()));
		}

		private static string GetOwaAppQualifier(HelpProvider.OwaHelpExperience appExp)
		{
			switch (appExp)
			{
			case HelpProvider.OwaHelpExperience.Light:
				return "ms.exch.owal.";
			case HelpProvider.OwaHelpExperience.Premium:
				return "ms.exch.owap.";
			case HelpProvider.OwaHelpExperience.Options:
				return "ms.exch.owao.";
			default:
				throw new ArgumentOutOfRangeException("appExp");
			}
		}

		private static Uri GetOwaBaseUrl(HelpProvider.OwaHelpExperience appExp)
		{
			if (appExp != HelpProvider.OwaHelpExperience.Light)
			{
				return HelpProvider.baseOwaPremiumUrl;
			}
			return HelpProvider.baseOwaLightUrl;
		}

		private static string GetUserHelpLanguage()
		{
			CultureInfo userCulture = HelpProvider.GetUserCulture();
			return userCulture.Name;
		}

		private static CultureInfo GetUserCulture()
		{
			return Thread.CurrentThread.CurrentUICulture;
		}

		private static string GetLoginInfo()
		{
			string result = string.Empty;
			if (!HelpProvider.initializedViaCmdlet)
			{
				result = "l=" + (Utilities.IsMicrosoftHostedOnly() ? "1" : "0");
			}
			return result;
		}

		private static string GetServicePlanInfo(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			return HelpProvider.ConstructServicePlanInfo(Utilities.GetServicePlanName(rbacConfiguration));
		}

		private static string ConstructServicePlanInfo(string servicePlan)
		{
			if (string.IsNullOrEmpty(servicePlan))
			{
				return string.Empty;
			}
			return "s=" + servicePlan;
		}

		private static void LoadBaseUrlViaADSession(HelpProvider.HelpAppName appName)
		{
			ExchangeAssistance exchangeAssistanceObjectFromAD = HelpProvider.GetExchangeAssistanceObjectFromAD(OrganizationId.ForestWideOrgId);
			if (exchangeAssistanceObjectFromAD != null)
			{
				HelpProvider.RegisterChangeNotification(exchangeAssistanceObjectFromAD.Id);
				if (appName.Equals(HelpProvider.HelpAppName.Ecp) || appName.Equals(HelpProvider.HelpAppName.TenantMonitoring))
				{
					if (exchangeAssistanceObjectFromAD.ControlPanelHelpURL != null)
					{
						HelpProvider.baseUrl = Utilities.NormalizeUrl(exchangeAssistanceObjectFromAD.ControlPanelHelpURL);
					}
					if (exchangeAssistanceObjectFromAD.ControlPanelFeedbackEnabled)
					{
						HelpProvider.controlPanelFeedbackUrl = exchangeAssistanceObjectFromAD.ControlPanelFeedbackURL;
					}
					else
					{
						HelpProvider.controlPanelFeedbackUrl = null;
					}
				}
				else if (appName.Equals(HelpProvider.HelpAppName.Owa))
				{
					if (exchangeAssistanceObjectFromAD.OWALightHelpURL != null)
					{
						HelpProvider.baseOwaLightUrl = exchangeAssistanceObjectFromAD.OWALightHelpURL;
					}
					if (exchangeAssistanceObjectFromAD.OWAHelpURL != null)
					{
						HelpProvider.baseOwaPremiumUrl = exchangeAssistanceObjectFromAD.OWAHelpURL;
					}
					if (exchangeAssistanceObjectFromAD.OWAFeedbackEnabled)
					{
						HelpProvider.owaPremiumFeedbackUrl = exchangeAssistanceObjectFromAD.OWAFeedbackURL;
					}
					else
					{
						HelpProvider.owaPremiumFeedbackUrl = null;
					}
					if (exchangeAssistanceObjectFromAD.OWALightFeedbackEnabled)
					{
						HelpProvider.owaLightFeedbackUrl = exchangeAssistanceObjectFromAD.OWALightFeedbackURL;
					}
					else
					{
						HelpProvider.owaLightFeedbackUrl = null;
					}
				}
				if (exchangeAssistanceObjectFromAD.PrivacyLinkDisplayEnabled)
				{
					HelpProvider.privacyStatementUrl = exchangeAssistanceObjectFromAD.PrivacyStatementURL;
				}
				else
				{
					HelpProvider.privacyStatementUrl = null;
				}
				if (exchangeAssistanceObjectFromAD.WindowsLiveAccountURLEnabled)
				{
					HelpProvider.windowsLiveAccountUrl = exchangeAssistanceObjectFromAD.WindowsLiveAccountPageURL;
				}
				else
				{
					HelpProvider.windowsLiveAccountUrl = null;
				}
				if (exchangeAssistanceObjectFromAD.CommunityLinkDisplayEnabled)
				{
					HelpProvider.communityUrl = exchangeAssistanceObjectFromAD.CommunityURL;
					return;
				}
				HelpProvider.communityUrl = null;
			}
		}

		private static void LoadBaseUrlViaCmdlet(HelpProvider.HelpAppName appName, RunspaceServerSettingsPresentationObject runspaceServerSettings, MonadConnectionInfo monadConnectionInfo)
		{
			if (appName != HelpProvider.HelpAppName.Toolbox)
			{
				if (appName != HelpProvider.HelpAppName.Eap)
				{
					throw new InvalidOperationException("appName is not a valid HelpAppName enum. Check caller of Initialize how we get this value.");
				}
			}
			try
			{
				ExchangeAssistance exchangeAssistance = null;
				MonadConnection connection = new MonadConnection("timeout=30", new CommandInteractionHandler(), runspaceServerSettings, monadConnectionInfo);
				using (new OpenConnection(connection))
				{
					using (MonadCommand monadCommand = new MonadCommand("Get-ExchangeAssistanceConfig", connection))
					{
						object[] array = monadCommand.Execute();
						if (array != null && array.Length != 0)
						{
							exchangeAssistance = (array[0] as ExchangeAssistance);
						}
					}
				}
				if (exchangeAssistance != null)
				{
					if (exchangeAssistance.ManagementConsoleHelpURL != null)
					{
						HelpProvider.baseUrl = Utilities.NormalizeUrl(exchangeAssistance.ManagementConsoleHelpURL);
					}
					if (exchangeAssistance.ManagementConsoleFeedbackEnabled)
					{
						HelpProvider.managementConsoleFeedbackUrl = exchangeAssistance.ManagementConsoleFeedbackURL;
					}
					else
					{
						HelpProvider.managementConsoleFeedbackUrl = null;
					}
					if (exchangeAssistance.PrivacyLinkDisplayEnabled)
					{
						HelpProvider.privacyStatementUrl = exchangeAssistance.PrivacyStatementURL;
					}
					else
					{
						HelpProvider.privacyStatementUrl = null;
					}
					if (exchangeAssistance.WindowsLiveAccountURLEnabled)
					{
						HelpProvider.windowsLiveAccountUrl = exchangeAssistance.WindowsLiveAccountPageURL;
					}
					else
					{
						HelpProvider.windowsLiveAccountUrl = null;
					}
					if (exchangeAssistance.CommunityLinkDisplayEnabled)
					{
						HelpProvider.communityUrl = exchangeAssistance.CommunityURL;
					}
					else
					{
						HelpProvider.communityUrl = null;
					}
				}
			}
			catch (CommandExecutionException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "CommandExecution Exception in LoadBaseURL: {0}", ex.Message);
			}
			catch (CmdletInvocationException ex2)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "CmdletInvocationException Exception in LoadBaseURL: {0}", ex2.Message);
			}
			catch (PipelineStoppedException ex3)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "PipelineStopped Exception in LoadBaseURL: {0}", ex3.Message);
			}
			HelpProvider.callingAppName = appName;
		}

		private static void LoadBaseUrl(HelpProvider.HelpAppName appName)
		{
			switch (appName)
			{
			case HelpProvider.HelpAppName.Ecp:
			case HelpProvider.HelpAppName.Owa:
			case HelpProvider.HelpAppName.TenantMonitoring:
				HelpProvider.LoadBaseUrlViaADSession(appName);
				goto IL_52;
			case HelpProvider.HelpAppName.Setup:
				HelpProvider.LoadSetupBaseUrlFromRegistry();
				goto IL_52;
			case HelpProvider.HelpAppName.Bpa:
				HelpProvider.LoadBpaBaseUrlFromRegistry();
				goto IL_52;
			case HelpProvider.HelpAppName.Compliance:
				HelpProvider.LoadComplianceBaseUrl();
				goto IL_52;
			}
			throw new InvalidOperationException("appName is not a valid HelpAppName enum. Check caller of Initialize how we get this value.");
			IL_52:
			HelpProvider.callingAppName = appName;
		}

		private static Uri ConstructFinalHelpUrl(Uri baseUrl, string contentId, string locale, params string[] queryParams)
		{
			string text = string.Format(baseUrl.ToString(), locale, contentId);
			StringBuilder stringBuilder = new StringBuilder(text.ToString(), HelpProvider.initialCapacity);
			stringBuilder.Append("?");
			stringBuilder.Append("v=");
			stringBuilder.Append(HelpProvider.applicationVersion);
			foreach (string value in queryParams)
			{
				if (!string.IsNullOrEmpty(value))
				{
					stringBuilder.Append("&");
					stringBuilder.Append(value);
				}
			}
			return new Uri(stringBuilder.ToString());
		}

		private static Uri UrlConstructHelper(string contentId, params string[] queryParams)
		{
			return HelpProvider.ConstructFinalHelpUrl(HelpProvider.baseUrl, contentId, HelpProvider.GetUserHelpLanguage(), queryParams);
		}

		private static Uri LocaleBasedUrlConstructHelper(string contentId, string locale, params string[] queryParams)
		{
			return HelpProvider.ConstructFinalHelpUrl(HelpProvider.baseUrl, contentId, locale, queryParams);
		}

		private static Uri UrlConstructHelperOwa(string contentId, HelpProvider.OwaHelpExperience owaExp, params string[] queryParams)
		{
			return HelpProvider.ConstructFinalHelpUrl(HelpProvider.GetOwaBaseUrl(owaExp), contentId, HelpProvider.GetUserHelpLanguage(), queryParams);
		}

		private static string GetEventParam(string source, string category, string eventId)
		{
			if (string.IsNullOrEmpty(category))
			{
				return string.Format("{0}{1}{2}.{3}", new object[]
				{
					"ev=",
					"ms.exch.evt.",
					source.Replace(" ", string.Empty),
					eventId
				});
			}
			return string.Format("{0}{1}{2}.{3}.{4}", new object[]
			{
				"ev=",
				"ms.exch.evt.",
				source.Replace(" ", string.Empty),
				category,
				eventId
			});
		}

		private static string GetErrorParam(LocalizedException exception)
		{
			return "e=" + "ms.exch.err." + exception.StringId;
		}

		private static bool ShouldConstructHelpUrlForException(LocalizedException exception)
		{
			return exception.LocalizedString.ShowAssistanceInfoInUIIfError && !string.IsNullOrEmpty(exception.StringId);
		}

		private static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			if (HelpProvider.notificationRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(HelpProvider.notificationRequestCookie);
				HelpProvider.notificationRequestCookie = null;
			}
		}

		private static void RegisterChangeNotification(ADObjectId adObjectId)
		{
			if (HelpProvider.notificationRequestCookie == null)
			{
				ADNotificationCallback callback = new ADNotificationCallback(HelpProvider.OnExchangeAssiatnceConfigChanged);
				HelpProvider.notificationRequestCookie = ADNotificationAdapter.RegisterChangeNotification<ExchangeAssistance>(adObjectId, callback);
				AppDomain.CurrentDomain.DomainUnload += HelpProvider.CurrentDomain_DomainUnload;
			}
		}

		private static void OnExchangeAssiatnceConfigChanged(ADNotificationEventArgs args)
		{
			ExchangeAssistance exchangeAssistanceObjectFromADWithRetry = HelpProvider.GetExchangeAssistanceObjectFromADWithRetry(OrganizationId.ForestWideOrgId);
			if (exchangeAssistanceObjectFromADWithRetry != null)
			{
				switch (HelpProvider.callingAppName)
				{
				case HelpProvider.HelpAppName.Ecp:
					if (exchangeAssistanceObjectFromADWithRetry.ControlPanelHelpURL != null)
					{
						Interlocked.Exchange<Uri>(ref HelpProvider.baseUrl, Utilities.NormalizeUrl(exchangeAssistanceObjectFromADWithRetry.ControlPanelHelpURL));
						return;
					}
					break;
				case HelpProvider.HelpAppName.Owa:
					if (exchangeAssistanceObjectFromADWithRetry.OWAHelpURL != null)
					{
						Interlocked.Exchange<Uri>(ref HelpProvider.baseOwaPremiumUrl, exchangeAssistanceObjectFromADWithRetry.OWAHelpURL);
					}
					if (exchangeAssistanceObjectFromADWithRetry.OWALightHelpURL != null)
					{
						Interlocked.Exchange<Uri>(ref HelpProvider.baseOwaLightUrl, exchangeAssistanceObjectFromADWithRetry.OWALightHelpURL);
					}
					break;
				default:
					return;
				}
			}
		}

		private static ExchangeAssistance GetExchangeAssistanceObjectFromADWithRetry(OrganizationId organizationId)
		{
			int num = 0;
			ExchangeAssistance exchangeAssistance = null;
			while (exchangeAssistance == null && num < 6)
			{
				exchangeAssistance = HelpProvider.GetExchangeAssistanceObjectFromAD(organizationId);
				if (exchangeAssistance == null)
				{
					num++;
					Thread.Sleep(10000);
				}
			}
			return exchangeAssistance;
		}

		private static bool IsGallatin()
		{
			bool flag = false;
			return bool.TryParse(ConfigurationManager.AppSettings["IsGallatin"], out flag) && flag;
		}

		public const string SetupQualifier = "ms.exch.setup.";

		public const string SetupReadinessQualifier = "ms.exch.setupreadiness.";

		public const string BPAQualifier = "ms.o365.bpa.";

		public const string ComplianceQualifier = "ms.o365.cc.";

		private const string DefaultRegistryBaseUrl = "http://technet.microsoft.com/library(EXCHG.150)";

		private const string BaseSetupUrlFromRegistryKeyName = "baseSetupHelpUrl";

		private const string BaseBpaUrlFromRegistryKeyName = "baseBpaHelpUrl";

		private const string ECPQualifier = "ms.exch.eac.";

		private const string OWALightQualifier = "ms.exch.owal.";

		private const string OWAPremiumQualifier = "ms.exch.owap.";

		private const string OWAOptionsQualifier = "ms.exch.owao.";

		private const string ToolboxQualifier = "ms.exch.toolbox.";

		private const string ERRQualifier = "ms.exch.err.";

		private const string EventQualifier = "ms.exch.evt.";

		private const string QueryParamErrorID = "e=";

		private const string QueryParamLoginInfo = "l=";

		private const string QueryParamAppVersion = "v=";

		private const string QueryParamServicePlan = "s=";

		private const string QueryParamF1Type = "t=";

		private const string QueryParamEventID = "ev=";

		private const string EHCERRPage = "ms.exch.err.default";

		private const string EHCEVTPage = "ms.exch.evt.default";

		private const int MaxRetryAttempts = 6;

		private const int RetryInterval = 10000;

		private const string ExchangeAssistanceContainerName = "ExchangeAssistance";

		private const string IsGallatinConfigKey = "IsGallatin";

		private const string DefaultComplianceBaseUrl = "http://technet.microsoft.com/library";

		private static readonly string[] officeRedirModes = new string[]
		{
			"Desktop",
			"Metro",
			"Mobile"
		};

		private static readonly string OwaLightNamespace = "OLWALIGHT";

		private static readonly string OwaPremiumNamespace = "OLWAENDUSER";

		private static readonly string OwaMsoProfessionalNamespace = "OLWAO365P";

		private static readonly string OwaMsoEnterpriseNamespace = "OLWAO365E";

		private static readonly string OwaMsoProfessionalGallatinNamespace = "OLWAO365Pg";

		private static readonly string OwaMsoEnterpriseGallatinNamespace = "OLWO365Eg";

		private static readonly string OwaMsoProfessionalServicePlanPrefix = "BPOS_L";

		private static readonly string CurrentVersionExchangeAssistanceContainerName = "ExchangeAssistance" + 15;

		private static readonly string applicationVersion = typeof(HelpProvider).GetApplicationVersion();

		private static readonly string baseSetupUrlFromRegistryPath = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\{1}", "v15", "Setup");

		private static readonly string baseBpaUrlFromRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\BPA";

		private static bool initializedViaCmdlet = false;

		private static int initialCapacity = 2000;

		private static Uri baseUrl = new Uri("http://technet.microsoft.com/{0}/library/{1}(EXCHG.150).aspx");

		private static Uri baseOwaLightUrl = new Uri("http://o15.officeredir.microsoft.com/r/rlidOfficeWebHelp");

		private static Uri baseOwaPremiumUrl = new Uri("http://o15.officeredir.microsoft.com/r/rlidOfficeWebHelp");

		private static Uri privacyStatementUrl = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253085");

		private static Uri privacyStatementUrlExchange2013 = new Uri("http://go.microsoft.com/fwlink/?LinkId=260597");

		private static Uri privacyStatementUrlMSOnline = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=259417");

		private static Uri windowsLiveAccountUrl = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253084");

		private static Uri controlPanelFeedbackUrl = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253080");

		private static Uri managementConsoleFeedbackUrl = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253081");

		private static Uri owaPremiumFeedbackUrl = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253083");

		private static Uri owaLightFeedbackUrl = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253087");

		private static Uri communityUrl = new Uri("http://go.microsoft.com/fwlink/p/?LinkId=253086");

		private static HelpProvider.HelpAppName callingAppName;

		private static ADNotificationRequestCookie notificationRequestCookie;

		public enum HelpAppName
		{
			Ecp,
			Owa,
			Toolbox,
			TenantMonitoring,
			Setup,
			Eap,
			Bpa,
			Compliance
		}

		public enum OwaHelpExperience
		{
			Light,
			Premium,
			Options
		}

		public enum RenderingMode
		{
			Mouse,
			TWide,
			TNarrow
		}
	}
}
