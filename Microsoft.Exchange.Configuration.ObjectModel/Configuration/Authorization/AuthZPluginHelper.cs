using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.FailFast;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal static class AuthZPluginHelper
	{
		internal static LocalizedString HandleUserOverBudgetException(OverBudgetException exception, AuthZPluginUserToken userToken)
		{
			string policyPart = exception.PolicyPart;
			string userName = userToken.UserName;
			string windowsLiveId = userToken.WindowsLiveId;
			ExTraceGlobals.PublicPluginAPITracer.TraceError<string, string>(0L, "Get User OverBudgetException for user {0}. Message: {1}", userName, exception.ToString());
			AuthZLogger.SafeAppendGenericError("User_OverBudgetException", exception.ToString(), false);
			if (!string.IsNullOrEmpty(windowsLiveId))
			{
				BlockedType blockedType = (policyPart == "PowerShellMaxCmdlets") ? BlockedType.NewRequest : BlockedType.NewSession;
				FailFastUserCache.Instance.AddUserToCache(userToken.WindowsLiveId, blockedType, TimeSpan.Zero);
				AuthZLogger.SafeAppendColumn(RpsCommonMetadata.ContributeToFailFast, "AuthZ-UserSelf", LoggerHelper.GetContributeToFailFastValue("User", userToken.WindowsLiveId, blockedType.ToString(), -1.0));
			}
			IThrottlingPolicy throttlingPolicy = userToken.GetThrottlingPolicy();
			LocalizedString value;
			if (policyPart == "MaxConcurrency")
			{
				value = Strings.ErrorMaxRunspacesLimit(throttlingPolicy.PowerShellMaxConcurrency.ToString(), policyPart);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_ReachedMaxUserPSConnectionLimit, null, new object[]
				{
					userName,
					throttlingPolicy.PowerShellMaxConcurrency
				});
			}
			else if (policyPart == "MaxRunspacesTimePeriod")
			{
				value = Strings.ErrorMaxRunspacesTarpitting(exception.BackoffTime / 1000);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_ReachedMaxPSRunspaceInTimePeriodLimit, null, new object[]
				{
					userName,
					throttlingPolicy.PowerShellMaxRunspaces,
					throttlingPolicy.PowerShellMaxRunspacesTimePeriod,
					exception.BackoffTime
				});
			}
			else
			{
				if (!(policyPart == "PowerShellMaxCmdlets"))
				{
					throw new NotSupportedException(string.Format("DEV bug. The exception policy part {0} is not expected.", policyPart));
				}
				value = Strings.ErrorOperationTarpitting(exception.BackoffTime / 1000);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_ReachedMaxPowershellCmdletLimit, null, new object[]
				{
					userName,
					throttlingPolicy.PowerShellMaxCmdlets,
					throttlingPolicy.PowerShellMaxCmdletsTimePeriod,
					exception.BackoffTime
				});
			}
			return new LocalizedString(value + string.Format("{2}Policy: {0}; {2}Snapshot: {1}", exception.ThrottlingPolicyDN, exception.Snapshot, Environment.NewLine));
		}

		internal static LocalizedString HandleTenantOverBudgetException(OverBudgetException exception, AuthZPluginUserToken userToken)
		{
			string policyPart = exception.PolicyPart;
			string orgIdInString = userToken.OrgIdInString;
			string userName = userToken.UserName;
			string windowsLiveId = userToken.WindowsLiveId;
			ExTraceGlobals.PublicPluginAPITracer.TraceError<string, string, string>(0L, "Get Tenant OverBudgetException for user {0}, Organization {1}. Message: {2}", userName, orgIdInString, exception.ToString());
			AuthZLogger.SafeAppendGenericError("Tenant_OverBudgetException", exception.ToString(), false);
			TimeSpan blockedTime = TimeSpan.FromMilliseconds((double)exception.BackoffTime);
			if (windowsLiveId != null)
			{
				FailFastUserCache.Instance.AddUserToCache(windowsLiveId, BlockedType.NewSession, blockedTime);
				AuthZLogger.SafeAppendColumn(RpsCommonMetadata.ContributeToFailFast, "AuthZ-UserOrg", LoggerHelper.GetContributeToFailFastValue("User", windowsLiveId, "NewSession", blockedTime.TotalMilliseconds));
			}
			if (!string.IsNullOrEmpty(orgIdInString))
			{
				FailFastUserCache.Instance.AddTenantToCache(orgIdInString, BlockedType.NewSession, blockedTime);
				AuthZLogger.SafeAppendColumn(RpsCommonMetadata.ContributeToFailFast, "AuthZ-Org", LoggerHelper.GetContributeToFailFastValue("Tenant", orgIdInString, "NewSession", blockedTime.TotalMilliseconds));
				foreach (string text in userToken.DomainsToBlockTogether)
				{
					FailFastUserCache.Instance.AddTenantToCache(text, BlockedType.NewSession, blockedTime);
					AuthZLogger.SafeAppendColumn(RpsCommonMetadata.ContributeToFailFast, "AuthZ-AcceptedDomain-" + text, LoggerHelper.GetContributeToFailFastValue("Tenant", text, "NewSession", blockedTime.TotalMilliseconds));
				}
			}
			IThrottlingPolicy throttlingPolicy = userToken.GetThrottlingPolicy();
			LocalizedString value;
			if (policyPart == "MaxTenantConcurrency")
			{
				value = Strings.ErrorMaxTenantPSConnectionLimit(orgIdInString);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_ReachedMaxTenantPSConnectionLimit, null, new object[]
				{
					userName,
					orgIdInString,
					throttlingPolicy.PowerShellMaxTenantConcurrency
				});
			}
			else
			{
				if (!(policyPart == "MaxTenantRunspaces"))
				{
					throw new NotSupportedException(string.Format("DEV bug. The exception policy part {0} is not expected.", policyPart));
				}
				value = Strings.ErrorTenantMaxRunspacesTarpitting(orgIdInString, exception.BackoffTime / 1000);
				TaskLogger.LogRbacEvent(TaskEventLogConstants.Tuple_ReachedMaxTenantPSRunspaceInTimePeriodLimit, null, new object[]
				{
					userName,
					orgIdInString,
					throttlingPolicy.PowerShellMaxTenantRunspaces,
					throttlingPolicy.PowerShellMaxRunspacesTimePeriod,
					exception.BackoffTime
				});
			}
			return new LocalizedString(value + string.Format("{2}Policy: {0}; {2}Snapshot: {1}", exception.ThrottlingPolicyDN, exception.Snapshot, Environment.NewLine));
		}

		internal static void TriggerFailFastForAuthZFailure(string windowsLiveId)
		{
			if (string.IsNullOrEmpty(windowsLiveId))
			{
				return;
			}
			FailFastUserCache.Instance.AddUserToCache(windowsLiveId, BlockedType.NewSession, TimeSpan.Zero);
			AuthZLogger.SafeAppendColumn(RpsCommonMetadata.ContributeToFailFast, "AuthZ-UserSelf", LoggerHelper.GetContributeToFailFastValue("User", windowsLiveId, "NewSession", -1.0));
		}

		internal static void DisposeCostHandleAndSetToNull(ref CostHandle costHandle)
		{
			if (costHandle != null)
			{
				costHandle.Dispose();
				costHandle = null;
			}
		}

		internal static IEnumerable<SmtpDomainWithSubdomains> GetAcceptedDomains(OrganizationId organizationId, OrganizationId executingOrganizationId)
		{
			ProvisioningCache provisioningCache = ProvisioningCache.Instance;
			return provisioningCache.TryAddAndGetOrganizationData<IEnumerable<SmtpDomainWithSubdomains>>(CannedProvisioningCacheKeys.OrganizationAcceptedDomains, organizationId, delegate()
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, executingOrganizationId, false);
				IConfigurationSession cfgSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 245, "GetAcceptedDomains", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\AuthZPluginHelper.cs");
				ADObjectId rootId = organizationId.ConfigurationUnit ?? provisioningCache.TryAddAndGetGlobalData<ADObjectId>(CannedProvisioningCacheKeys.FirstOrgContainerId, () => cfgSession.GetOrgContainerId());
				QueryFilter filter = new NotFilter(new BitMaskAndFilter(AcceptedDomainSchema.AcceptedDomainFlags, 1UL));
				ADPagedReader<AcceptedDomain> adpagedReader = cfgSession.FindPaged<AcceptedDomain>(rootId, QueryScope.SubTree, filter, null, 0);
				AcceptedDomain[] array = adpagedReader.ReadAllPages();
				SmtpDomainWithSubdomains[] array2 = new SmtpDomainWithSubdomains[array.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = array[i].DomainName;
				}
				return array2;
			});
		}

		internal static void UpdateAuthZPluginPerfCounters(BudgetManager budgetManager)
		{
			RemotePowershellPerformanceCountersInstance remotePowershellPerfCounter = ExchangeAuthorizationPlugin.RemotePowershellPerfCounter;
			if (remotePowershellPerfCounter != null)
			{
				remotePowershellPerfCounter.CurrentSessions.RawValue = (long)budgetManager.TotalActiveRunspaces;
				remotePowershellPerfCounter.CurrentUniqueUsers.RawValue = (long)budgetManager.TotalActiveUsers;
			}
		}

		internal static IIdentity ConstructGenericIdentityFromUserToken(UserToken userToken)
		{
			string text = userToken.UserSid.ToString();
			string partitionId = null;
			if (userToken.PartitionId != null)
			{
				partitionId = userToken.PartitionId.ToString();
			}
			string type = userToken.AuthenticationType.ToString();
			return new GenericSidIdentity(text, type, new SecurityIdentifier(text), partitionId);
		}

		internal static IIdentity ConstructAuthZUser(UserToken userToken, Microsoft.Exchange.Configuration.Core.AuthenticationType authenticationType)
		{
			SecurityIdentifier userSid = userToken.UserSid;
			string partitionId = null;
			if (userToken.PartitionId != null)
			{
				partitionId = userToken.PartitionId.ToString();
			}
			return new GenericSidIdentity(userSid.ToString(), authenticationType.ToString(), userSid, partitionId);
		}

		internal static bool IsFatalException(Exception e)
		{
			return e != null && (e is AccessViolationException || e is DataMisalignedException || e is TypeLoadException || e is TypeInitializationException || e is EntryPointNotFoundException || e is InsufficientMemoryException || e is OutOfMemoryException || e is BadImageFormatException || e is StackOverflowException || e is InvalidProgramException);
		}
	}
}
