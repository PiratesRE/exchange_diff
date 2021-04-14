using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class AirSyncUser : IAirSyncUser
	{
		internal AirSyncUser(IAirSyncContext context)
		{
			this.context = context;
			bool flag = false;
			bool flag2 = true;
			try
			{
				if (this.context.Request.WasFromCafe)
				{
					this.InitializeFromRehydratedIdentity();
				}
				else if (this.context.Request.WasProxied)
				{
					flag2 = false;
					this.InitializeFromClientSecurityContext();
				}
				else
				{
					this.InitializeFromLoggedOnIdentity();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					((IAirSyncUser)this).DisposeBudget();
					if (flag2 && this.clientSecurityContextWrapper != null)
					{
						this.clientSecurityContextWrapper.Dispose();
						this.clientSecurityContextWrapper = null;
					}
				}
			}
		}

		internal IStandardBudget Budget { get; private set; }

		internal IEasFeaturesManager Features { get; private set; }

		IAirSyncContext IAirSyncUser.Context
		{
			get
			{
				return this.context;
			}
		}

		Guid IAirSyncUser.DeviceBehaviorCacheGuid
		{
			get
			{
				return this.deviceBehaviorCacheGuid;
			}
		}

		IStandardBudget IAirSyncUser.Budget
		{
			get
			{
				return this.Budget;
			}
		}

		IIdentity IAirSyncUser.Identity
		{
			get
			{
				return this.context.Principal.Identity;
			}
		}

		WindowsIdentity IAirSyncUser.WindowsIdentity
		{
			get
			{
				return (WindowsIdentity)this.context.Principal.Identity;
			}
		}

		byte[] IAirSyncUser.SID
		{
			get
			{
				return this.clientSecurityContextWrapper.UserSidBytes;
			}
		}

		ExchangePrincipal IAirSyncUser.ExchangePrincipal
		{
			get
			{
				if (this.exchangePrincipalInitialized)
				{
					return this.exchangePrincipal;
				}
				this.InitializeExchangePrincipal();
				return this.exchangePrincipal;
			}
		}

		WindowsPrincipal IAirSyncUser.WindowsPrincipal
		{
			get
			{
				return this.windowsPrincipal;
			}
		}

		bool IAirSyncUser.IsEnabled
		{
			get
			{
				return this.activeDirectoryUser != null && this.activeDirectoryUser.ActiveSyncEnabled;
			}
		}

		ActiveSyncMiniRecipient IAirSyncUser.ADUser
		{
			get
			{
				((IAirSyncUser)this).InitializeADUser();
				return this.activeDirectoryUser;
			}
		}

		OrganizationId IAirSyncUser.OrganizationId
		{
			get
			{
				if (((IAirSyncUser)this).ADUser == null)
				{
					return null;
				}
				return ((IAirSyncUser)this).ADUser.OrganizationId;
			}
		}

		bool IAirSyncUser.MailboxIsOnE12Server
		{
			get
			{
				return this.mailboxIsOnE12Server;
			}
			set
			{
				this.mailboxIsOnE12Server = value;
			}
		}

		bool IAirSyncUser.IsMonitoringTestUser
		{
			get
			{
				return string.Compare(this.context.Request.UserAgent, "TestActiveSyncConnectivity", StringComparison.OrdinalIgnoreCase) == 0;
			}
		}

		ClientSecurityContextWrapper IAirSyncUser.ClientSecurityContextWrapper
		{
			get
			{
				return this.clientSecurityContextWrapper;
			}
		}

		string IAirSyncUser.Name
		{
			get
			{
				return this.username;
			}
		}

		string IAirSyncUser.ServerFullyQualifiedDomainName
		{
			get
			{
				if (this.exchangePrincipal != null)
				{
					return this.exchangePrincipal.MailboxInfo.Location.ServerFqdn;
				}
				return null;
			}
		}

		Guid IAirSyncUser.MailboxGuid
		{
			get
			{
				return ((IAirSyncUser)this).ExchangePrincipal.MailboxInfo.MailboxGuid;
			}
		}

		string IAirSyncUser.DisplayName
		{
			get
			{
				return ((IAirSyncUser)this).ExchangePrincipal.MailboxInfo.DisplayName;
			}
		}

		string IAirSyncUser.SmtpAddress
		{
			get
			{
				return ((IAirSyncUser)this).ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			}
		}

		bool IAirSyncUser.IrmEnabled
		{
			get
			{
				if (ADNotificationManager.GetPolicyData(this) == null || !ADNotificationManager.GetPolicyData(this).IsIrmEnabled)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "IRM feature disabled via EAS policy for user {0}", ((IAirSyncUser)this).DisplayName);
					return false;
				}
				bool result;
				try
				{
					result = RmsClientManager.IRMConfig.IsClientAccessServerEnabledForTenant(((IAirSyncUser)this).OrganizationId);
				}
				catch (ExchangeConfigurationException ex)
				{
					AirSyncDiagnostics.TraceError<ExchangeConfigurationException>(ExTraceGlobals.RequestsTracer, null, "ExchangeConfigurationException while reading IRM Configuration: {0}", ex);
					throw new AirSyncPermanentException(StatusCode.IRM_TransientError, ex, false)
					{
						ErrorStringForProtocolLogger = "asuIeExchangeConfigurationException"
					};
				}
				catch (RightsManagementException ex2)
				{
					AirSyncDiagnostics.TraceError<RightsManagementException>(ExTraceGlobals.RequestsTracer, null, "RightsManagementException while reading IRM Configuration: {0}", ex2);
					throw new AirSyncPermanentException(ex2.IsPermanent ? StatusCode.IRM_PermanentError : StatusCode.IRM_TransientError, ex2, false)
					{
						ErrorStringForProtocolLogger = "asuIeRightsManagementException" + ex2.FailureCode.ToString()
					};
				}
				return result;
			}
		}

		string IAirSyncUser.WindowsLiveId
		{
			get
			{
				if (!GlobalSettings.IsWindowsLiveIDEnabled)
				{
					return null;
				}
				return this.context.WindowsLiveId;
			}
		}

		IEasFeaturesManager IAirSyncUser.Features
		{
			get
			{
				return this.Features;
			}
		}

		BudgetKey IAirSyncUser.BudgetKey
		{
			get
			{
				if (this.budgetKey == null)
				{
					if (GlobalSettings.UseTestBudget)
					{
						this.budgetKey = new UnthrottledBudgetKey(this.clientSecurityContextWrapper.UserSid.ToString(), BudgetType.Eas);
					}
					else
					{
						CommandType commandType = this.context.Request.CommandType;
						if (commandType != CommandType.Sync)
						{
							switch (commandType)
							{
							case CommandType.Ping:
							case CommandType.ItemOperations:
								break;
							default:
								this.budgetKey = new SidBudgetKey(this.clientSecurityContextWrapper.UserSid, BudgetType.Eas, false, ADUserCache.GetSessionSettings(((IAirSyncUser)this).ExchangePrincipal.MailboxInfo.OrganizationId, ((IAirSyncUser)this).Context.ProtocolLogger));
								goto IL_F1;
							}
						}
						this.budgetKey = new EasDeviceBudgetKey(this.clientSecurityContextWrapper.UserSid, this.context.DeviceIdentity.DeviceId, this.context.DeviceIdentity.DeviceType, ADUserCache.GetSessionSettings(((IAirSyncUser)this).ExchangePrincipal.MailboxInfo.OrganizationId, ((IAirSyncUser)this).Context.ProtocolLogger));
					}
				}
				IL_F1:
				return this.budgetKey;
			}
		}

		bool IAirSyncUser.IsConsumerOrganizationUser
		{
			get
			{
				return this.Features.IsEnabled(EasFeature.ConsumerOrganizationUser) || (((IAirSyncUser)this).ADUser != null && ((IAirSyncUser)this).ADUser.IsConsumerOrganization());
			}
		}

		BackOffValue IAirSyncUser.GetBudgetBackOffValue()
		{
			BackOffValue backOffValue = null;
			if (((IAirSyncUser)this).Budget != null)
			{
				ITokenBucket budgetTokenBucket = ((IAirSyncUser)this).GetBudgetTokenBucket();
				if (budgetTokenBucket == null)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.GetBudgetBackOffDuration] Budget does not contain a token bucket.  Likely unthrottled.");
					return BackOffValue.NoBackOffValue;
				}
				float balance = budgetTokenBucket.GetBalance();
				AirSyncDiagnostics.TraceInfo<float, int>(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.GetBudgetBackOffDuration]. Balance :{0}, RechargeRate:{1}", balance, budgetTokenBucket.RechargeRate);
				backOffValue = new BackOffValue
				{
					BackOffReason = "Budget"
				};
				if ((double)balance < GlobalSettings.BudgetBackOffMinThreshold.TotalMilliseconds)
				{
					backOffValue.BackOffDuration = Math.Ceiling((GlobalSettings.BudgetBackOffMinThreshold.TotalMilliseconds - (double)balance) * 60.0 * 60.0 / (double)budgetTokenBucket.RechargeRate);
					backOffValue.BackOffType = ((balance > (float)(ulong.MaxValue * (ulong)((IAirSyncUser)this).Budget.ThrottlingPolicy.EasMaxBurst.Value)) ? BackOffType.Medium : BackOffType.High);
				}
				else
				{
					backOffValue.BackOffDuration = Math.Ceiling((GlobalSettings.BudgetBackOffMinThreshold.TotalMilliseconds - (double)balance) / 1000.0);
				}
			}
			AirSyncDiagnostics.TraceInfo<double, BackOffType>(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.GetBudgetBackOffDuration]. BudgetBackOff Duration:{0} sec. BackOffType:{1}", backOffValue.BackOffDuration, backOffValue.BackOffType);
			return backOffValue ?? BackOffValue.NoBackOffValue;
		}

		void IAirSyncUser.DisposeBudget()
		{
			if (this.Budget == null)
			{
				return;
			}
			try
			{
				((IAirSyncUser)this).Budget.Dispose();
			}
			catch (FailFastException arg)
			{
				AirSyncDiagnostics.TraceError<FailFastException>(ExTraceGlobals.RequestsTracer, null, "Budget.Dispose failed with exception: {0}", arg);
			}
			if (this.context != null)
			{
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Budget, "(D)" + ((IAirSyncUser)this).Budget.ToString());
				if (GlobalSettings.WriteBudgetDiagnostics || this.context.User.IsMonitoringTestUser)
				{
					this.context.Response.AppendHeader("X-BudgetDiagnostics", ((IAirSyncUser)this).Budget.ToString());
				}
			}
			this.Budget = null;
		}

		void IAirSyncUser.PrepareToHang()
		{
			this.activeDirectoryUser = null;
		}

		void IAirSyncUser.AcquireBudget()
		{
			if (((IAirSyncUser)this).Budget != null)
			{
				return;
			}
			this.Budget = StandardBudget.Acquire(((IAirSyncUser)this).BudgetKey);
			((IAirSyncUser)this).SetBudgetDiagnosticValues(true);
			this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Budget, "(A)" + ((IAirSyncUser)this).Budget.ToString());
		}

		void IAirSyncUser.SetBudgetDiagnosticValues(bool start)
		{
			ITokenBucket budgetTokenBucket = ((IAirSyncUser)this).GetBudgetTokenBucket();
			if (budgetTokenBucket != null)
			{
				float balance = budgetTokenBucket.GetBalance();
				this.context.SetDiagnosticValue(start ? ConditionalHandlerSchema.BudgetBalanceStart : ConditionalHandlerSchema.BudgetBalanceEnd, balance);
				this.context.SetDiagnosticValue(start ? ConditionalHandlerSchema.IsOverBudgetAtStart : ConditionalHandlerSchema.IsOverBudgetAtEnd, balance < 0f);
			}
			StandardBudgetWrapper standardBudgetWrapper = ((IAirSyncUser)this).Budget as StandardBudgetWrapper;
			if (standardBudgetWrapper != null)
			{
				this.context.SetDiagnosticValue(start ? ConditionalHandlerSchema.ConcurrencyStart : ConditionalHandlerSchema.ConcurrencyEnd, standardBudgetWrapper.GetInnerBudget().Connections);
			}
		}

		void IAirSyncUser.InitializeADUser()
		{
			if (this.activeDirectoryUser == null)
			{
				this.activeDirectoryUser = ADUserCache.TryGetADUser(this, this.context.ProtocolLogger);
				if (this.activeDirectoryUser != null)
				{
					this.deviceBehaviorCacheGuid = this.activeDirectoryUser.OriginalId.ObjectGuid;
					this.Features = EasFeaturesManager.Create(this.activeDirectoryUser, this.context.FlightingOverrides);
				}
			}
		}

		ITokenBucket IAirSyncUser.GetBudgetTokenBucket()
		{
			StandardBudgetWrapper standardBudgetWrapper = ((IAirSyncUser)this).Budget as StandardBudgetWrapper;
			if (standardBudgetWrapper != null)
			{
				return standardBudgetWrapper.GetInnerBudget().CasTokenBucket;
			}
			return null;
		}

		private void InitializeFromClientSecurityContext()
		{
			string proxyHeader = this.context.Request.ProxyHeader;
			if (this.clientSecurityContextWrapper != null)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.InitializeFromClientSecurityContext]. clientSecurityContextWrapper is not null. calling dispose.");
				this.clientSecurityContextWrapper.Dispose();
			}
			this.clientSecurityContextWrapper = (ClientSecurityContextWrapper)HttpRuntime.Cache.Get(proxyHeader);
			if (this.clientSecurityContextWrapper == null)
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.InitializeFromClientSecurityContext] ProxyHeader key '{0}' was missing from HttpRuntime cache.  Returning HttpStatusNeedIdentity.", proxyHeader);
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MissingCscCacheEntry");
				AirSyncPermanentException ex = new AirSyncPermanentException((HttpStatusCode)441, StatusCode.None, null, false);
				throw ex;
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.InitializeFromClientSecurityContext] ProxyHeader key '{0}' was found in the HttpRuntime cache.  Reusing CSC for user.", proxyHeader);
			string[] array = proxyHeader.Split(",".ToCharArray(), 2);
			if (array.Length != 2)
			{
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadProxyHeader");
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidCombinationOfIDs, null, false);
			}
			this.username = array[1];
			((IAirSyncUser)this).InitializeADUser();
			((IAirSyncUser)this).AcquireBudget();
		}

		private void InitializeFromRehydratedIdentity()
		{
			if (this.clientSecurityContextWrapper != null)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.InitializeFromRehydratedIdentity]. clientSecurityContextWrapper is not null. calling dispose.");
				this.clientSecurityContextWrapper.Dispose();
			}
			this.clientSecurityContextWrapper = ClientSecurityContextWrapper.FromIdentity(((IAirSyncUser)this).Identity);
			this.username = ((IAirSyncUser)this).Identity.Name;
			AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.InitializeFromRehydratedIdentity] Hyrdating CSC from user {0}", this.username);
			((IAirSyncUser)this).InitializeADUser();
			((IAirSyncUser)this).AcquireBudget();
		}

		private void InitializeFromLoggedOnIdentity()
		{
			if (((IAirSyncUser)this).WindowsIdentity.User == null)
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.InitializeFromLoggedOnIdentity] Anonymous user is forbidden.");
				this.context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AnonymousUser");
				AirSyncPermanentException ex = new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.UserCannotBeAnonymous, EASServerStrings.AnonymousAccessError, true);
				throw ex;
			}
			this.windowsPrincipal = new WindowsPrincipal(((IAirSyncUser)this).WindowsIdentity);
			this.username = this.context.Request.LogonUserName;
			if (this.clientSecurityContextWrapper != null)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.InitializeFromLoggedOnIdentity]. clientSecurityContextWrapper is not null. calling dispose.");
				this.clientSecurityContextWrapper.Dispose();
			}
			this.clientSecurityContextWrapper = ClientSecurityContextWrapper.FromWindowsIdentity(((IAirSyncUser)this).WindowsIdentity);
			AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "[AirSyncUser.InitializeFromLoggedOnIdentity] Acquired CSC for user '{0}'", this.username);
			((IAirSyncUser)this).InitializeADUser();
			((IAirSyncUser)this).AcquireBudget();
		}

		private void InitializeExchangePrincipal()
		{
			try
			{
				this.exchangePrincipalInitialized = true;
				if (((IAirSyncUser)this).ADUser == null)
				{
					throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.UserHasNoMailbox, null, false)
					{
						ErrorStringForProtocolLogger = "UserHasNoMailbox"
					};
				}
				AirSyncDiagnostics.FaultInjectionPoint(3414568253U, delegate
				{
					this.exchangePrincipal = ExchangePrincipal.FromMiniRecipient(((IAirSyncUser)this).ADUser);
				}, delegate
				{
					throw new ObjectNotFoundException(new LocalizedString("FaultInjection ObjectNotFoundException"));
				});
			}
			catch (ObjectNotFoundException innerException)
			{
				string redirectAddressForUserHasNoMailbox = this.GetRedirectAddressForUserHasNoMailbox(((IAirSyncUser)this).ADUser);
				if (!string.IsNullOrEmpty(redirectAddressForUserHasNoMailbox))
				{
					throw new IncorrectUrlRequestException((HttpStatusCode)451, "X-MS-Location", redirectAddressForUserHasNoMailbox);
				}
				throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.UserHasNoMailbox, innerException, false)
				{
					ErrorStringForProtocolLogger = "UserHasNoMailbox"
				};
			}
		}

		private string GetRedirectAddressForUserHasNoMailbox(ActiveSyncMiniRecipient activesyncMiniRecipient)
		{
			string easEndpoint = null;
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.ActiveSync.RedirectForOnBoarding.Enabled)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "The hybrid on boarding redirect feature is only for OnPrem servers.");
				return null;
			}
			if (this.context.CommandType != CommandType.Options && this.context.AirSyncVersion < GlobalSettings.MinRedirectProtocolVersion)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "The protocol version is less than 14.0 that doesn't support 451 redirect protocol call.");
				return null;
			}
			AirSyncDiagnostics.FaultInjectionPoint(3414568253U, delegate
			{
				if (activesyncMiniRecipient != null && activesyncMiniRecipient.ExternalEmailAddress != null)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Try to figure out eas endpoint for user: {0}.", activesyncMiniRecipient.ExternalEmailAddress.AddressString);
					this.context.ProtocolLogger.SetValue(ProtocolLoggerData.RedirectTo, "TryToFigureOutEasEndpoint");
					SmtpProxyAddress smtpProxyAddress = activesyncMiniRecipient.ExternalEmailAddress as SmtpProxyAddress;
					if (smtpProxyAddress != null && !string.IsNullOrEmpty(smtpProxyAddress.AddressString))
					{
						OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(activesyncMiniRecipient.OrganizationId);
						string domain = ((SmtpAddress)smtpProxyAddress).Domain;
						OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(domain);
						if (organizationRelationship != null)
						{
							Uri targetOwaURL = organizationRelationship.TargetOwaURL;
							easEndpoint = this.TransferTargetOwaUrlToEasEndpoint(targetOwaURL);
							AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Redirect to EASEndpoint : {0}.", easEndpoint);
							this.context.ProtocolLogger.AppendValue(ProtocolLoggerData.RedirectTo, easEndpoint);
							return;
						}
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "OrganizationRelationShip is null for the domain {0}", domain);
						return;
					}
					else
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "External email address is null");
					}
				}
			}, delegate
			{
				Uri targetOwaUri = new Uri("http://outlook.com/owa");
				easEndpoint = this.TransferTargetOwaUrlToEasEndpoint(targetOwaUri);
			});
			return easEndpoint;
		}

		private string TransferTargetOwaUrlToEasEndpoint(Uri targetOwaUri)
		{
			string result = null;
			if (targetOwaUri != null && targetOwaUri.Host != null)
			{
				if (targetOwaUri.Host.Equals("outlook.com"))
				{
					result = string.Format("https://{0}/Microsoft-Server-ActiveSync", "outlook.office365.com");
				}
				else
				{
					result = string.Format("https://{0}/Microsoft-Server-ActiveSync", targetOwaUri.Host);
				}
			}
			return result;
		}

		private const uint Migration451RedirectFaultInjectionLid = 3414568253U;

		private const string ActvieSyncServerUrl = "https://{0}/Microsoft-Server-ActiveSync";

		private const string CorrectO365SNSUrl = "outlook.office365.com";

		private const string LegacyO365OWAHost = "outlook.com";

		private BudgetKey budgetKey;

		private IAirSyncContext context;

		private ExchangePrincipal exchangePrincipal;

		private bool exchangePrincipalInitialized;

		private WindowsPrincipal windowsPrincipal;

		private ActiveSyncMiniRecipient activeDirectoryUser;

		private bool mailboxIsOnE12Server;

		private ClientSecurityContextWrapper clientSecurityContextWrapper;

		private Guid deviceBehaviorCacheGuid;

		private string username;
	}
}
