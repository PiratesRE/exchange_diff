using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.CommonHelpProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.UM.ClientAccess;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class GetOwaUserConfiguration : ServiceCommand<OwaUserConfiguration>
	{
		public GetOwaUserConfiguration(CallContext callContext, PlacesConfigurationCache placesConfigurationCache, IWeatherConfigurationCache weatherConfigurationCache, Action<string, Type> registerType, Func<bool> isWindowsLiveIdAuth) : base(callContext)
		{
			this.placesConfigurationCache = placesConfigurationCache;
			this.weatherConfigurationCache = weatherConfigurationCache;
			this.isWindowsLiveIdAuth = isWindowsLiveIdAuth;
			registerType("GetOwaUserConfiguration", typeof(OwaUserConfigurationLogMetadata));
			registerType("SessionData_GetOwaUserConfiguration", typeof(OwaUserConfigurationLogMetadata));
		}

		internal static bool IsPolicyTipsEnabled(OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				ExTraceGlobals.UserContextCallTracer.TraceError(0L, "Find OrganizationConfig-PolicyTipRules was called with null OrganizationId.");
				return false;
			}
			try
			{
				RuleCollection ruleCollection = ADUtils.GetPolicyTipRulesPerTenantSettings(organizationId).RuleCollection;
				return ruleCollection != null && ruleCollection.Count > 0;
			}
			catch (ADTransientException arg)
			{
				ExTraceGlobals.UserContextCallTracer.TraceError<OrganizationId, ADTransientException>(0L, "Find OrganizationConfig-PolicyTipRules for {0} threw exception: {1}", organizationId, arg);
			}
			catch (DataValidationException arg2)
			{
				ExTraceGlobals.UserContextCallTracer.TraceError<OrganizationId, DataValidationException>(0L, "Find OrganizationConfig-PolicyTipRules for {0} threw exception: {1}", organizationId, arg2);
			}
			catch (DataSourceOperationException arg3)
			{
				ExTraceGlobals.UserContextCallTracer.TraceError<OrganizationId, DataSourceOperationException>(0L, "Find OrganizationConfig-PolicyTipRules for {0} threw exception: {1}", organizationId, arg3);
			}
			catch (TransientException arg4)
			{
				ExTraceGlobals.UserContextCallTracer.TraceError<OrganizationId, TransientException>(0L, "Find OrganizationConfig-PolicyTipRules for {0} threw exception: {1}", organizationId, arg4);
			}
			return false;
		}

		protected virtual bool CheckForForgottenAttachmentsEnabled
		{
			get
			{
				return Globals.CheckForForgottenAttachmentsEnabled;
			}
		}

		protected virtual bool ControlTasksQueueDisabled
		{
			get
			{
				return Globals.ControlTasksQueueDisabled;
			}
		}

		private bool IsLegacyLogOff
		{
			get
			{
				return LogOnSettings.IsLegacyLogOff;
			}
		}

		protected override OwaUserConfiguration InternalExecute()
		{
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			UserConfigurationManager.IAggregationContext aggregationContext = null;
			OwaUserConfiguration result;
			try
			{
				if (!DefaultPageBase.IsRecoveryBoot(base.CallContext.HttpContext))
				{
					using (UserConfigurationManager.IAggregationContext aggregationContext2 = userContext.TryConsumeBootAggregation())
					{
						if (aggregationContext2 != null)
						{
							aggregationContext = mailboxIdentityMailboxSession.UserConfigurationManager.AttachAggregator(aggregationContext2);
						}
						else
						{
							aggregationContext = this.CreateAggregatedConfiguration(userContext, mailboxIdentityMailboxSession);
						}
					}
				}
				UserOptionsType userOptionsType = new UserOptionsType();
				userOptionsType.Load(mailboxIdentityMailboxSession, true, true);
				OwaUserConfiguration owaUserConfiguration = new OwaUserConfiguration();
				owaUserConfiguration.UserOptions = userOptionsType;
				string userAgent = CallContext.Current.HttpContext.Request.UserAgent;
				UserAgent userAgent2 = new UserAgent(userAgent, userContext.FeaturesManager.ClientServerSettings.ChangeLayout.Enabled, base.CallContext.HttpContext.Request.Cookies);
				StorePerformanceCountersCapture countersCapture = StorePerformanceCountersCapture.Start(mailboxIdentityMailboxSession);
				UMSettingsData umSettings = this.ReadAggregatedUMSettingsData(aggregationContext, userContext.ExchangePrincipal);
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.UMClient, countersCapture, true);
				owaUserConfiguration.SessionSettings = new SessionSettingsType(userContext, mailboxIdentityMailboxSession, userAgent2, base.CallContext, umSettings, this.ReadAggregatedOwaHelpUrlData(aggregationContext, Thread.CurrentThread.CurrentUICulture, userContext.MailboxIdentity, userAgent2));
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.SessionSettings, countersCapture, true);
				ConfigurationContext configurationContext = new ConfigurationContext(userContext, aggregationContext);
				string defaultTheme = configurationContext.DefaultTheme;
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.ConfigContext, countersCapture, true);
				owaUserConfiguration.SegmentationSettings = new SegmentationSettingsType(configurationContext);
				owaUserConfiguration.SegmentationSettings.InstantMessage &= !UserAgentUtilities.IsMonitoringRequest(userAgent);
				owaUserConfiguration.SegmentationSettings.InstantMessage &= VdirConfiguration.Instance.InstantMessagingEnabled;
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.SegmentationSettings, countersCapture, true);
				WacConfigData wacData = AttachmentPolicy.ReadAggregatedWacData(userContext, aggregationContext);
				owaUserConfiguration.AttachmentPolicy = configurationContext.AttachmentPolicy.CreateAttachmentPolicyType(userContext, userAgent2, wacData);
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.AttachmentPolicy, countersCapture, true);
				PolicySettingsType policySettingsType = new PolicySettingsType();
				policySettingsType.PlacesEnabled = (this.placesConfigurationCache.IsFeatureEnabled && configurationContext.PlacesEnabled && !PlacesConfigurationCache.IsRestrictedCulture(owaUserConfiguration.SessionSettings.UserCulture));
				policySettingsType.WeatherEnabled = (this.weatherConfigurationCache.IsFeatureEnabled && configurationContext.WeatherEnabled && !this.weatherConfigurationCache.IsRestrictedCulture(owaUserConfiguration.SessionSettings.UserCulture));
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.PlacesWeather, countersCapture, true);
				policySettingsType.DefaultTheme = configurationContext.DefaultTheme;
				policySettingsType.InstantMessagingType = configurationContext.InstantMessagingType;
				policySettingsType.UseGB18030 = configurationContext.UseGB18030;
				policySettingsType.UseISO885915 = configurationContext.UseISO885915;
				policySettingsType.OutboundCharset = configurationContext.OutboundCharset;
				policySettingsType.AllowCopyContactsToDeviceAddressBook = configurationContext.AllowCopyContactsToDeviceAddressBook;
				policySettingsType.AllowOfflineOnString = configurationContext.AllowOfflineOn.ToString();
				policySettingsType.MySiteUrl = configurationContext.MySiteUrl;
				owaUserConfiguration.PolicySettings = policySettingsType;
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.PolicySettings, countersCapture, true);
				owaUserConfiguration.MobileDevicePolicySettings = MobileDevicePolicyDataFactory.GetPolicySettings(this.ReadAggregatedMobileDevicePolicyData(aggregationContext, userContext.ExchangePrincipal));
				owaUserConfiguration.ApplicationSettings = this.GetApplicationSettings();
				owaUserConfiguration.ViewStateConfiguration = new OwaViewStateConfiguration();
				owaUserConfiguration.ViewStateConfiguration.LoadAll(mailboxIdentityMailboxSession);
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.OwaViewStateConfiguration, countersCapture, true);
				OrganizationId organizationId = mailboxIdentityMailboxSession.MailboxOwner.MailboxInfo.OrganizationId;
				this.SetUserConfigPropertiesFromOrganizationConfig(aggregationContext, organizationId, owaUserConfiguration);
				userContext.IsPublicLogon = (owaUserConfiguration.SessionSettings.IsPublicLogon || (owaUserConfiguration.PublicComputersDetectionEnabled && owaUserConfiguration.SessionSettings.IsPublicComputerSession));
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.GetMailTipsLargeAudienceThreshold, countersCapture, true);
				owaUserConfiguration.RetentionPolicyTags = this.GetRetentionPolicyTags(mailboxIdentityMailboxSession);
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.GetRetentionPolicyTags, countersCapture, true);
				try
				{
					owaUserConfiguration.MasterCategoryList = MasterCategoryListHelper.GetMasterCategoryListType(mailboxIdentityMailboxSession, base.CallContext.OwaCulture);
				}
				catch (QuotaExceededException ex)
				{
					ExTraceGlobals.UserContextCallTracer.TraceDebug<string>(0L, "GetOwaUserConfiguration:  Get MasterCategoryList failed. Exception: {0}", ex.Message);
				}
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.GetMasterCategoryListType, countersCapture, true);
				owaUserConfiguration.MaxRecipientsPerMessage = this.GetMaxRecipientsPerMessage();
				OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.GetMaxRecipientsPerMessage, countersCapture, false);
				owaUserConfiguration.RecoverDeletedItemsEnabled = configurationContext.RecoverDeletedItemsEnabled;
				base.CallContext.ProtocolLog.Set(OwaUserConfigurationLogMetadata.UserCulture, owaUserConfiguration.SessionSettings.UserCulture);
				Converter<KeyValuePair<string, string>, string> converter;
				if (userContext.FeaturesManager.ServerSettings.FlightFormat.Enabled)
				{
					converter = ((KeyValuePair<string, string> pair) => "&" + pair.Key + ":" + pair.Value);
				}
				else
				{
					converter = ((KeyValuePair<string, string> pair) => pair.Key + " = " + pair.Value);
				}
				if (userContext.FeaturesManager.ConfigurationSnapshot != null && userContext.FeaturesManager.ClientSettings.OWADiagnostics.Enabled)
				{
					owaUserConfiguration.FlightConfiguration = Array.ConvertAll<KeyValuePair<string, string>, string>(userContext.FeaturesManager.ConfigurationSnapshot.Constraints, converter);
				}
				else
				{
					owaUserConfiguration.FlightConfiguration = new string[0];
				}
				this.ReadInferenceSettings(mailboxIdentityMailboxSession, userContext, owaUserConfiguration);
				if (base.CallContext.IsSmimeInstalled)
				{
					owaUserConfiguration.SmimeAdminSettings = new SmimeAdminSettingsType(this.ReadAggregatedSmimeData(aggregationContext, organizationId));
				}
				VariantConfigurationSnapshot configurationSnapshot = userContext.FeaturesManager.ConfigurationSnapshot;
				if (configurationSnapshot != null)
				{
					IInlineExploreSettings inlineExploreSettings = configurationSnapshot.OwaServer.InlineExploreSettings;
					if (inlineExploreSettings != null)
					{
						owaUserConfiguration.InlineExploreContent = inlineExploreSettings.Content;
					}
				}
				owaUserConfiguration.PolicyTipsEnabled = this.ReadAggregatedPolicyTipsData(aggregationContext, organizationId).IsPolicyTipsEnabled;
				UserContext.ReadAggregatedFlightConfigData(aggregationContext, organizationId);
				this.RecordAggregationStats(aggregationContext);
				result = owaUserConfiguration;
			}
			finally
			{
				this.ValidateAndDisposeAggregatedConfiguration(aggregationContext, mailboxIdentityMailboxSession);
			}
			return result;
		}

		protected ApplicationSettingsType GetApplicationSettings()
		{
			InstrumentationSettings instrumentationSettings = this.GetInstrumentationSettings();
			ApplicationSettingsType applicationSettingsType = new ApplicationSettingsType();
			double num = GetOwaUserConfiguration.random.NextDouble();
			applicationSettingsType.AnalyticsEnabled = (num < (double)instrumentationSettings.AnalyticsProbability);
			applicationSettingsType.CoreAnalyticsEnabled = (applicationSettingsType.AnalyticsEnabled || num < (double)instrumentationSettings.CoreAnalyticsProbability);
			applicationSettingsType.InferenceEnabled = instrumentationSettings.IsInferenceEnabled;
			applicationSettingsType.WatsonEnabled = instrumentationSettings.IsClientWatsonEnabled;
			applicationSettingsType.ManualPerfTracerEnabled = instrumentationSettings.IsManualPerfTracerEnabled;
			applicationSettingsType.ConsoleTracingEnabled = instrumentationSettings.IsConsoleTracingEnabled;
			applicationSettingsType.DefaultTraceLevel = instrumentationSettings.DefaultTraceLevel;
			applicationSettingsType.DefaultPerfTraceLevel = instrumentationSettings.DefaultPerfTraceLevel;
			applicationSettingsType.DefaultJsMvvmPerfTraceLevel = instrumentationSettings.DefaultJsMvvmPerfTraceLevel;
			switch (instrumentationSettings.DefaultTraceLevel)
			{
			case TraceLevel.Error:
				applicationSettingsType.TraceWarningComponents = instrumentationSettings.TraceWarningComponents;
				applicationSettingsType.TracePerfComponents = instrumentationSettings.TracePerfComponents;
				applicationSettingsType.TraceInfoComponents = instrumentationSettings.TraceInfoComponents;
				applicationSettingsType.TraceVerboseComponents = instrumentationSettings.TraceVerboseComponents;
				break;
			case TraceLevel.Warning:
				applicationSettingsType.TracePerfComponents = instrumentationSettings.TracePerfComponents;
				applicationSettingsType.TraceInfoComponents = instrumentationSettings.TraceInfoComponents;
				applicationSettingsType.TraceVerboseComponents = instrumentationSettings.TraceVerboseComponents;
				break;
			case TraceLevel.Perf:
				applicationSettingsType.TraceInfoComponents = instrumentationSettings.TraceInfoComponents;
				applicationSettingsType.TraceVerboseComponents = instrumentationSettings.TraceVerboseComponents;
				break;
			case TraceLevel.Info:
				applicationSettingsType.TraceVerboseComponents = instrumentationSettings.TraceVerboseComponents;
				break;
			}
			applicationSettingsType.InstrumentationSendIntervalSeconds = (int)instrumentationSettings.SendInterval.TotalSeconds;
			if (this.placesConfigurationCache.IsFeatureEnabled)
			{
				applicationSettingsType.MapControlKey = this.placesConfigurationCache.MapControlKey;
				applicationSettingsType.StaticMapUrl = this.placesConfigurationCache.StaticMapUrl;
				applicationSettingsType.DirectionsPageUrl = this.placesConfigurationCache.DirectionsPageUrl;
			}
			applicationSettingsType.CheckForForgottenAttachmentsEnabled = this.CheckForForgottenAttachmentsEnabled;
			applicationSettingsType.ControlTasksQueueDisabled = this.ControlTasksQueueDisabled;
			applicationSettingsType.CloseWindowOnLogout = this.CloseWindowOnLogout();
			applicationSettingsType.IsLegacySignOut = this.IsLegacyLogOff;
			applicationSettingsType.FilterWebBeaconsAndHtmlForms = VdirConfiguration.Instance.FilterWebBeaconsAndHtmlForms;
			applicationSettingsType.FindFolderCountLimit = Global.FindCountLimit;
			return applicationSettingsType;
		}

		protected virtual bool CloseWindowOnLogout()
		{
			return this.IsLegacyLogOff && (!this.isWindowsLiveIdAuth() && !VdirConfiguration.Instance.FormsAuthenticationEnabled) && (VdirConfiguration.Instance.BasicAuthenticationEnabled || VdirConfiguration.Instance.WindowsAuthenticationEnabled || VdirConfiguration.Instance.DigestAuthenticationEnabled);
		}

		protected virtual InstrumentationSettings GetInstrumentationSettings()
		{
			return InstrumentationSettings.Instance;
		}

		protected virtual IEnumerable<PolicyTag> GetPolicyTags(MailboxSession mailboxSession)
		{
			IEnumerable<PolicyTag> result = new List<PolicyTag>();
			if (mailboxSession != null)
			{
				PolicyTagList policyTagList = mailboxSession.GetPolicyTagList((Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType)0);
				if (policyTagList != null)
				{
					result = policyTagList.Values;
				}
			}
			return result;
		}

		protected void SetUserConfigPropertiesFromOrganizationConfig(UserConfigurationManager.IAggregationContext ctx, OrganizationId organizationId, OwaUserConfiguration configuration)
		{
			OwaOrgConfigData owaOrgConfigData = this.ReadAggregatedOrgConfigData(ctx, organizationId);
			configuration.MailTipsLargeAudienceThreshold = owaOrgConfigData.MailTipsLargeAudienceThreshold;
			configuration.PublicComputersDetectionEnabled = owaOrgConfigData.PublicComputersDetectionEnabled;
		}

		protected SmimeConfigurationContainer LoadSmimeAdminOptions(OrganizationId organizationId)
		{
			SmimeConfigurationContainer smimeConfiguration = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 542, "LoadSmimeAdminOptions", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\ServiceCommands\\GetOwaUserConfiguration.cs");
				SmimeConfigurationContainer[] array = tenantOrTopologyConfigurationSession.Find<SmimeConfigurationContainer>(SmimeConfigurationContainer.GetWellKnownParentLocation(tenantOrTopologyConfigurationSession.GetOrgContainerId()), QueryScope.SubTree, null, null, 1);
				if (array != null && array.Length > 0)
				{
					smimeConfiguration = array[0];
				}
			});
			if (!adoperationResult.Succeeded)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<OrganizationId, object>((long)this.GetHashCode(), "Find SmimeConfigurationContainer for {0} threw exception: {1}", organizationId, adoperationResult.Exception ?? "<null>");
			}
			return smimeConfiguration;
		}

		protected Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag[] GetRetentionPolicyTags(MailboxSession mailboxSession)
		{
			IEnumerable<Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag> source = from policyTag in this.GetPolicyTags(mailboxSession)
			select new Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag(policyTag);
			return source.ToArray<Microsoft.Exchange.Services.Core.Types.RetentionPolicyTag>();
		}

		protected int GetMaxRecipientsPerMessage()
		{
			if (GetOwaUserConfiguration.maxRecipientsPerMessage == null)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA"))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue("MaxRecipientsPerMessage", 2000);
						if (value is int)
						{
							GetOwaUserConfiguration.maxRecipientsPerMessage = new int?((int)value);
						}
						else
						{
							ExTraceGlobals.CoreTracer.TraceWarning((long)this.GetHashCode(), "Skipping over registry key {0}\\{1} because the key value is of type {2} but int was expected, defaulting to {3}", new object[]
							{
								"SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA",
								"MaxRecipientsPerMessage",
								value.GetType(),
								2000
							});
							GetOwaUserConfiguration.maxRecipientsPerMessage = new int?(2000);
						}
					}
					else
					{
						GetOwaUserConfiguration.maxRecipientsPerMessage = new int?(2000);
					}
				}
			}
			return GetOwaUserConfiguration.maxRecipientsPerMessage.Value;
		}

		private UMSettingsData ReadAggregatedUMSettingsData(UserConfigurationManager.IAggregationContext ctx, ExchangePrincipal exchangePrincipal)
		{
			return UserContextUtilities.ReadAggregatedType<UMSettingsData>(ctx, "OWA.UMSettings", delegate
			{
				UMSettingsData umsettingsData = new UMSettingsData();
				try
				{
					using (UMClientCommon umclientCommon = new UMClientCommon(exchangePrincipal))
					{
						umsettingsData.PlayOnPhoneDialString = umclientCommon.GetPlayOnPhoneDialString();
						umsettingsData.IsRequireProtectedPlayOnPhone = umclientCommon.IsRequireProtectedPlayOnPhone();
						umsettingsData.IsUMEnabled = umclientCommon.IsUMEnabled();
					}
				}
				catch (LocalizedException arg)
				{
					ExTraceGlobals.CoreTracer.TraceWarning<LocalizedException>((long)this.GetHashCode(), "Failed to initialize UM settings due to exception: {0}", arg);
				}
				return umsettingsData;
			});
		}

		private OwaOrgConfigData ReadAggregatedOrgConfigData(UserConfigurationManager.IAggregationContext ctx, OrganizationId organizationId)
		{
			return UserContextUtilities.ReadAggregatedType<OwaOrgConfigData>(ctx, "OWA.OrgConfig", () => UserContextUtilities.GetOrgConfigTypeFromAd(organizationId));
		}

		private SmimeSettingsData ReadAggregatedSmimeData(UserConfigurationManager.IAggregationContext ctx, OrganizationId organizationId)
		{
			return UserContextUtilities.ReadAggregatedType<SmimeSettingsData>(ctx, "OWA.SMIME", delegate
			{
				SmimeSettingsData result = null;
				ISmimeSettingsProvider smimeSettingsProvider = SmimeAdminSettingsType.ReadSmimeSettingsFromAd(organizationId);
				if (smimeSettingsProvider != null)
				{
					result = new SmimeSettingsData(SmimeAdminSettingsType.ReadSmimeSettingsFromAd(organizationId));
				}
				return result;
			});
		}

		private MobileDevicePolicyData ReadAggregatedMobileDevicePolicyData(UserConfigurationManager.IAggregationContext ctx, ExchangePrincipal principal)
		{
			return UserContextUtilities.ReadAggregatedType<MobileDevicePolicyData>(ctx, "OWA.MobileDevicePolicy", () => MobileDevicePolicyDataFactory.GetPolicyData(principal));
		}

		private OwaHelpUrlData ReadAggregatedOwaHelpUrlData(UserConfigurationManager.IAggregationContext ctx, CultureInfo cultureInfo, OwaIdentity mailboxIdentity, UserAgent userAgent)
		{
			return UserContextUtilities.ReadAggregatedType<OwaHelpUrlData>(ctx, "OWA.OwaHelpUrlData", delegate
			{
				HelpProvider.RenderingMode mode = HelpProvider.RenderingMode.Mouse;
				switch (userAgent.Layout)
				{
				case LayoutType.TouchNarrow:
					mode = HelpProvider.RenderingMode.TNarrow;
					break;
				case LayoutType.TouchWide:
					mode = HelpProvider.RenderingMode.TWide;
					break;
				case LayoutType.Mouse:
					mode = HelpProvider.RenderingMode.Mouse;
					break;
				}
				OrganizationProperties organizationProperties = null;
				if (mailboxIdentity != null)
				{
					organizationProperties = mailboxIdentity.UserOrganizationProperties;
				}
				string helpUrl = HelpProvider.ConstructHelpRenderingUrl(cultureInfo.LCID, HelpProvider.OwaHelpExperience.Premium, string.Empty, mode, null, organizationProperties).ToString();
				return new OwaHelpUrlData
				{
					HelpUrl = helpUrl
				};
			});
		}

		private PolicyTipsData ReadAggregatedPolicyTipsData(UserConfigurationManager.IAggregationContext ctx, OrganizationId organizationId)
		{
			return UserContextUtilities.ReadAggregatedType<PolicyTipsData>(ctx, "OWA.PolicyTips", () => new PolicyTipsData
			{
				IsPolicyTipsEnabled = GetOwaUserConfiguration.IsPolicyTipsEnabled(organizationId)
			});
		}

		private void ReadInferenceSettings(MailboxSession mailboxSession, UserContext userContext, OwaUserConfiguration owaUserConfiguration)
		{
			owaUserConfiguration.ApplicationSettings.InferenceEnabled &= ActivityLogHelper.IsActivityLoggingEnabled(false);
			if (userContext.FeaturesManager.ClientServerSettings.FolderBasedClutter.Enabled)
			{
				InferenceSettingsType.ReadFolderBasedClutterSettings(mailboxSession, userContext.FeaturesManager.ConfigurationSnapshot, owaUserConfiguration);
				return;
			}
			bool featureSupportedState = InferenceSettingsType.GetFeatureSupportedState(mailboxSession, userContext);
			owaUserConfiguration.SegmentationSettings.PredictedActions = featureSupportedState;
			if (featureSupportedState)
			{
				InferenceSettingsType inferenceSettingsType = new InferenceSettingsType();
				inferenceSettingsType.LoadAll(mailboxSession);
				if (inferenceSettingsType.TryMigrateUserOptionsValue(mailboxSession, owaUserConfiguration.UserOptions))
				{
					ExTraceGlobals.CoreTracer.TraceDebug((long)this.GetHashCode(), "Migrated inference flags from Owa.Settings to Inference.Settings configuration in GetOwaUserConfiguration.");
				}
				owaUserConfiguration.UserOptions.ShowInferenceUiElements = inferenceSettingsType.IsClutterUIEnabled.GetValueOrDefault(false);
			}
		}

		private UserConfigurationManager.IAggregationContext CreateAggregatedConfiguration(UserContext userContext, MailboxSession session)
		{
			UserConfigurationManager.IAggregationContext result = null;
			if (userContext != null && !userContext.IsExplicitLogon && session != null && session.UserConfigurationManager != null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug((long)this.GetHashCode(), "user configuration will create an aggregated user configuration.");
				result = session.UserConfigurationManager.AttachAggregator(AggregatedUserConfigurationSchema.Instance.OwaUserConfiguration);
			}
			return result;
		}

		private void ValidateAndDisposeAggregatedConfiguration(UserConfigurationManager.IAggregationContext aggregator, MailboxSession session)
		{
			bool flag = true;
			try
			{
				if (aggregator != null)
				{
					aggregator.Detach();
					if (!base.CallContext.HasDeviceHeaders)
					{
						ValidateAggregatedConfiguration.AddToValidationCache(session, aggregator);
						flag = false;
					}
					else
					{
						flag = true;
						ExTraceGlobals.CoreTracer.TraceDebug((long)this.GetHashCode(), "user configuration being requested by an offline action.");
						object obj = ValidateAggregatedConfiguration.RemoveFromValidationCache(session);
						bool flag2 = obj is bool && (bool)obj;
						if (flag2)
						{
							ExTraceGlobals.CoreTracer.TraceDebug((long)this.GetHashCode(), "aggregated configuration was already validated during boot.");
						}
						else
						{
							DisposeGuard.DisposeIfPresent(obj as IDisposable);
							ExTraceGlobals.CoreTracer.TraceDebug((long)this.GetHashCode(), "aggregated configuration was not validated during boot. validating now.");
							aggregator.Validate(session, delegate(IEnumerable<UserConfigurationDescriptor.MementoClass> param0, IEnumerable<string> param1)
							{
							});
						}
						ValidateAggregatedConfiguration.AddToValidationCache(session, true);
					}
				}
			}
			finally
			{
				if (flag)
				{
					DisposeGuard.DisposeIfPresent(aggregator);
				}
			}
		}

		private void RecordAggregationStats(UserConfigurationManager.IAggregationContext aggregation)
		{
			if (aggregation == null)
			{
				base.CallContext.ProtocolLog.Set(OwaUserConfigurationLogMetadata.AggregationStats, "NULL");
				return;
			}
			int num = aggregation.FaiCacheHits + aggregation.TypeCacheHits;
			int num2 = aggregation.FaiCacheMisses + aggregation.TypeCacheMisses;
			int num3 = num + num2;
			string value = string.Format("{0}:{1}::{2}:{3}", new object[]
			{
				aggregation.FaiCacheHits,
				aggregation.FaiCacheMisses,
				aggregation.TypeCacheHits,
				aggregation.TypeCacheMisses
			});
			base.CallContext.ProtocolLog.Set(OwaUserConfigurationLogMetadata.AggregationStats, value);
			if (num > 0)
			{
				OwaSingleCounters.AggregatedUserConfigurationPartsRead.IncrementBy((long)num);
			}
			if (num3 > 0)
			{
				OwaSingleCounters.AggregatedUserConfigurationPartsRequested.IncrementBy((long)num3);
			}
		}

		private const string RegistryKeyPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA";

		private const string MaxRecipientsPerMessageKeyName = "MaxRecipientsPerMessage";

		private const string GetOwaUserConfigurationActionName = "GetOwaUserConfiguration";

		private const int DefaultMaxRecipientsPerMessage = 2000;

		private static readonly Random random = new Random();

		private readonly PlacesConfigurationCache placesConfigurationCache;

		private readonly IWeatherConfigurationCache weatherConfigurationCache;

		private readonly Func<bool> isWindowsLiveIdAuth;

		private static int? maxRecipientsPerMessage;
	}
}
