using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.TenantMonitoring;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class RbacContext
	{
		static RbacContext()
		{
			RbacContext.EsoAllowedCmdlets = new List<RoleEntry>
			{
				new CmdletRoleEntry("Clear-ActiveSyncDevice", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-ActiveSyncDevice", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Remove-ActiveSyncDevice", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-ActiveSyncDeviceClass", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-ActiveSyncDeviceStatistics", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-ActiveSyncMailboxPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("New-ActiveSyncMailboxPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Remove-ActiveSyncMailboxPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-ActiveSyncMailboxPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-ActiveSyncOrganizationSettings", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-ActiveSyncOrganizationSettings", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-CalendarDiagnosticLog", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-CalendarNotification", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-CalendarNotification", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-CASMailbox", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-CASMailbox", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-ActiveSyncDeviceAccessRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("New-ActiveSyncDeviceAccessRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Remove-ActiveSyncDeviceAccessRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-ActiveSyncDeviceAccessRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Disable-InboxRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Enable-InboxRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-InboxRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("New-InboxRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Remove-InboxRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-InboxRule", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-RetentionPolicyTag", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-RetentionPolicyTag", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Clear-MobileDevice", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MobileDevice", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Remove-MobileDevice", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MobileDeviceStatistics", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MobileDeviceMailboxPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("New-MobileDeviceMailboxPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-MobileDeviceMailboxPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Remove-MobileDeviceMailboxPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MailboxAutoReplyConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-MailboxAutoReplyConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MailboxCalendarConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-MailboxCalendarConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-CalendarProcessing", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-CalendarProcessing", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MailboxFolder", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("New-MailboxFolder", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MailboxCalendarFolder", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-MailboxCalendarFolder", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MailboxFolderPermission", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-MailboxFolderPermission", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Remove-MailboxFolderPermission", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MailboxJunkEmailConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-MailboxJunkEmailConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MailboxMessageConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-MailboxMessageConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MailboxRegionalConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-MailboxRegionalConfiguration", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("New-RoleAssignmentPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-RoleAssignmentPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Remove-RoleAssignmentPolicy", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("New-MailMessage", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MessageClassification", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MessageCategory", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-MessageTrackingReport", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Search-MessageTrackingReport", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-Recipient", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-SiteMailbox", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Clear-TextMessagingAccount", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Get-TextMessagingAccount", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Set-TextMessagingAccount", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Compare-TextMessagingVerificationCode", "Microsoft.Exchange.Management.PowerShell.E2010", null),
				new CmdletRoleEntry("Send-TextMessagingVerificationCode", "Microsoft.Exchange.Management.PowerShell.E2010", null)
			};
			RbacContext.EsoAllowedCmdlets.Sort(RoleEntry.NameComparer);
			string value = ConfigurationManager.AppSettings["ExchangeRunspaceConfigurationSnapinSet"];
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					RbacContext.snapinSet = (SnapinSet)Enum.Parse(typeof(SnapinSet), value);
					goto IL_6CD;
				}
				catch
				{
					RbacContext.snapinSet = SnapinSet.Admin;
					goto IL_6CD;
				}
			}
			RbacContext.snapinSet = SnapinSet.Admin;
			IL_6CD:
			string value2 = ConfigurationManager.AppSettings["PullHostedTenantRbac"];
			if (!bool.TryParse(value2, out RbacContext.PullHostedTenantRbac))
			{
				RbacContext.PullHostedTenantRbac = false;
			}
			RbacContext.clientAppId = RbacContext.GetClientApplication();
		}

		public static ExchangeRunspaceConfigurationSettings.ExchangeApplication ClientAppId
		{
			get
			{
				return RbacContext.clientAppId;
			}
		}

		public RbacContext(RbacSettings settings)
		{
			RbacContext <>4__this = this;
			ExTraceGlobals.RBACTracer.TraceInformation<string>(0, 0L, "Creating RBAC context for {0}", settings.UserName);
			this.Settings = settings;
			this.roles = new LazilyInitialized<ExchangeRunspaceConfiguration>(delegate()
			{
				ExchangeRunspaceConfiguration exchangeRunspaceConfiguration;
				if (DatacenterRegistry.IsForefrontForOffice())
				{
					Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.Security.Authorization");
					string siteName = HostingEnvironment.ApplicationHost.GetSiteName();
					try
					{
						string name = (RbacContext.PullHostedTenantRbac && (bool)HttpContext.Current.Items["IsHostedTenant"]) ? "Microsoft.Exchange.Hygiene.Security.Authorization.ForefrontRunspaceConfigurationForHostedTenant" : "Microsoft.Exchange.Hygiene.Security.Authorization.ForefrontRunspaceConfiguration";
						Type type = assembly.GetType(name);
						exchangeRunspaceConfiguration = (ExchangeRunspaceConfiguration)Activator.CreateInstance(type, new object[]
						{
							<>4__this.Settings.OriginalLogonUserIdentity,
							siteName
						});
						goto IL_222;
					}
					catch (TargetInvocationException ex)
					{
						throw ex.InnerException ?? ex;
					}
				}
				IList<RoleType> list = <>4__this.GetAllowedRoleTypes(settings);
				ExchangeRunspaceConfigurationSettings.ExchangeUserType user = ExchangeRunspaceConfigurationSettings.ExchangeUserType.Unknown;
				string text = (HttpContext.Current.Request == null) ? null : HttpContext.Current.Request.UserAgent;
				IEnumerable<KeyValuePair<string, string>> customizedConstraints = <>4__this.GetCustomizedConstraints(settings);
				if (!string.IsNullOrEmpty(text) && (text == "User-Agent:+Mozilla/4.0+(compatible;+MSIE+7.0)" || text == "Mozilla/4.0+(compatible;+MSIE+9.0;+Windows+NT+6.1;+MSEXCHMON;+TESTCONN2)" || text.IndexOf("ACTIVEMONITORING", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("MONITORINGWEBCLIENT", StringComparison.OrdinalIgnoreCase) >= 0))
				{
					user = ExchangeRunspaceConfigurationSettings.ExchangeUserType.Monitoring;
				}
				ExchangeRunspaceConfigurationSettings settings2 = new ExchangeRunspaceConfigurationSettings(null, RbacContext.ClientAppId, null, ExchangeRunspaceConfigurationSettings.SerializationLevel.None, PSLanguageMode.NoLanguage, ExchangeRunspaceConfigurationSettings.ProxyMethod.RPS, false, false, false, user, customizedConstraints);
				if (<>4__this.Settings.IsExplicitSignOn)
				{
					if (<>4__this.Settings.HasFullAccess)
					{
						IList<RoleType> list2;
						if (list == null)
						{
							IList<RoleType> esoAllowedRoleTypes = RbacContext.EsoAllowedRoleTypes;
							list2 = esoAllowedRoleTypes;
						}
						else
						{
							list2 = list.Intersect(RbacContext.EsoAllowedRoleTypes).ToList<RoleType>();
						}
						list = list2;
						exchangeRunspaceConfiguration = new ExchangeRunspaceConfiguration(<>4__this.Settings.OriginalLogonUserIdentity, <>4__this.Settings.AccessedUserIdentity, settings2, list, RbacContext.EsoAllowedCmdlets, null, true, RbacContext.snapinSet);
					}
					else
					{
						exchangeRunspaceConfiguration = new ExchangeRunspaceConfiguration(<>4__this.Settings.OriginalLogonUserIdentity, <>4__this.Settings.AccessedUserIdentity, settings2, list, null, RbacContext.EsoRequiredRoleTypesForAdmin, false, RbacContext.snapinSet);
					}
				}
				else
				{
					exchangeRunspaceConfiguration = new ExchangeRunspaceConfiguration(<>4__this.Settings.AccessedUserIdentity, null, settings2, list, null, null, false, RbacContext.snapinSet);
				}
				IL_222:
				if (!exchangeRunspaceConfiguration.ExecutingUserIsAllowedECP)
				{
					ExTraceGlobals.RBACTracer.TraceInformation<string>(0, 0L, "{0} is not allowed to use ECP.", <>4__this.Settings.UserName);
					throw new CmdletAccessDeniedException(Strings.UserIsNotEcpEnabled(<>4__this.Settings.UserName));
				}
				return exchangeRunspaceConfiguration;
			});
			this.mailbox = new LazilyInitialized<ExchangePrincipal>(() => this.Settings.GetAccessedUserExchangePrincipal());
		}

		public RbacSettings Settings { get; private set; }

		public bool IsCrossSiteMailboxLogon
		{
			get
			{
				if (this.isCrossSiteMailboxLogon == null)
				{
					this.isCrossSiteMailboxLogon = new bool?(this.IsMailbox && HttpContext.Current.TargetServerOrVersionSpecifiedInUrlOrCookie());
				}
				return this.isCrossSiteMailboxLogon.Value;
			}
		}

		public bool IsCrossSiteMailboxLogonAllowed { get; internal set; }

		public bool IsMailbox
		{
			get
			{
				return this.mailbox.Value != null;
			}
		}

		public ServerVersion MailboxServerVersion
		{
			get
			{
				return new ServerVersion(this.Mailbox.MailboxInfo.Location.ServerVersion);
			}
		}

		public ExchangeRunspaceConfiguration Roles
		{
			get
			{
				return this.roles;
			}
		}

		private ExchangePrincipal Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		private IEnumerable<RbacSession.Factory> RbacSessionFactories
		{
			get
			{
				yield return new InboundProxySession.ProxyLogonNeededFactory(this);
				yield return new InboundProxySession.Factory(this);
				this.CheckMailboxVersion();
				if (OutboundProxySession.Factory.ProxyToLocalHost)
				{
					yield return new OutboundProxySession.Factory(this);
				}
				bool isFromCafe = HttpContext.Current.Request.Headers["X-IsFromCafe"] == "1";
				if (isFromCafe)
				{
					this.IsCrossSiteMailboxLogonAllowed = true;
					ExTraceGlobals.RedirectTracer.TraceInformation<bool>(0, 0L, "Redirection will be skipped because the request comes from CAFE (IsFromCafe={0}).", isFromCafe);
				}
				else
				{
					ExTraceGlobals.RedirectTracer.TraceInformation<bool>(0, 0L, "Redirection will be tried because of (IsFromCafe={0}).", isFromCafe);
					if (!this.IsCrossSiteMailboxLogonAllowed && this.IsCrossSiteMailboxLogon)
					{
						yield return new OutboundProxySession.Factory(this);
					}
				}
				yield return new StandardSession.Factory(this);
				yield break;
			}
		}

		public IList<EcpService> GetServicesInMailboxSite(ClientAccessType clientAccessType, Predicate<EcpService> serviceFilter)
		{
			return ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\RBAC\\RbacContext.cs", "GetServicesInMailboxSite", 568).FindAll<EcpService>(this.Mailbox, clientAccessType, serviceFilter, "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\RBAC\\RbacContext.cs", "GetServicesInMailboxSite", 568);
		}

		public RbacSession CreateSession()
		{
			ExTraceGlobals.RBACTracer.TraceInformation<string>(0, 0L, "Creating RBAC session for {0}", this.Settings.UserName);
			TenantMonitor.LogActivity(CounterType.ECPSessionCreationAttempts, this.Settings.TenantNameForMonitoringPurpose);
			foreach (RbacSession.Factory factory in this.RbacSessionFactories)
			{
				RbacSession rbacSession = factory.CreateSession();
				if (rbacSession != null)
				{
					TenantMonitor.LogActivity(CounterType.ECPSessionCreationSuccesses, this.Settings.TenantNameForMonitoringPurpose);
					return rbacSession;
				}
			}
			throw new InvalidOperationException();
		}

		[Conditional("DEBUG")]
		private static void BackupAllowedList()
		{
			RbacContext.esoAllowedCmdletsCopy = new List<RoleEntry>(RbacContext.EsoAllowedCmdlets.Count);
			foreach (RoleEntry roleEntry in RbacContext.EsoAllowedCmdlets)
			{
				CmdletRoleEntry cmdletRoleEntry = (CmdletRoleEntry)roleEntry;
				RbacContext.esoAllowedCmdletsCopy.Add(new CmdletRoleEntry(cmdletRoleEntry.Name, cmdletRoleEntry.PSSnapinName, null));
			}
		}

		[Conditional("DEBUG")]
		private static void ThrowIfAllowedListChanged()
		{
			for (int i = 0; i < RbacContext.esoAllowedCmdletsCopy.Count; i++)
			{
				RoleEntry.CompareRoleEntriesByName(RbacContext.esoAllowedCmdletsCopy[i], RbacContext.EsoAllowedCmdlets[i]);
			}
		}

		private static ExchangeRunspaceConfigurationSettings.ExchangeApplication GetClientApplication()
		{
			try
			{
				string text = ConfigurationManager.AppSettings["IsOSPEnvironment"];
				if (text != null && text.ToUpperInvariant().Equals("TRUE"))
				{
					RbacContext.clientAppId = ExchangeRunspaceConfigurationSettings.ExchangeApplication.OSP;
				}
				else
				{
					RbacContext.clientAppId = ExchangeRunspaceConfigurationSettings.ExchangeApplication.ECP;
				}
			}
			catch (ConfigurationErrorsException ex)
			{
				ExTraceGlobals.RBACTracer.TraceWarning<string>(0, 0L, "Cannot read AppSettings: {0}", ex.Message);
				RbacContext.clientAppId = ExchangeRunspaceConfigurationSettings.ExchangeApplication.ECP;
			}
			return RbacContext.clientAppId;
		}

		private void CheckMailboxVersion()
		{
			ExchangePrincipal accessedUserExchangePrincipal = this.Settings.GetAccessedUserExchangePrincipal();
			if (accessedUserExchangePrincipal != null)
			{
				this.CheckVersion(accessedUserExchangePrincipal.MailboxInfo.Location.ServerVersion);
			}
			if (this.Settings.IsExplicitSignOn && !this.Settings.HasFullAccess)
			{
				ExchangePrincipal logonUserExchangePrincipal = this.Settings.GetLogonUserExchangePrincipal();
				if (logonUserExchangePrincipal != null)
				{
					this.CheckVersion(logonUserExchangePrincipal.MailboxInfo.Location.ServerVersion);
				}
			}
		}

		private void CheckVersion(int versionNumber)
		{
			ServerVersion serverVersion = new ServerVersion(versionNumber);
			if (serverVersion.Major < 14 && !HttpContext.Current.TargetServerOrVersionSpecifiedInUrlOrCookie())
			{
				throw new LowVersionUserDeniedException();
			}
		}

		private IList<RoleType> GetAllowedRoleTypes(RbacSettings settings)
		{
			IList<RoleType> result = null;
			if (!Util.IsDataCenter)
			{
				RoleTypeSegment roleTypeSegment = new RoleTypeSegment(settings);
				result = roleTypeSegment.GetAllowedFeatures();
			}
			return result;
		}

		private IEnumerable<KeyValuePair<string, string>> GetCustomizedConstraints(RbacSettings settings)
		{
			KeyValuePair<string, string>? oauthUserConstraint = OAuthHelper.GetOAuthUserConstraint(settings.LogonUserIdentity);
			if (oauthUserConstraint != null)
			{
				return new KeyValuePair<string, string>[]
				{
					oauthUserConstraint.Value
				};
			}
			return null;
		}

		private const int EcpMinimumSupportedVersionMajor = 14;

		private const string EnvironmentKey = "IsOSPEnvironment";

		private const string IsHostedTenantKey = "IsHostedTenant";

		private const string PullHostedTenantRbacKey = "PullHostedTenantRbac";

		internal static readonly ReadOnlyCollection<RoleType> EsoAllowedRoleTypes = new List<RoleType>
		{
			RoleType.MyBaseOptions,
			RoleType.MyMailSubscriptions,
			RoleType.MyProfileInformation,
			RoleType.MyContactInformation,
			RoleType.MyRetentionPolicies,
			RoleType.MyTextMessaging,
			RoleType.MyVoiceMail
		}.AsReadOnly();

		internal static readonly List<RoleEntry> EsoAllowedCmdlets;

		internal static readonly ReadOnlyCollection<RoleType> EsoRequiredRoleTypesForAdmin = new List<RoleType>
		{
			RoleType.UserOptions
		}.AsReadOnly();

		private static readonly bool PullHostedTenantRbac;

		private static List<RoleEntry> esoAllowedCmdletsCopy;

		private static SnapinSet snapinSet;

		private static ExchangeRunspaceConfigurationSettings.ExchangeApplication clientAppId = ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown;

		private LazilyInitialized<ExchangePrincipal> mailbox;

		private LazilyInitialized<ExchangeRunspaceConfiguration> roles;

		private bool? isCrossSiteMailboxLogon;
	}
}
