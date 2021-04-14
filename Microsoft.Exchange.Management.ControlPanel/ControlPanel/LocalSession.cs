using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal abstract class LocalSession : RbacSession
	{
		protected LocalSession(RbacContext context, SessionPerformanceCounters sessionPerfCounters, EsoSessionPerformanceCounters esoSessionPerfCounters) : base(context, sessionPerfCounters, esoSessionPerfCounters)
		{
			this.IsCrossSiteMailboxLogon = context.IsCrossSiteMailboxLogon;
			foreach (string role in LocalSession.roleList)
			{
				this.logonTypeFlag <<= 1;
				this.logonTypeFlag |= (base.IsInRole(role) ? 1 : 0);
			}
		}

		public new static LocalSession Current
		{
			get
			{
				return (LocalSession)RbacPrincipal.Current;
			}
		}

		public string LogonTypeFlag
		{
			get
			{
				return this.logonTypeFlag.ToString();
			}
		}

		public override string DateFormat
		{
			get
			{
				this.WaitingForCmdlet();
				return base.DateFormat;
			}
			protected set
			{
				base.DateFormat = value;
			}
		}

		public override string TimeFormat
		{
			get
			{
				this.WaitingForCmdlet();
				return base.TimeFormat;
			}
			protected set
			{
				base.TimeFormat = value;
			}
		}

		public override ExTimeZone UserTimeZone
		{
			get
			{
				this.WaitingForCmdlet();
				return base.UserTimeZone;
			}
			protected set
			{
				base.UserTimeZone = value;
			}
		}

		public SmtpAddress ExecutingUserPrimarySmtpAddress
		{
			get
			{
				if (this.executingUserPrimarySmtpAddress == null)
				{
					if (base.RbacConfiguration.DelegatedPrincipal != null)
					{
						if (SmtpAddress.IsValidSmtpAddress(base.RbacConfiguration.DelegatedPrincipal.UserId))
						{
							this.executingUserPrimarySmtpAddress = new SmtpAddress?(new SmtpAddress(base.RbacConfiguration.DelegatedPrincipal.UserId));
						}
					}
					else
					{
						this.executingUserPrimarySmtpAddress = new SmtpAddress?(base.RbacConfiguration.ExecutingUserPrimarySmtpAddress);
					}
					if (this.executingUserPrimarySmtpAddress == null)
					{
						this.executingUserPrimarySmtpAddress = new SmtpAddress?(SmtpAddress.Empty);
					}
				}
				return this.executingUserPrimarySmtpAddress.Value;
			}
		}

		public bool IsCrossSiteMailboxLogon { get; private set; }

		public bool IsDehydrated
		{
			get
			{
				if (this.isDehydrated == null)
				{
					if (base.RbacConfiguration.OrganizationId != OrganizationId.ForestWideOrgId)
					{
						IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(base.RbacConfiguration.OrganizationId), 203, "IsDehydrated", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\RBAC\\LocalSession.cs");
						tenantOrTopologyConfigurationSession.UseGlobalCatalog = true;
						tenantOrTopologyConfigurationSession.UseConfigNC = true;
						ADRawEntry adrawEntry = tenantOrTopologyConfigurationSession.ReadADRawEntry(base.RbacConfiguration.OrganizationId.ConfigurationUnit, new PropertyDefinition[]
						{
							OrganizationSchema.IsDehydrated
						});
						this.isDehydrated = new bool?((bool)adrawEntry[OrganizationSchema.IsDehydrated]);
					}
					else
					{
						this.isDehydrated = new bool?(false);
					}
				}
				return this.isDehydrated.Value;
			}
		}

		public bool IsSoftDeletedFeatureEnabled
		{
			get
			{
				if (this.isSoftDeletedFeatureEnabled == null)
				{
					if (base.RbacConfiguration.OrganizationId != OrganizationId.ForestWideOrgId)
					{
						ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsPartitionId(base.RbacConfiguration.OrganizationId.PartitionId), 241, "IsSoftDeletedFeatureEnabled", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\RBAC\\LocalSession.cs");
						ExchangeConfigurationUnit exchangeConfigurationUnit = tenantConfigurationSession.Read<ExchangeConfigurationUnit>(base.RbacConfiguration.OrganizationId.ConfigurationUnit);
						bool msosyncEnabled = exchangeConfigurationUnit.MSOSyncEnabled;
						Organization rootOrgContainer = ADSystemConfigurationSession.GetRootOrgContainer(TopologyProvider.LocalForestFqdn, null, null);
						SoftDeletedFeatureStatusFlags softDeletedFeatureStatus = rootOrgContainer.SoftDeletedFeatureStatus;
						if (!this.IsFeatureEnabled(softDeletedFeatureStatus, msosyncEnabled))
						{
							softDeletedFeatureStatus = exchangeConfigurationUnit.SoftDeletedFeatureStatus;
							this.isSoftDeletedFeatureEnabled = new bool?(this.IsFeatureEnabled(softDeletedFeatureStatus, msosyncEnabled));
						}
						else
						{
							this.isSoftDeletedFeatureEnabled = new bool?(true);
						}
					}
					else
					{
						this.isSoftDeletedFeatureEnabled = new bool?(false);
					}
				}
				return this.isSoftDeletedFeatureEnabled.Value;
			}
		}

		private bool IsFeatureEnabled(SoftDeletedFeatureStatusFlags feature, bool isMSOTenant)
		{
			return (!isMSOTenant && (feature & SoftDeletedFeatureStatusFlags.EDUEnabled) == SoftDeletedFeatureStatusFlags.EDUEnabled) || (isMSOTenant && (feature & SoftDeletedFeatureStatusFlags.MSOEnabled) == SoftDeletedFeatureStatusFlags.MSOEnabled);
		}

		private bool HasRegionalSettings { get; set; }

		internal void AddRegionalSettingsToCache(RegionalSettingsConfiguration settings)
		{
			HttpRuntime.Cache.Insert(this.RegionalCacheKey, settings, null, (DateTime)ExDateTime.UtcNow.Add(TimeSpan.FromHours(24.0)), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
		}

		private string RegionalCacheKey
		{
			get
			{
				return base.Settings.CacheKey + "_Regional";
			}
		}

		private void InitializeRegionalSettings()
		{
			RegionalSettingsConfiguration regionalSettingsConfiguration = null;
			this.HasRegionalSettings = false;
			RegionalSettingsConfiguration regionalSettingsConfiguration2 = new RegionalSettingsConfiguration(new MailboxRegionalConfiguration());
			if (base.IsInRole("Get-MailboxRegionalConfiguration?Identity@R:Self") && base.IsInRole("!DelegatedAdmin"))
			{
				if (HttpRuntime.Cache[this.RegionalCacheKey] != null)
				{
					regionalSettingsConfiguration2 = (RegionalSettingsConfiguration)HttpRuntime.Cache[this.RegionalCacheKey];
					this.regionalCmdletStatus = LocalSession.RegionalCmdletStatus.Finished;
				}
				else
				{
					this.regionalCmdletStatus = LocalSession.RegionalCmdletStatus.Waiting;
					regionalSettingsConfiguration2 = new RegionalSettingsConfiguration(new MailboxRegionalConfiguration
					{
						Language = base.UserCulture
					});
				}
				if (base.IsInRole("MailboxFullAccess"))
				{
					regionalSettingsConfiguration = regionalSettingsConfiguration2;
					this.HasRegionalSettings = (regionalSettingsConfiguration != null && regionalSettingsConfiguration.UserCulture != null);
					if (this.regionalCmdletStatus == LocalSession.RegionalCmdletStatus.Finished)
					{
						this.HasRegionalSettings = (this.HasRegionalSettings && regionalSettingsConfiguration.TimeZone != null);
					}
				}
			}
			else
			{
				this.regionalCmdletStatus = LocalSession.RegionalCmdletStatus.NotNeeded;
			}
			bool flag = !this.HasRegionalSettings;
			HttpContext httpContext = HttpContext.Current;
			CultureInfo cultureInfo = (regionalSettingsConfiguration == null) ? null : regionalSettingsConfiguration.UserCulture;
			if (regionalSettingsConfiguration == null || (!this.CanRedirectToOwa(httpContext) && !this.HasRegionalSettings) || (cultureInfo == null && httpContext.TargetServerOrVersionSpecifiedInUrlOrCookie()))
			{
				if (regionalSettingsConfiguration != null && cultureInfo == null && regionalSettingsConfiguration.MailboxRegionalConfiguration.Language != null)
				{
					EcpEventLogConstants.Tuple_LanguagePackIsNotInstalled.LogEvent(new object[]
					{
						EcpEventLogExtensions.GetUserNameToLog(),
						regionalSettingsConfiguration.MailboxRegionalConfiguration.Language.IetfLanguageTag
					});
				}
				regionalSettingsConfiguration = this.GetDefaultRegionalSettings(regionalSettingsConfiguration2);
				this.HasRegionalSettings = true;
			}
			string text = httpContext.Request.QueryString["mkt"];
			if (flag && string.IsNullOrEmpty(text))
			{
				text = httpContext.Request.QueryString["mkt2"];
			}
			if (!string.IsNullOrEmpty(text))
			{
				httpContext.Response.Cookies.Add(new HttpCookie("mkt", text));
			}
			else
			{
				HttpCookie httpCookie = httpContext.Request.Cookies["mkt"];
				if (httpCookie != null)
				{
					text = httpCookie.Value;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					CultureInfo cultureInfoByIetfLanguageTag = CultureInfo.GetCultureInfoByIetfLanguageTag(text);
					if (Culture.IsSupportedCulture(cultureInfoByIetfLanguageTag))
					{
						MailboxRegionalConfiguration mailboxRegionalConfiguration = new MailboxRegionalConfiguration();
						mailboxRegionalConfiguration.Language = Culture.GetCultureInfoInstance(cultureInfoByIetfLanguageTag.LCID);
						mailboxRegionalConfiguration.TimeZone = regionalSettingsConfiguration.MailboxRegionalConfiguration.TimeZone;
						if (regionalSettingsConfiguration.UserCulture != null && regionalSettingsConfiguration.UserCulture.DateTimeFormat != null)
						{
							mailboxRegionalConfiguration.DateFormat = regionalSettingsConfiguration.DateFormat;
							mailboxRegionalConfiguration.TimeFormat = regionalSettingsConfiguration.TimeFormat;
						}
						regionalSettingsConfiguration = new RegionalSettingsConfiguration(mailboxRegionalConfiguration);
						this.HasRegionalSettings = true;
					}
				}
				catch (ArgumentException)
				{
				}
			}
			if (this.HasRegionalSettings)
			{
				this.UpdateRegionalSettings(regionalSettingsConfiguration);
			}
		}

		private bool CanRedirectToOwa(HttpContext context)
		{
			return base.IsInRole("Mailbox+OWA") && (!context.IsAcsOAuthRequest() || !context.IsWebServiceRequest());
		}

		private RegionalSettingsConfiguration GetDefaultRegionalSettings(RegionalSettingsConfiguration executingUserSettings)
		{
			return new RegionalSettingsConfiguration(new MailboxRegionalConfiguration
			{
				Language = (executingUserSettings.UserCulture ?? Culture.GetDefaultCulture(HttpContext.Current)),
				TimeZone = executingUserSettings.MailboxRegionalConfiguration.TimeZone
			});
		}

		private void WaitingForCmdlet()
		{
			if (this.regionalCmdletStatus == LocalSession.RegionalCmdletStatus.Waiting)
			{
				this.regionalCmdletStatus = LocalSession.RegionalCmdletStatus.Running;
				RegionalSettingsConfiguration settings = new RegionalSettingsConfiguration(new MailboxRegionalConfiguration());
				RegionalSettings regionalSettings = new RegionalSettings();
				PowerShellResults<RegionalSettingsConfiguration> settings2 = regionalSettings.GetSettings(Microsoft.Exchange.Management.ControlPanel.Identity.FromExecutingUserId(), false);
				if (settings2.SucceededWithValue)
				{
					settings = settings2.Value;
					this.AddRegionalSettingsToCache(settings);
					this.InitializeRegionalSettings();
				}
			}
		}

		public void UpdateRegionalSettings(RegionalSettingsConfiguration settings)
		{
			this.DateFormat = settings.DateFormat;
			this.TimeFormat = settings.TimeFormat;
			base.UserCulture = RoleBasedStringMapping.GetRoleBasedCultureInfo(settings.UserCulture, base.RbacConfiguration.RoleTypes);
			this.UpdateUserTimeZone(settings.TimeZone);
		}

		internal void UpdateUserTimeZone(string userTimeZone)
		{
			ExTimeZone userTimeZone2 = null;
			if (!string.IsNullOrEmpty(userTimeZone))
			{
				ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(userTimeZone, out userTimeZone2);
			}
			this.UserTimeZone = userTimeZone2;
		}

		public override void RequestReceived()
		{
			this.ThrowIfUserIsMailboxButNoMyBaseOptions();
			this.RedirectIfUserEsoSelf();
			this.RedirectIfTargetTenantSpecifiedForRegularAdmin();
			bool flag = !this.HasRegionalSettings;
			HttpContext httpContext = HttpContext.Current;
			DiagnosticsBehavior.CheckSystemProbeCookie(httpContext);
			string text = HttpContext.Current.Request.QueryString["mkt"];
			if (string.IsNullOrEmpty(text))
			{
				text = HttpContext.Current.Request.QueryString["mkt2"];
			}
			if (this.HasRegionalSettings && !string.IsNullOrEmpty(text))
			{
				try
				{
					CultureInfo cultureInfoByIetfLanguageTag = CultureInfo.GetCultureInfoByIetfLanguageTag(text);
					if (Culture.IsSupportedCulture(cultureInfoByIetfLanguageTag) && base.UserCulture.LCID != RoleBasedStringMapping.GetRoleBasedCultureInfo(cultureInfoByIetfLanguageTag, base.RbacConfiguration.RoleTypes).LCID)
					{
						flag = true;
					}
				}
				catch (ArgumentException)
				{
				}
			}
			if (flag)
			{
				this.InitializeRegionalSettings();
				if (!this.HasRegionalSettings)
				{
					if (!(httpContext.Request.HttpMethod == "GET"))
					{
						throw new RegionalSettingsNotConfiguredException(base.ExecutingUserId);
					}
					base.RbacConfiguration.ExecutingUserLanguagesChanged = true;
					string url = string.Format(EcpUrl.OwaVDir + "languageselection.aspx?url={0}", HttpUtility.UrlEncode(EcpUrl.ProcessUrl(httpContext.GetRequestUrlPathAndQuery())));
					httpContext.Response.Redirect(url, true);
				}
			}
			base.RequestReceived();
		}

		private void ThrowIfUserIsMailboxButNoMyBaseOptions()
		{
			if (!base.IsInRole("ControlPanelAdmin") && base.IsInRole("Mailbox") && !base.IsInRole(RoleType.MyBaseOptions.ToString()))
			{
				ExTraceGlobals.RBACTracer.TraceInformation<string>(0, 0L, "{0} is not allowed to logon to ECP as the user has mailbox but doesn't have MyBaseOptions role.", base.Name);
				throw new CmdletAccessDeniedException(Strings.UserMissingMyBaseOptionsRole(base.NameForEventLog));
			}
		}

		private void RedirectIfUserEsoSelf()
		{
			if (base.Settings.LogonUserEsoSelf)
			{
				HttpContext httpContext = HttpContext.Current;
				httpContext.Response.Redirect(httpContext.GetRequestUrlPathAndQuery(), true);
				throw new InvalidOperationException();
			}
		}

		private void RedirectIfTargetTenantSpecifiedForRegularAdmin()
		{
			HttpContext httpContext = HttpContext.Current;
			if (httpContext.HasTargetTenant() && (base.IsInRole("LogonUserMailbox") || base.IsInRole("LogonMailUser")))
			{
				string url = EcpUrl.ProcessUrl(httpContext.GetRequestUrlPathAndQuery(), true, EcpUrl.EcpVDirForStaticResource, true, false);
				httpContext.Response.Redirect(url, true);
				throw new InvalidOperationException();
			}
		}

		public virtual void FlushCache()
		{
			if (this.isDehydrated != null)
			{
				this.isDehydrated = null;
				this.IsDehydrated.ToString();
			}
		}

		public int WeekStartDay
		{
			get
			{
				if (this.weekStartDay == -1)
				{
					if (!base.IsInRole("Get-MailboxCalendarConfiguration?Identity@R:Self"))
					{
						return 0;
					}
					CalendarAppearance calendarAppearance = new CalendarAppearance();
					PowerShellResults<CalendarAppearanceConfiguration> @object = calendarAppearance.GetObject(Microsoft.Exchange.Management.ControlPanel.Identity.FromExecutingUserId());
					this.weekStartDay = (@object.SucceededWithValue ? @object.Output[0].WeekStartDay : 0);
				}
				return this.weekStartDay;
			}
			set
			{
				this.weekStartDay = value;
			}
		}

		internal const string RegionalKeySuffix = "_Regional";

		private readonly int logonTypeFlag;

		private static List<string> roleList = new List<string>
		{
			"Impersonated",
			"Mailbox",
			"MailboxFullAccess",
			"ByoidAdmin",
			"DelegatedAdmin",
			"Admin"
		};

		private LocalSession.RegionalCmdletStatus regionalCmdletStatus;

		private SmtpAddress? executingUserPrimarySmtpAddress;

		private bool? isDehydrated;

		private bool? isSoftDeletedFeatureEnabled;

		private int weekStartDay = -1;

		private enum RegionalCmdletStatus
		{
			Initial,
			Waiting,
			Finished,
			NotNeeded,
			Running
		}
	}
}
