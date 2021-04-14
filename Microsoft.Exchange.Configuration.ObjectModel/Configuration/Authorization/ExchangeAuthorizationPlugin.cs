using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.FailFast;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Configuration.Authorization
{
	public class ExchangeAuthorizationPlugin : PSSessionConfiguration
	{
		static ExchangeAuthorizationPlugin()
		{
			FailFastUserCache.IsPrimaryUserCache = false;
			ExchangeAuthorizationPlugin.InitWatsonForRemotePowershell();
			if (ExchangeSetupContext.IsUnpacked)
			{
				ExchangeAuthorizationPlugin.fileSearchAssemblyResolver.Recursive = false;
				ExchangeAuthorizationPlugin.fileSearchAssemblyResolver.SearchPaths = new string[]
				{
					ExchangeSetupContext.BinPath,
					Path.Combine(ExchangeSetupContext.BinPath, "FIP-FS\\Bin"),
					Path.Combine(ExchangeSetupContext.BinPath, "CmdletExtensionAgents"),
					Path.Combine(ExchangeSetupContext.BinPath, "res")
				};
				ExchangeAuthorizationPlugin.fileSearchAssemblyResolver.ErrorTracer = delegate(string error)
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceError(0L, error.ToString());
				};
				ExchangeAuthorizationPlugin.fileSearchAssemblyResolver.Install();
			}
		}

		internal static bool ShouldShowFismaBanner
		{
			get
			{
				return Constants.IsRemotePS && VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ShowFismaBanner.Enabled && AppSettings.Current.ShouldShowFismaBanner;
			}
		}

		internal static RemotePowershellPerformanceCountersInstance RemotePowershellPerfCounter
		{
			get
			{
				return ExchangeAuthorizationPlugin.perfCounter;
			}
		}

		private void CheckSessionOverBudget()
		{
			OverBudgetException exception;
			if (WSManBudgetManager.Instance.CheckOverBudget(this.currentAuthZUserToken, CostType.ActiveRunspace, out exception))
			{
				LocalizedString message = AuthZPluginHelper.HandleUserOverBudgetException(exception, this.currentAuthZUserToken);
				throw new AuthorizationException(message);
			}
			if (this.currentAuthZUserToken.OrgId != null && WSManTenantBudgetManager.Instance.CheckOverBudget(this.currentAuthZUserToken, CostType.ActiveRunspace, out exception))
			{
				LocalizedString message2 = AuthZPluginHelper.HandleTenantOverBudgetException(exception, this.currentAuthZUserToken);
				throw new AuthorizationException(message2);
			}
			if (AppSettings.Current.MaxPowershellAppPoolConnections > 0 && WSManBudgetManager.Instance.TotalActiveRunspaces >= AppSettings.Current.MaxPowershellAppPoolConnections)
			{
				string windowsLiveId = this.currentAuthZUserToken.WindowsLiveId;
				if (!string.IsNullOrEmpty(windowsLiveId))
				{
					FailFastUserCache.Instance.AddUserToCache(windowsLiveId, BlockedType.NewSession, TimeSpan.Zero);
				}
				AuthZLogger.SafeAppendColumn(RpsCommonMetadata.ContributeToFailFast, "AuthZ-Machine", LoggerHelper.GetContributeToFailFastValue("AllUsers", "-1", "NewSesion", -1.0));
				FailFastUserCache.Instance.AddAllUsersToCache(BlockedType.NewSession, TimeSpan.Zero);
				LocalizedString localizedString = Strings.ErrorMaxConnectionLimit(AppSettings.Current.VDirName);
				ExTraceGlobals.RunspaceConfigTracer.TraceError(0L, localizedString);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_ReachedMaxPSConnectionLimit, null, new object[]
				{
					AppSettings.Current.VDirName,
					AppSettings.Current.MaxPowershellAppPoolConnections
				});
				throw new AuthorizationException(localizedString);
			}
		}

		private void IncreasePowershellConnections()
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Increase PowershellConnections for {0}.", this.currentAuthZUserToken.UserName);
			if (this.connCounterStatus != ExchangeAuthorizationPlugin.ConnCounterStatus.WaitingIncreasement)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceError<ExchangeAuthorizationPlugin.ConnCounterStatus>(0L, "Try to increase the powershell connection counter, but connCounterStatus is NOT WaitingIncreasement, instead it is {0}.", this.connCounterStatus);
				this.connCounterStatus = ExchangeAuthorizationPlugin.ConnCounterStatus.Error;
				return;
			}
			this.activeRunSpaceCostHandle = WSManBudgetManager.Instance.StartRunspace(this.currentAuthZUserToken);
			if (this.currentAuthZUserToken.OrgId != null)
			{
				this.tenantActiveRunspaceCostHandle = WSManTenantBudgetManager.Instance.StartRunspace(this.currentAuthZUserToken);
			}
			this.connCounterStatus = ExchangeAuthorizationPlugin.ConnCounterStatus.Increased;
			AuthZLogger.SafeSetLogger(RpsAuthZMetadata.ServerActiveRunspaces, WSManBudgetManager.Instance.TotalActiveRunspaces);
			AuthZLogger.SafeSetLogger(RpsAuthZMetadata.ServerActiveUsers, WSManBudgetManager.Instance.TotalActiveUsers);
			AuthZLogger.SafeSetLogger(RpsAuthZMetadata.UserBudgetOnStart, WSManBudgetManager.Instance.GetWSManBudgetUsage(this.currentAuthZUserToken));
			AuthZLogger.SafeSetLogger(RpsAuthZMetadata.TenantBudgetOnStart, WSManTenantBudgetManager.Instance.GetWSManBudgetUsage(this.currentAuthZUserToken));
			AuthZPluginHelper.UpdateAuthZPluginPerfCounters(WSManBudgetManager.Instance);
		}

		private void DecreasePowershellConnections()
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>(0L, "Decrease PowershellConnections for {0}.", (this.currentAuthZUserToken == null) ? null : this.currentAuthZUserToken.UserName);
			if (this.connCounterStatus != ExchangeAuthorizationPlugin.ConnCounterStatus.Increased)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceError<ExchangeAuthorizationPlugin.ConnCounterStatus>(0L, "Try to decrease the powershell connection counter, but connCounterStatus is NOT Increased, instead it is {0}.", this.connCounterStatus);
				this.connCounterStatus = ExchangeAuthorizationPlugin.ConnCounterStatus.Error;
				return;
			}
			AuthZPluginHelper.DisposeCostHandleAndSetToNull(ref this.activeRunSpaceCostHandle);
			AuthZPluginHelper.DisposeCostHandleAndSetToNull(ref this.tenantActiveRunspaceCostHandle);
			if (this.currentAuthZUserToken != null)
			{
				WSManBudgetManager.Instance.RemoveBudgetIfNoActiveRunspace(this.currentAuthZUserToken);
				if (this.currentAuthZUserToken.OrgId != null)
				{
					WSManTenantBudgetManager.Instance.RemoveBudgetIfNoActiveRunspace(this.currentAuthZUserToken);
				}
			}
			this.connCounterStatus = ExchangeAuthorizationPlugin.ConnCounterStatus.Decreased;
			AuthZPluginHelper.UpdateAuthZPluginPerfCounters(WSManBudgetManager.Instance);
		}

		public override PSPrimitiveDictionary GetApplicationPrivateData(PSSenderInfo senderInfo)
		{
			if (!Constants.IsPowerShellWebService)
			{
				ExchangeAuthorizationPlugin.InitializeAuthZPluginForRemotePS(senderInfo.ConnectionString);
			}
			ExchangeAuthorizationPlugin.EnsureSettingOverrideSyncIsStarted();
			return AuthZLogHelper.StartAndEndLoging<PSPrimitiveDictionary>("GetApplicationPrivateData", () => AuthZLogHelper.ExecuteWSManPluginAPI<PSPrimitiveDictionary>("GetApplicationPrivateData", true, true, null, delegate()
			{
				InitialSessionState initialSessionStateCore = this.GetInitialSessionStateCore(senderInfo);
				if (!Constants.IsPowerShellWebService)
				{
					this.CheckSessionOverBudget();
				}
				this.currentUserISS.Target = initialSessionStateCore;
				int value = InitialSessionStateBuilder.CalculateHashForImplicitRemoting(initialSessionStateCore);
				PSPrimitiveDictionary psprimitiveDictionary = new PSPrimitiveDictionary();
				psprimitiveDictionary.Add("ImplicitRemoting", new PSPrimitiveDictionary
				{
					{
						"Hash",
						value
					}
				});
				if (!Constants.IsPowerShellWebService)
				{
					psprimitiveDictionary.Add("SupportedVersions", this.ExpandVersions(senderInfo.ConnectionString));
				}
				this.LogCommonValues();
				return psprimitiveDictionary;
			}));
		}

		private string ExpandVersions(string connectionString)
		{
			Uri uri = new Uri(connectionString, UriKind.Absolute);
			NameValueCollection nameValueCollectionFromUri = LiveIdBasicAuthModule.GetNameValueCollectionFromUri(uri);
			string text = nameValueCollectionFromUri.Get("ExchClientVer");
			string text2 = nameValueCollectionFromUri.Get("ClientApplication");
			StringBuilder stringBuilder = new StringBuilder();
			string supportedEMCVersions = AppSettings.Current.SupportedEMCVersions;
			if (supportedEMCVersions.Contains("*"))
			{
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2) && text2.Equals("EMC", StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						ExchangeBuild exchangeBuild = ExchangeBuild.Parse(text);
						string[] array = supportedEMCVersions.Split(new char[]
						{
							';'
						});
						foreach (string text3 in array)
						{
							if (!string.IsNullOrEmpty(text3))
							{
								if (text3.Contains("*"))
								{
									string[] array3 = text3.Split(new char[]
									{
										'.'
									});
									int num;
									int num2;
									if (array3.Length == 4 && int.TryParse(array3[0], out num) && num == (int)exchangeBuild.Major && (string.Equals(array3[1], "*") || (int.TryParse(array3[1], out num2) && num2 == (int)exchangeBuild.Minor && string.Equals(array3[2], "*"))))
									{
										for (int j = 1; j < 999; j++)
										{
											stringBuilder.Append(string.Format(";{0}.{1}.{2}.0", exchangeBuild.Major, exchangeBuild.Minor, j));
										}
										break;
									}
								}
								else
								{
									stringBuilder.Append(string.Format(";{0}", text3));
								}
							}
						}
						goto IL_1D3;
					}
					catch (ArgumentException)
					{
						stringBuilder.Append(SupportedVersionList.DefaultVersionString);
						goto IL_1D3;
					}
				}
				stringBuilder.Append(SupportedVersionList.DefaultVersionString);
			}
			else
			{
				stringBuilder.Append(supportedEMCVersions);
			}
			IL_1D3:
			return stringBuilder.ToString();
		}

		public override int? GetMaximumReceivedObjectSize(PSSenderInfo senderInfo)
		{
			return new int?(AppSettings.Current.PSMaximumReceivedObjectSizeMB);
		}

		public override int? GetMaximumReceivedDataSizePerCommand(PSSenderInfo senderInfo)
		{
			return new int?(AppSettings.Current.PSMaximumReceivedDataSizePerCommandMB);
		}

		public override InitialSessionState GetInitialSessionState(PSSenderInfo senderInfo)
		{
			return AuthZLogHelper.StartAndEndLoging<InitialSessionState>("GetInitialSessionState", () => AuthZLogHelper.ExecuteWSManPluginAPI<InitialSessionState>("GetInitialSessionState", true, true, null, delegate()
			{
				if (senderInfo == null || senderInfo.UserInfo == null || senderInfo.UserInfo.Identity == null || senderInfo.UserInfo.Identity.Name == null)
				{
					throw new ArgumentException("senderInfo");
				}
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[EAP.GetInitialSessionState] Enter.");
				this.PreGetInitialSessionState(senderInfo);
				InitialSessionState initialSessionState = this.currentUserISS.Target as InitialSessionState;
				this.currentUserISS.Target = null;
				if (initialSessionState == null)
				{
					initialSessionState = this.GetInitialSessionStateCore(senderInfo);
				}
				this.PostGetInitialSessionState(senderInfo);
				return initialSessionState;
			}));
		}

		protected virtual void PreGetInitialSessionState(PSSenderInfo senderInfo)
		{
		}

		protected virtual void PostGetInitialSessionState(PSSenderInfo senderInfo)
		{
			this.IncreasePowershellConnections();
			this.LogCommonValues();
		}

		protected virtual void OnGetInitialSessionStateError(PSSenderInfo senderInfo, Exception exception)
		{
		}

		protected override void Dispose(bool isDisposing)
		{
			AuthZLogHelper.StartAndEndLoging("Dispose", delegate()
			{
				AuthZLogHelper.ExecuteWSManPluginAPI("Dispose", false, false, delegate()
				{
					if (isDisposing)
					{
						this.OnDispose();
					}
					this.<>n__FabricatedMethode(isDisposing);
				});
			});
		}

		protected virtual void OnDispose()
		{
			this.DecreasePowershellConnections();
			this.LogCommonValues();
		}

		protected virtual IIdentity GetExecutingUserIdentity(PSPrincipal psPrincipal, string connectionUrl, out UserToken userToken, out Microsoft.Exchange.Configuration.Core.AuthenticationType authenticationType)
		{
			return ExchangeAuthorizationPlugin.InternalGetExecutingUserIdentity(psPrincipal, connectionUrl, out userToken, out authenticationType, out this.sessionId, out this.firstRequestId);
		}

		public static SecurityIdentifier GetExecutingUserSecurityIdentifier(PSPrincipal psPrincipal, string connectionUrl)
		{
			if (psPrincipal == null)
			{
				throw new ArgumentNullException("psPrincipal");
			}
			UserToken userToken = null;
			Microsoft.Exchange.Configuration.Core.AuthenticationType authenticationType;
			string text;
			string text2;
			IIdentity identity = ExchangeAuthorizationPlugin.InternalGetExecutingUserIdentity(psPrincipal, connectionUrl, out userToken, out authenticationType, out text, out text2);
			return identity.GetSecurityIdentifier();
		}

		private static IIdentity InternalGetExecutingUserIdentity(PSPrincipal psPrincipal, string connectionUrl, out UserToken userToken, out Microsoft.Exchange.Configuration.Core.AuthenticationType authenticationType, out string sessionId, out string firstRequestId)
		{
			authenticationType = Microsoft.Exchange.Configuration.Core.AuthenticationType.Unknown;
			userToken = null;
			sessionId = null;
			firstRequestId = null;
			if (psPrincipal.Identity.AuthenticationType.StartsWith("Cafe-", StringComparison.OrdinalIgnoreCase))
			{
				using (WinRMDataReceiver winRMDataReceiver = new WinRMDataReceiver(connectionUrl, psPrincipal.Identity.Name, psPrincipal.Identity.AuthenticationType, AuthZLogHelper.LantencyTracker))
				{
					userToken = winRMDataReceiver.UserToken;
					sessionId = winRMDataReceiver.SessionId;
					firstRequestId = winRMDataReceiver.RequestId;
					string text = winRMDataReceiver.AuthenticationType.Substring("Cafe-".Length);
					if (text.Equals("GenericIdentity", StringComparison.OrdinalIgnoreCase))
					{
						return AuthZPluginHelper.ConstructGenericIdentityFromUserToken(userToken);
					}
					if (userToken.CommonAccessToken != null)
					{
						return new WindowsTokenIdentity(userToken.CommonAccessToken.WindowsAccessToken).ToSerializedIdentity();
					}
				}
			}
			if (DelegatedPrincipal.DelegatedAuthenticationType.Equals(psPrincipal.Identity.AuthenticationType, StringComparison.OrdinalIgnoreCase))
			{
				authenticationType = Microsoft.Exchange.Configuration.Core.AuthenticationType.RemotePowerShellDelegated;
				return DelegatedPrincipal.GetDelegatedIdentity(psPrincipal.Identity.Name);
			}
			if (psPrincipal.WindowsIdentity != null)
			{
				string authenticationType2 = psPrincipal.Identity.AuthenticationType;
				if (authenticationType2 != null && authenticationType2.StartsWith("Converted-", StringComparison.OrdinalIgnoreCase))
				{
					if (authenticationType2.StartsWith("Converted-Kerberos", StringComparison.OrdinalIgnoreCase))
					{
						authenticationType = Microsoft.Exchange.Configuration.Core.AuthenticationType.Kerberos;
					}
					else
					{
						AuthZLogger.SafeAppendGenericError("InternalGetExecutingUserIdentity", "Unexpected AuthenticationType " + authenticationType2, true);
					}
					using (WinRMDataReceiver winRMDataReceiver2 = new WinRMDataReceiver(connectionUrl, psPrincipal.Identity.Name, psPrincipal.Identity.AuthenticationType, AuthZLogHelper.LantencyTracker))
					{
						userToken = winRMDataReceiver2.UserToken;
						sessionId = winRMDataReceiver2.SessionId;
						firstRequestId = winRMDataReceiver2.RequestId;
						if (userToken.CommonAccessToken == null)
						{
							throw new AuthzException("DEV BUG, the CommonAccessToken should not be NULL when passing from Locally Kerberos logon.");
						}
						return new WindowsTokenIdentity(userToken.CommonAccessToken.WindowsAccessToken).ToSerializedIdentity();
					}
				}
				if ("CertificateLinkedUser".Equals(authenticationType2, StringComparison.OrdinalIgnoreCase))
				{
					authenticationType = Microsoft.Exchange.Configuration.Core.AuthenticationType.CertificateLinkedUser;
					return new GenericIdentity(psPrincipal.Identity.Name);
				}
				try
				{
					authenticationType = Microsoft.Exchange.Configuration.Core.AuthenticationType.Certificate;
					new SecurityIdentifier(psPrincipal.Identity.Name);
					return new GenericIdentity(psPrincipal.Identity.Name);
				}
				catch (ArgumentException)
				{
					authenticationType = Microsoft.Exchange.Configuration.Core.AuthenticationType.Unknown;
					return psPrincipal.WindowsIdentity;
				}
			}
			if ("RPS".Equals(psPrincipal.Identity.AuthenticationType, StringComparison.OrdinalIgnoreCase) || "Kerberos".Equals(psPrincipal.Identity.AuthenticationType, StringComparison.OrdinalIgnoreCase) || "Basic".Equals(psPrincipal.Identity.AuthenticationType, StringComparison.OrdinalIgnoreCase))
			{
				authenticationType = Microsoft.Exchange.Configuration.Core.AuthenticationType.Kerberos;
				SecurityIdentifier securityIdentifier = (SecurityIdentifier)new NTAccount(psPrincipal.Identity.Name).Translate(typeof(SecurityIdentifier));
				return new GenericIdentity(securityIdentifier.ToString());
			}
			authenticationType = Microsoft.Exchange.Configuration.Core.AuthenticationType.Unknown;
			return new GenericIdentity(psPrincipal.Identity.Name);
		}

		private static ExchangeConfigurationUnit GetExchangeConfigurationUnitByNameOrAcceptedDomain(string organization)
		{
			PartitionId partitionIdByAcceptedDomainName = ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(organization);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsPartitionId(partitionIdByAcceptedDomainName), 733, "GetExchangeConfigurationUnitByNameOrAcceptedDomain", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeAuthorizationPlugin.cs");
			return tenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(organization);
		}

		internal static bool TryFindOrganizationIdForDelegatedPrincipal(DelegatedPrincipal principal, out OrganizationId orgId)
		{
			orgId = null;
			ExchangeConfigurationUnit exchangeConfigurationUnit = null;
			Exception ex = null;
			try
			{
				exchangeConfigurationUnit = ExchangeAuthorizationPlugin.GetExchangeConfigurationUnitByNameOrAcceptedDomain(principal.DelegatedOrganization);
			}
			catch (CannotResolveTenantNameException ex2)
			{
				ex = ex2;
			}
			catch (DataSourceOperationException ex3)
			{
				ex = ex3;
			}
			catch (TransientException ex4)
			{
				ex = ex4;
			}
			catch (DataValidationException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				AuthZLogger.SafeAppendGenericError("TryFindOrganizationIdForDelegatedPrincipal", ex, new Func<Exception, bool>(KnownException.IsUnhandledException));
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_FailedToResolveOrganizationIdForDelegatedPrincipal, null, new object[]
				{
					principal.DelegatedOrganization,
					ex
				});
				return false;
			}
			orgId = exchangeConfigurationUnit.OrganizationId;
			return true;
		}

		internal static ADRawEntry FindUserEntry(SecurityIdentifier userSid, WindowsIdentity windowsIdentity, SerializedIdentity serializedIdentity, PartitionId partitionId)
		{
			ADRawEntry result;
			using (new MonitoredScope("FindUserEntry", "FindUserEntry", AuthZLogHelper.AuthZPerfMonitors))
			{
				ADSessionSettings sessionSettings;
				if (partitionId != null)
				{
					sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
				}
				else
				{
					sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				}
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 817, "FindUserEntry", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeAuthorizationPlugin.cs");
				ADRawEntry adrawEntry = tenantOrRootOrgRecipientSession.FindMiniRecipientBySid<MiniRecipient>(userSid, ExchangeRunspaceConfiguration.userPropertyArray);
				if (adrawEntry == null && VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ServiceAccountForest.Enabled)
				{
					adrawEntry = PartitionDataAggregator.GetMiniRecipientFromUserId(userSid, ExchangeRunspaceConfiguration.userPropertyArray, ConsistencyMode.IgnoreInvalid);
				}
				if (adrawEntry == null)
				{
					ExTraceGlobals.AccessDeniedTracer.TraceWarning<SecurityIdentifier, string>(0L, "EAP.FindUserEntry user {0} could not be found in AD, partitionId: {1}", userSid, (partitionId == null) ? "null" : partitionId.ToString());
					adrawEntry = ExchangeRunspaceConfiguration.TryFindComputer(userSid);
				}
				if (adrawEntry == null && (windowsIdentity != null || serializedIdentity != null))
				{
					ExTraceGlobals.AccessDeniedTracer.TraceWarning<SecurityIdentifier>(0L, "EAP.FindUserEntry computer {0} could not be found in AD", userSid);
					IIdentity identity = (windowsIdentity != null) ? windowsIdentity : serializedIdentity;
					ICollection<SecurityIdentifier> groupAccountsSIDs = identity.GetGroupAccountsSIDs();
					tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 850, "FindUserEntry", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\ExchangeAuthorizationPlugin.cs");
					List<ADObjectId> list = null;
					if (ExchangeRunspaceConfiguration.TryFindLinkedRoleGroupsBySidList(tenantOrRootOrgRecipientSession, groupAccountsSIDs, identity.Name, out list))
					{
						adrawEntry = new ADUser
						{
							RemotePowerShellEnabled = true
						};
					}
				}
				result = adrawEntry;
			}
			return result;
		}

		private static void ValidateQueryString(string httpURL, ADRawEntry userEntry)
		{
			string text;
			string tenantOrganization = ExchangeRunspaceConfigurationSettings.FromUriConnectionString(httpURL, out text).TenantOrganization;
			string value = tenantOrganization;
			if (string.IsNullOrEmpty(tenantOrganization))
			{
				return;
			}
			AuthZLogger.SafeSetLogger(ConfigurationCoreMetadata.ManagedOrganization, value);
			OrganizationId organizationId = userEntry[ADObjectSchema.OrganizationId] as OrganizationId;
			if (organizationId != null && !ExchangeRunspaceConfiguration.IsAllowedOrganizationForPartnerAccounts(organizationId))
			{
				AuthZLogger.SafeAppendGenericError("ValidateQueryString", string.Format("Organization {0} not allowed for partner account.", organizationId.ToString()), false);
				ExTraceGlobals.AccessDeniedTracer.TraceError<ADObjectId>(0L, "EAP.ValidateQueryString returns AccessDenied because user {0} does not belong to the appropriate organization", userEntry.Id);
				throw new NotAllowedForPartnerAccessException(Strings.ErrorNotAllowedForPartnerAccess);
			}
			Uri uri;
			if (!Uri.TryCreate(httpURL, UriKind.Absolute, out uri))
			{
				AuthZLogger.SafeAppendGenericError("ValidateQueryString", string.Format("Url {0} Incorrect.", httpURL), false);
				ExTraceGlobals.AccessDeniedTracer.TraceError<ADObjectId, string, string>(0L, "EAP.ValidateQueryString returns AccessDenied because tenant user {0} requested partner access to {1} which is not found in the local forest and URL {2} is invalid", userEntry.Id, tenantOrganization, httpURL);
				throw new UrlInValidException(Strings.ErrorUrlInValid);
			}
			ExTraceGlobals.ADConfigTracer.TraceDebug(0L, "EAP.ValidateQueryString returns Success.");
		}

		private static void InitializeExchangeAuthZPluginPerfCounter(string vdirName, int port)
		{
			if (ExchangeAuthorizationPlugin.perfCounter == null)
			{
				if (Constants.IsPowerShellWebService)
				{
					ExchangeAuthorizationPlugin.perfCounter = RemotePowershellPerformanceCounters.GetInstance("Psws");
				}
				else
				{
					ExchangeAuthorizationPlugin.perfCounter = RPSPerfCounterHelper.GetPerfCounterForAuthZ(vdirName, port);
					Globals.InitializeMultiPerfCounterInstance(ExchangeAuthorizationPlugin.perfCounter.Name);
				}
				ExchangeAuthorizationPlugin.perfCounter.CurrentSessions.RawValue = 0L;
				ExchangeAuthorizationPlugin.perfCounter.CurrentUniqueUsers.RawValue = 0L;
				ExchangeAuthorizationPlugin.perfCounter.ConnectionLeak.RawValue = 0L;
			}
		}

		private static void InitializeAuthZPluginForRemotePS(string connectionUri)
		{
			if (AppSettings.RpsAuthZAppSettingsInitialized)
			{
				return;
			}
			AppSettings.InitializeManualLoadAppSettings(connectionUri, delegate
			{
				IAppSettings appSettings = AppSettings.Current;
				if (appSettings.SupportedEMCVersions == null)
				{
					((ManualLoadAppSettings)appSettings).SupportedEMCVersions = SupportedVersionList.DefaultVersionString;
				}
				AppDomain.CurrentDomain.SetupInformation.ConfigurationFile = appSettings.ConfigurationFilePath;
				ConfigFiles.SetConfigSource(appSettings.VDirName, appSettings.WebSiteName);
				FailFastUserCache.FailFastEnabled = appSettings.FailFastEnabled;
				Uri uri = new Uri(connectionUri, UriKind.Absolute);
				ExchangeAuthorizationPlugin.InitializeExchangeAuthZPluginPerfCounter(appSettings.VDirName, uri.Port);
				ProvisioningCache.InitializeAppRegistrySettings(appSettings.ProvisioningCacheIdentification);
				ThreadPool.SetMaxThreads(appSettings.ThreadPoolMaxThreads, appSettings.ThreadPoolMaxCompletionPorts);
				ThrottlingPerfCounterWrapper.Initialize(BudgetType.PowerShell);
				ADSession.DisableAdminTopologyMode();
			});
		}

		private static void InitWatsonForRemotePowershell()
		{
			ExWatson.Register("E12");
		}

		private static void EnsureSettingOverrideSyncIsStarted()
		{
			if (!ExchangeAuthorizationPlugin.SettingOverrideSyncStarted)
			{
				SettingOverrideSync.Instance.Start(true);
				ExchangeAuthorizationPlugin.SettingOverrideSyncStarted = true;
			}
		}

		private InitialSessionState GetInitialSessionStateCore(PSSenderInfo senderInfo)
		{
			InitialSessionState result;
			using (new MonitoredScope("GetInitialSessionStateCore", "GetInitialSessionStateCore", AuthZLogHelper.AuthZPerfMonitors))
			{
				if (senderInfo == null || senderInfo.UserInfo == null || senderInfo.UserInfo.Identity == null || senderInfo.UserInfo.Identity.Name == null)
				{
					throw new ArgumentException("senderInfo");
				}
				PSPrincipal userInfo = senderInfo.UserInfo;
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>((long)this.GetHashCode(), "Entering EAP.GetInitialSessionState({0})", userInfo.Identity.Name);
				UserToken userToken = null;
				Microsoft.Exchange.Configuration.Core.AuthenticationType authenticatedType;
				IIdentity executingUserIdentity = this.GetExecutingUserIdentity(userInfo, senderInfo.ConnectionString, out userToken, out authenticatedType);
				ExchangeRunspaceConfigurationSettings exchangeRunspaceConfigurationSettings = this.BuildRunspaceConfigurationSettings(senderInfo.ConnectionString, executingUserIdentity);
				if (userToken != null)
				{
					exchangeRunspaceConfigurationSettings.UserToken = userToken;
				}
				if (AppSettings.Current.SiteRedirectTemplate != null)
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "EAP.GetInitialSessionState({0}) site redirection template used is {1}, pod redirection template used is {2}", userInfo.Identity.Name, AppSettings.Current.SiteRedirectTemplate, AppSettings.Current.PodRedirectTemplate);
					exchangeRunspaceConfigurationSettings.SiteRedirectionTemplate = AppSettings.Current.SiteRedirectTemplate;
					exchangeRunspaceConfigurationSettings.PodRedirectionTemplate = AppSettings.Current.PodRedirectTemplate;
				}
				ExchangeExpiringRunspaceConfiguration exchangeExpiringRunspaceConfiguration;
				using (new MonitoredScope("GetInitialSessionStateCore", "ExchangeExpiringRunspaceConfiguration", AuthZLogHelper.AuthZPerfMonitors))
				{
					if (DatacenterRegistry.IsForefrontForOffice())
					{
						try
						{
							using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\Setup", "v15")))
							{
								string name = "Microsoft.Exchange.Hygiene.Security.Authorization.ForefrontExpiringDatacenterRunspaceConfiguration";
								string path = (string)registryKey.GetValue("MsiInstallPath");
								string assemblyFile = Path.Combine(path, "Bin", "Microsoft.Exchange.Hygiene.Security.Authorization.dll");
								Assembly assembly = Assembly.LoadFrom(assemblyFile);
								Type type = assembly.GetType(name);
								exchangeExpiringRunspaceConfiguration = (ExchangeExpiringRunspaceConfiguration)type.InvokeMember("Instance", BindingFlags.InvokeMethod, Type.DefaultBinder, null, new object[]
								{
									executingUserIdentity,
									exchangeRunspaceConfigurationSettings,
									senderInfo.ConnectionString,
									Constants.IsPowerShellWebService
								});
							}
							goto IL_1FA;
						}
						catch (TargetInvocationException ex)
						{
							throw ex.InnerException ?? ex;
						}
					}
					exchangeExpiringRunspaceConfiguration = new ExchangeExpiringRunspaceConfiguration(executingUserIdentity, exchangeRunspaceConfigurationSettings, Constants.IsPowerShellWebService);
					IL_1FA:;
				}
				this.currentAuthZUserToken = new AuthZPluginUserToken(exchangeExpiringRunspaceConfiguration.DelegatedPrincipal, exchangeExpiringRunspaceConfiguration.LogonUser, authenticatedType, exchangeExpiringRunspaceConfiguration.IdentityName);
				ADRawEntry logonUser = exchangeExpiringRunspaceConfiguration.LogonUser;
				if (logonUser[ADRecipientSchema.RemotePowerShellEnabled] != null && !(bool)logonUser[ADRecipientSchema.RemotePowerShellEnabled])
				{
					AuthZLogger.SafeAppendGenericError("GetInitialSessionStateCore", "RemotePowerShellEnabled false", false);
					ExTraceGlobals.AccessDeniedTracer.TraceError<string>(0L, "EAP.GetInitialSessionStateCore user {0} is not allowed to use remote Powershell, access denied", executingUserIdentity.Name);
					AuthZPluginHelper.TriggerFailFastForAuthZFailure(this.currentAuthZUserToken.WindowsLiveId);
					throw new RemotePowerShellNotEnabledException(Strings.ErrorRemotePowerShellNotEnabled);
				}
				if (exchangeExpiringRunspaceConfiguration.DelegatedPrincipal == null)
				{
					ExchangeAuthorizationPlugin.ValidateQueryString(senderInfo.ConnectionString, logonUser);
				}
				else if (exchangeExpiringRunspaceConfiguration.DelegatedPrincipal.UserOrganizationId == null)
				{
					AuthZLogger.SafeAppendGenericError("GetInitialSessionStateCore", "User Token is delegated user, but user.OrgId is null.", false);
					ExTraceGlobals.AccessDeniedTracer.TraceError(0L, "EAP.GetInitialSessionStateCore delegated user is not in organization.");
					AuthZPluginHelper.TriggerFailFastForAuthZFailure(this.currentAuthZUserToken.WindowsLiveId);
					throw new DelegatedUserNotInOrgException(Strings.ErrorDelegatedUserNotInOrg);
				}
				string friendlyName = exchangeExpiringRunspaceConfiguration.OrganizationId.GetFriendlyName();
				if (exchangeExpiringRunspaceConfiguration.HasAdminRoles && exchangeExpiringRunspaceConfiguration.IsAppPasswordUsed)
				{
					AuthZLogger.SafeAppendGenericError("GetInitialSessionStateCore", string.Format("User {0} of Domain {1} is not allowed to create session using app password.", userInfo.Identity.Name, friendlyName), false);
					AuthZPluginHelper.TriggerFailFastForAuthZFailure(this.currentAuthZUserToken.WindowsLiveId);
					throw new AppPasswordLoginException(Strings.ErrorAdminLoginUsingAppPassword);
				}
				if (string.Equals(executingUserIdentity.AuthenticationType, "LiveIdBasic", StringComparison.OrdinalIgnoreCase) || DelegatedPrincipal.DelegatedAuthenticationType.Equals(executingUserIdentity.AuthenticationType, StringComparison.OrdinalIgnoreCase))
				{
					using (new MonitoredScope("GetInitialSessionStateCore", "ValidateFilteringOnlyUser", AuthZLogHelper.AuthZPerfMonitors))
					{
						if (UserValidationHelper.ValidateFilteringOnlyUser(friendlyName, this.currentAuthZUserToken.WindowsLiveId))
						{
							AuthZLogger.SafeAppendGenericError("GetInitialSessionStateCore", string.Format("User {0} of Domain {1} doesn't have valid subscriptions for Exchange Hosted.", userInfo.Identity.Name, friendlyName), false);
							AuthZPluginHelper.TriggerFailFastForAuthZFailure(this.currentAuthZUserToken.WindowsLiveId);
							throw new FilteringOnlyUserLoginException(Strings.ErrorFilteringOnlyUserLogin);
						}
					}
				}
				InitialSessionState initialSessionState;
				using (new MonitoredScope("GetInitialSessionStateCore", "exchangeRunspaceConfig.CreateInitialSessionState", AuthZLogHelper.AuthZPerfMonitors))
				{
					initialSessionState = exchangeExpiringRunspaceConfiguration.CreateInitialSessionState();
				}
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<int>((long)this.GetHashCode(), "EAP.GetInitialSessionState(PSSenderInfo) returns ISS with {0} commands", initialSessionState.Commands.Count);
				result = initialSessionState;
			}
			return result;
		}

		protected virtual ExchangeRunspaceConfigurationSettings BuildRunspaceConfigurationSettings(string connectionString, IIdentity identity)
		{
			string text;
			return ExchangeRunspaceConfigurationSettings.FromUriConnectionString(connectionString, ExchangeRunspaceConfigurationSettings.ExchangeApplication.PowerShell, out text);
		}

		private void LogCommonValues()
		{
			AuthZLogger.SafeSetLogger(RpsCommonMetadata.SessionId, this.sessionId);
			AuthZLogger.SafeAppendGenericInfo("FirstRequestId", this.firstRequestId);
			AuthZLogHelper.LogAuthZUserToken(this.currentAuthZUserToken);
		}

		private const string PowerShellWebServicePerfCounterInstanceName = "Psws";

		internal const string CafeAuthenticationTypePrefix = "Cafe-";

		private const string GenericIdentityAuthenticationType = "GenericIdentity";

		private WeakReference currentUserISS = new WeakReference(null);

		private AuthZPluginUserToken currentAuthZUserToken;

		private CostHandle activeRunSpaceCostHandle;

		private CostHandle tenantActiveRunspaceCostHandle;

		private string sessionId;

		private string firstRequestId;

		private ExchangeAuthorizationPlugin.ConnCounterStatus connCounterStatus;

		private static RemotePowershellPerformanceCountersInstance perfCounter = null;

		private static FileSearchAssemblyResolver fileSearchAssemblyResolver = new FileSearchAssemblyResolver();

		private static bool SettingOverrideSyncStarted = false;

		private enum ConnCounterStatus
		{
			WaitingIncreasement,
			Increased,
			Decreased,
			Error
		}
	}
}
