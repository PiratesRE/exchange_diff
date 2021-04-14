using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.PartnerToken;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[MessageContract]
	public class GetUserSettingsRequestMessage : AutodiscoverRequestMessage
	{
		internal override AutodiscoverResponseMessage Execute()
		{
			GetUserSettingsResponseMessage getUserSettingsResponseMessage = new GetUserSettingsResponseMessage();
			GetUserSettingsResponse response = getUserSettingsResponseMessage.Response;
			IPrincipal user = HttpContext.Current.User;
			string errorMessage;
			ExchangeVersion exchangeVersion;
			if (!AutodiscoverRequestMessage.ValidateRequest<User>(this.Request, this.Request.Users, this.Request.RequestedSettings, this.Request.RequestedVersion, GetUserSettingsRequestMessage.maxUsersPerGetUserSettingsRequest, Strings.MaxUsersPerGetUserSettingsRequestExceeded, out errorMessage, out exchangeVersion))
			{
				response.ErrorCode = ErrorCode.InvalidRequest;
				response.ErrorMessage = errorMessage;
			}
			else
			{
				HashSet<UserConfigurationSettingName> hashSet;
				UserSettingErrorCollection settingErrors;
				this.TryParseRequestUserSettings(out hashSet, out settingErrors);
				if (hashSet.Count == 0)
				{
					response.ErrorCode = ErrorCode.InvalidRequest;
					response.ErrorMessage = Strings.NoSettingsToReturn;
				}
				else
				{
					ExchangeServerVersion? requestedVersion = null;
					if (this.Request.RequestedVersion != null)
					{
						switch (this.Request.RequestedVersion.Value)
						{
						case ExchangeVersion.Exchange2010:
							requestedVersion = new ExchangeServerVersion?(ExchangeServerVersion.E14);
							break;
						case ExchangeVersion.Exchange2010_SP1:
							requestedVersion = new ExchangeServerVersion?(ExchangeServerVersion.E14_SP1);
							break;
						case ExchangeVersion.Exchange2010_SP2:
							requestedVersion = new ExchangeServerVersion?(ExchangeServerVersion.E14_SP2);
							break;
						case ExchangeVersion.Exchange2012:
						case ExchangeVersion.Exchange2013:
						case ExchangeVersion.Exchange2013_SP1:
							requestedVersion = new ExchangeServerVersion?(ExchangeServerVersion.E15);
							break;
						}
					}
					CallContext callContext = new CallContext(HttpContext.Current, this.Request.Users, hashSet, requestedVersion, settingErrors, response);
					try
					{
						this.ExecuteGetUserSettingsCommand(user, callContext);
					}
					catch (OverBudgetException arg)
					{
						ExTraceGlobals.FrameworkTracer.TraceError<OverBudgetException>(0L, "GetUserSettingsRequestMessage.Execute()returning ServerBusy for exception: {0}.", arg);
						response.ErrorCode = ErrorCode.ServerBusy;
						response.ErrorMessage = Strings.ServerBusy;
						response.UserResponses.Clear();
					}
					catch (LocalizedException ex)
					{
						ExTraceGlobals.FrameworkTracer.TraceError<LocalizedException>(0L, "GetUserSettingsRequestMessage.Execute()returning InternalServerError for exception: {0}.", ex);
						Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrWebException, Common.PeriodicKey, new object[]
						{
							ex.Message,
							ex
						});
						response.ErrorCode = ErrorCode.InternalServerError;
						response.ErrorMessage = Strings.InternalServerError;
						response.UserResponses.Clear();
					}
				}
			}
			return getUserSettingsResponseMessage;
		}

		private void ExecuteGetUserSettingsCommand(IPrincipal callingPrincipal, CallContext callContext)
		{
			IOrganizationScopedIdentity organizationScopedIdentity = callingPrincipal.Identity as IOrganizationScopedIdentity;
			if (organizationScopedIdentity != null)
			{
				this.ExecuteCommandForPartnerUser(organizationScopedIdentity, callContext);
				return;
			}
			WindowsIdentity windowsIdentity = callingPrincipal.Identity as WindowsIdentity;
			ClientSecurityContextIdentity clientSecurityContextIdentity = callingPrincipal.Identity as ClientSecurityContextIdentity;
			if (windowsIdentity != null || clientSecurityContextIdentity != null)
			{
				this.ExecuteCommand(callingPrincipal.Identity, callContext);
				return;
			}
			ExternalIdentity externalIdentity = callingPrincipal.Identity as ExternalIdentity;
			if (externalIdentity != null)
			{
				this.ExecuteCommandForExternalUser(externalIdentity, callContext);
				return;
			}
			callContext.Response.ErrorCode = ErrorCode.InvalidRequest;
			callContext.Response.ErrorMessage = Strings.InvalidRequest;
		}

		private void ExecuteCommand(IIdentity callerIdentity, CallContext callContext)
		{
			ADRecipient adrecipient = HttpContext.Current.Items["CallerRecipient"] as ADRecipient;
			if (adrecipient == null && (Common.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.NoCrossForestDiscover.Enabled))
			{
				string identityNameForTrace = Common.GetIdentityNameForTrace(callerIdentity);
				ExTraceGlobals.FrameworkTracer.TraceError<string>(0L, "ExecuteCommand -- IRecipientSession.FindBySid user for {0} returned null.", identityNameForTrace);
				callContext.Response.ErrorCode = ErrorCode.InvalidUser;
				callContext.Response.ErrorMessage = string.Format(Strings.InvalidUser, identityNameForTrace);
				return;
			}
			PartnerInfo partnerInfo = null;
			string targetTenant = null;
			bool flag = false;
			string text = null;
			byte[] binarySecret = null;
			GetUserSettingsCommandBase getUserSettingsCommandBase;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.ParseBinarySecretHeader.Enabled && AutodiscoverRequestMessage.HasBinarySecretHeader(out text))
			{
				PerformanceCounters.UpdatePartnerTokenRequests(callContext.UserAgent);
				bool flag2 = false;
				if (!string.IsNullOrEmpty(text))
				{
					try
					{
						binarySecret = Convert.FromBase64String(text);
						flag2 = true;
					}
					catch (FormatException)
					{
						ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "[GetUserSettingsRequestMessage::ExecuteCommand] the binary secret header {0} has invalid format", text);
					}
				}
				if (!flag2)
				{
					PerformanceCounters.UpdatePartnerTokenRequestsFailed(callContext.UserAgent);
					callContext.Response.ErrorCode = ErrorCode.InvalidRequest;
					callContext.Response.ErrorMessage = Strings.InvalidBinarySecretHeader;
					return;
				}
				if (!this.TryGetMailboxAccessPartnerInfo(callContext, adrecipient, out partnerInfo, out targetTenant))
				{
					ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "[GetUserSettingsRequestMessage::ExecuteCommand] TryGetMailboxAccessPartnerInfo returns false.");
					PerformanceCounters.UpdatePartnerTokenRequestsFailed(callContext.UserAgent);
					callContext.Response.ErrorCode = ErrorCode.InvalidRequest;
					callContext.Response.ErrorMessage = Strings.InvalidPartnerTokenRequest;
					return;
				}
				flag = true;
				ExTraceGlobals.FrameworkTracer.TraceDebug<ADRecipient>((long)this.GetHashCode(), "GetUserSettingsForPartner created for the '{0}'. The caller is FPO partner.", adrecipient);
				getUserSettingsCommandBase = new GetUserSettingsForPartner(callerIdentity.GetSecurityIdentifier(), callContext);
			}
			else if (adrecipient == null)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "GetUserSettingsForUserWithUnscopedCaller created for '{0}'.", Common.GetIdentityNameForTrace(callerIdentity));
				getUserSettingsCommandBase = new GetUserSettingsForUserWithUnscopedCaller(callerIdentity.GetSecurityIdentifier(), callContext);
			}
			else if (ExchangeRunspaceConfiguration.IsAllowedOrganizationForPartnerAccounts(adrecipient.OrganizationId))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<ADRecipient>((long)this.GetHashCode(), "GetUserSettingsForPartner created for '{0}'.", adrecipient);
				getUserSettingsCommandBase = new GetUserSettingsForPartner(callerIdentity.GetSecurityIdentifier(), callContext);
			}
			else
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<ADRecipient>((long)this.GetHashCode(), "GetUserSettingsForUser created for '{0}'.", adrecipient);
				getUserSettingsCommandBase = new GetUserSettingsForUser(adrecipient, callerIdentity.GetSecurityIdentifier(), callContext);
			}
			getUserSettingsCommandBase.Execute();
			if (flag)
			{
				UserResponse userResponse = callContext.Response.UserResponses[0];
				if (userResponse.ErrorCode == ErrorCode.NoError || userResponse.ErrorCode == ErrorCode.RedirectUrl)
				{
					string assertionId = SecurityTokenService.CreateSamlAssertionId();
					MessageHeader header = MessageHeader.CreateHeader("PartnerToken", "http://schemas.microsoft.com/exchange/2010/Autodiscover", partnerInfo.CreateSamlToken(assertionId, targetTenant, binarySecret, GetUserSettingsRequestMessage.tokenLifetime.Value));
					MessageHeader header2 = MessageHeader.CreateHeader("PartnerTokenReference", "http://schemas.microsoft.com/exchange/2010/Autodiscover", PartnerInfo.GetTokenReference(assertionId));
					OperationContext.Current.OutgoingMessageHeaders.Add(header);
					OperationContext.Current.OutgoingMessageHeaders.Add(header2);
					return;
				}
				PerformanceCounters.UpdatePartnerTokenRequestsFailed(callContext.UserAgent);
				ExTraceGlobals.FrameworkTracer.TraceDebug<ErrorCode, string>((long)this.GetHashCode(), "No partner token header added since the user response error code is {0}, error message is '{1}'", userResponse.ErrorCode, userResponse.ErrorMessage);
			}
		}

		private bool TryGetMailboxAccessPartnerInfo(CallContext callContext, ADRecipient callerAdRecipient, out PartnerInfo partnerInfo, out string targetTenant)
		{
			partnerInfo = null;
			targetTenant = null;
			if (!Common.IsMultiTenancyEnabled)
			{
				return false;
			}
			if (callContext.Users.Count != 1)
			{
				return false;
			}
			User user = callContext.Users[0];
			SmtpAddress smtpAddress = new SmtpAddress(user.Mailbox);
			if (!smtpAddress.IsValidAddress)
			{
				return false;
			}
			string name = callerAdRecipient.Name;
			SmtpDomain smtpDomain = new SmtpDomain(smtpAddress.Domain);
			OrganizationId organizationId = DomainToOrganizationIdCache.Singleton.Get(smtpDomain);
			if (organizationId != null && !ADAccountPartitionLocator.IsKnownPartition(organizationId.PartitionId))
			{
				organizationId = null;
			}
			OrganizationId organizationId2 = callerAdRecipient.OrganizationId;
			if (organizationId != null && organizationId.Equals(organizationId2))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string, OrganizationId>((long)this.GetHashCode(), "The caller {0} tries to query its' own org {1}", name, organizationId2);
				return false;
			}
			partnerInfo = MailboxAccessPartnerInfoCache.Singleton.Get(new MailboxAccessPartnerInfoCacheKey(callerAdRecipient.Id, organizationId2));
			if (partnerInfo == PartnerInfo.Invalid)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "The caller {0} could not be acted as mailbox access partner.", name);
				return false;
			}
			if (!partnerInfo.HasPartnerRelationship(organizationId))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string, OrganizationId>((long)this.GetHashCode(), "The caller {0} has no partner relationship with organization {1}", name, organizationId);
				return false;
			}
			targetTenant = smtpDomain.Domain;
			return true;
		}

		private void ExecuteCommandForExternalUser(ExternalIdentity callerExternalIdentity, CallContext callContext)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "GetUserSettingsForExternalUser created for '{0}'.", callerExternalIdentity.Name);
			GetUserSettingsCommandBase getUserSettingsCommandBase = new GetUserSettingsForExternalUser(callerExternalIdentity, callContext);
			getUserSettingsCommandBase.Execute();
		}

		private void ExecuteCommandForPartnerUser(IOrganizationScopedIdentity callerIdentity, CallContext callContext)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<IOrganizationScopedIdentity>((long)this.GetHashCode(), "ExecuteCommandForPartnerUser created for '{0}'.", callerIdentity);
			GetUserSettingsCommandBase getUserSettingsCommandBase = new GetUserSettingsForPartnerUser(callerIdentity, callContext);
			getUserSettingsCommandBase.Execute();
		}

		private bool TryParseRequestUserSettings(out HashSet<UserConfigurationSettingName> settingNames, out UserSettingErrorCollection userSettingErrors)
		{
			settingNames = new HashSet<UserConfigurationSettingName>();
			userSettingErrors = new UserSettingErrorCollection();
			foreach (string text in this.Request.RequestedSettings)
			{
				try
				{
					UserConfigurationSettingName item = (UserConfigurationSettingName)Enum.Parse(typeof(UserConfigurationSettingName), text);
					if (AutodiscoverRequestMessage.RestrictedSettings.Member.Contains(item))
					{
						UserSettingError userSettingError = new UserSettingError();
						userSettingError.SettingName = text;
						userSettingError.ErrorCode = ErrorCode.SettingIsNotAvailable;
						userSettingError.ErrorMessage = string.Format(Strings.SettingIsNotAvailable, text);
						userSettingErrors.Add(userSettingError);
					}
					else
					{
						settingNames.Add(item);
					}
				}
				catch (ArgumentException)
				{
					UserSettingError userSettingError2 = new UserSettingError();
					userSettingError2.SettingName = text;
					userSettingError2.ErrorCode = ErrorCode.InvalidSetting;
					userSettingError2.ErrorMessage = string.Format(Strings.InvalidUserSetting, text);
					userSettingErrors.Add(userSettingError2);
				}
			}
			return userSettingErrors.Count == 0;
		}

		private const int DefaultMaxUsersPerRequest = 90;

		private static readonly TimeSpanAppSettingsEntry tokenLifetime = new TimeSpanAppSettingsEntry("PartnerTokenLifetime", TimeSpanUnit.Minutes, TimeSpan.FromHours(8.0), ExTraceGlobals.FrameworkTracer);

		private static LazyMember<int> maxUsersPerGetUserSettingsRequest = new LazyMember<int>(delegate()
		{
			int result;
			if (int.TryParse(ConfigurationManager.AppSettings["MaxUsersPerGetUserSettingsRequest"], out result))
			{
				return result;
			}
			return 90;
		});

		[MessageBodyMember(Name = "Request", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
		public GetUserSettingsRequest Request;
	}
}
