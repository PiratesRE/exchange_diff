using System;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Management.Odata;

namespace Microsoft.Exchange.Configuration.Authorization
{
	public class PswsAuthorization : CustomAuthorization
	{
		public override WindowsIdentity AuthorizeUser(SenderInfo senderInfo, out UserQuota userQuota)
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[PswsAuthorization.AuthorizeUser] Enter.");
			WindowsIdentity current;
			try
			{
				if (this.IsBlockedPswsDirectInvocation())
				{
					AuthZLogger.SafeAppendGenericError("PswsAuthorization.AuthorizeUser", "UnAuthorized. Blocked Psws direct invocation", false);
					throw new InvalidOperationException(Strings.InvalidPswsDirectInvocationBlocked);
				}
				CultureInfo cultureInfo = null;
				if (PswsAuthZHelper.TryParseCultureInfo(HttpContext.Current.Request.Headers, out cultureInfo))
				{
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>((long)this.GetHashCode(), "[PswsAuthorization.AuthorizeUser] Set thread culture to be {0}", cultureInfo.Name);
					Thread.CurrentThread.CurrentCulture = cultureInfo;
					Thread.CurrentThread.CurrentUICulture = cultureInfo;
				}
				IThrottlingPolicy throttlingPolicy = null;
				PswsAuthZUserToken authZUserToken = null;
				AuthZLogHelper.ExecuteWSManPluginAPI("PswsAuthorization.AuthorizeUser", false, true, delegate()
				{
					UserToken userToken = HttpContext.Current.CurrentUserToken();
					authZUserToken = PswsAuthZHelper.GetAuthZPluginUserToken(userToken);
					if (authZUserToken != null)
					{
						throttlingPolicy = authZUserToken.GetThrottlingPolicy();
					}
				});
				ExAssert.RetailAssert(authZUserToken != null, "UnAuthorized. The user token is invalid (null).");
				ExAssert.RetailAssert(throttlingPolicy != null, "UnAuthorized. Unable to get the user quota.");
				PswsBudgetManager.Instance.HeartBeat(authZUserToken);
				AuthZLogger.SafeSetLogger(RpsAuthZMetadata.ServerActiveRunspaces, PswsBudgetManager.Instance.TotalActiveRunspaces);
				AuthZLogger.SafeSetLogger(RpsAuthZMetadata.ServerActiveUsers, PswsBudgetManager.Instance.TotalActiveUsers);
				AuthZLogger.SafeSetLogger(RpsAuthZMetadata.UserBudgetOnStart, PswsBudgetManager.Instance.GetWSManBudgetUsage(authZUserToken));
				userQuota = new UserQuota((int)(throttlingPolicy.PswsMaxConcurrency.IsUnlimited ? 2147483647U : throttlingPolicy.PswsMaxConcurrency.Value), (int)(throttlingPolicy.PswsMaxRequest.IsUnlimited ? 2147483647U : throttlingPolicy.PswsMaxRequest.Value), (int)(throttlingPolicy.PswsMaxRequestTimePeriod.IsUnlimited ? 2147483647U : throttlingPolicy.PswsMaxRequestTimePeriod.Value));
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<Unlimited<uint>, Unlimited<uint>, Unlimited<uint>>((long)this.GetHashCode(), "[PswsAuthorization.AuthorizeUser] User quota: PswsMaxConcurrenty={0}, PswsMaxRequest={1}, PswsMaxRequestTimePeriod={2}.", throttlingPolicy.PswsMaxConcurrency, throttlingPolicy.PswsMaxRequest, throttlingPolicy.PswsMaxRequestTimePeriod);
				AuthZLogger.SafeSetLogger(RpsAuthZMetadata.IsAuthorized, true);
				AuthZLogger.SafeSetLogger(RpsAuthZMetadata.AuthorizeUser, authZUserToken.UserNameForLogging);
				AuthZLogger.SafeSetLogger(RpsAuthZMetadata.GetQuota, string.Format("PswsMaxConcurrenty={0};PswsMaxRequest={1};PswsMaxRequestTimePeriod={2}", throttlingPolicy.PswsMaxConcurrency, throttlingPolicy.PswsMaxRequest, throttlingPolicy.PswsMaxRequestTimePeriod));
				string ruleName = null;
				if (this.ConnectionBlockedByClientAccessRules(authZUserToken, out ruleName))
				{
					throw new ClientAccessRulesBlockedConnectionException(ruleName);
				}
				current = WindowsIdentity.GetCurrent();
			}
			catch (Exception ex)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceError<Exception>((long)this.GetHashCode(), "[PswsAuthorization.AuthorizeUser] Exception: {0}", ex);
				AuthZLogger.SafeAppendGenericError("PswsAuthorization.AuthorizeUser", ex, new Func<Exception, bool>(KnownException.IsUnhandledException));
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_PswsPublicAPIFailed, null, new object[]
				{
					"PswsAuthorization.AuthorizeUser",
					ex.ToString()
				});
				AuthZLogger.SafeSetLogger(RpsAuthZMetadata.IsAuthorized, false);
				PswsErrorHandling.SendErrorToClient((ex is ClientAccessRulesBlockedConnectionException) ? PswsErrorCode.ClientAccessRuleBlock : PswsErrorCode.AuthZUserError, ex, null);
				throw;
			}
			finally
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[PswsAuthorization.AuthorizeUser] Exit.");
			}
			return current;
		}

		public override string GetMembershipId(SenderInfo senderInfo)
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[PswsAuthorization.GetMembershipId] Enter.");
			string result;
			try
			{
				string name = senderInfo.Principal.Identity.Name;
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string>((long)this.GetHashCode(), "[PswsAuthorization.GetMembershipId] membershipId = \"{0}\".", name);
				result = name;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceError<Exception>((long)this.GetHashCode(), "[PswsAuthorization.GetMembershipId] Exception: {0}", ex);
				AuthZLogger.SafeAppendGenericError("PswsAuthorization.GetMembershipId", ex, new Func<Exception, bool>(KnownException.IsUnhandledException));
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_PswsPublicAPIFailed, null, new object[]
				{
					"PswsAuthorization.GetMembershipId",
					ex.ToString()
				});
				PswsErrorHandling.SendErrorToClient(PswsErrorCode.MemberShipIdError, ex, null);
				throw;
			}
			finally
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceDebug((long)this.GetHashCode(), "[PswsAuthorization.GetMembershipId] Exit.");
			}
			return result;
		}

		private bool ConnectionBlockedByClientAccessRules(PswsAuthZUserToken userToken, out string blockingRuleName)
		{
			blockingRuleName = null;
			if (userToken.OrgId != null && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.PswsClientAccessRulesEnabled.Enabled)
			{
				string blockRuleName = null;
				bool result = ClientAccessRulesUtils.ShouldBlockConnection(userToken.OrgId, ClientAccessRulesUtils.GetUsernameFromADRawEntry(userToken.UserEntry), ClientAccessProtocol.PowerShellWebServices, ClientAccessRulesUtils.GetRemoteEndPointFromContext(HttpContext.Current), ClientAccessAuthenticationMethod.BasicAuthentication, userToken.UserEntry, delegate(ClientAccessRulesEvaluationContext context)
				{
					blockRuleName = context.CurrentRule.Name;
					AuthZLogger.SafeAppendGenericError(ClientAccessRulesConstants.ClientAccessRuleName, context.CurrentRule.Name, false);
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, string>((long)this.GetHashCode(), "[PswsAuthorization.AuthorizeUser] Blocked by Client Access Rules ({0}={1})", ClientAccessRulesConstants.ClientAccessRuleName, context.CurrentRule.Name);
				}, delegate(double latency)
				{
					if (latency > 50.0)
					{
						AuthZLogger.SafeAppendGenericInfo(ClientAccessRulesConstants.ClientAccessRulesLatency, latency.ToString());
						ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, string>((long)this.GetHashCode(), "[PswsAuthorization.AuthorizeUser] Client Access Rules latency logger ({0}={1})", ClientAccessRulesConstants.ClientAccessRulesLatency, latency.ToString());
					}
				});
				blockingRuleName = blockRuleName;
				return result;
			}
			return false;
		}

		private bool IsPswsDirectInvocation()
		{
			return HttpContext.Current.Request.Path.IndexOf("psws/service.svc/commandinvocations", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private bool IsBlockedPswsDirectInvocation()
		{
			return this.IsPswsDirectInvocation() && !ExchangeRunspaceConfigurationSettings.IsCalledFromProxy(HttpContext.Current.Request.Headers);
		}
	}
}
