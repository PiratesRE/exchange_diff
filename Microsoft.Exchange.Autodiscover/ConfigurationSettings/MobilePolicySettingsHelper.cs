using System;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	internal class MobilePolicySettingsHelper
	{
		internal static string GetPolicyDataForUser(ADUser user, IBudget budget)
		{
			PolicyData policyData = MobilePolicySettingsHelper.GetPolicyData(user, budget);
			if (policyData != null)
			{
				bool flag;
				return ProvisionCommandPhaseOne.BuildEASProvisionDoc(121, out flag, policyData);
			}
			ExTraceGlobals.FrameworkTracer.TraceDebug<string>(0L, "[MobilePolicySettingsHelper.GetPolicyDataForUser()] No explicit or default policy found for user {0}", user.Alias);
			return null;
		}

		private static PolicyData GetPolicyData(ADUser user, IBudget budget)
		{
			PolicyData policyData = null;
			if (user.ActiveSyncMailboxPolicy != null)
			{
				policyData = MobilePolicySettingsHelper.GetPolicySetting(user, budget);
			}
			if (policyData == null)
			{
				policyData = MobilePolicySettingsHelper.GetDefaultPolicySetting(user, budget);
			}
			return policyData;
		}

		private static PolicyData GetPolicySetting(ADUser user, IBudget budget)
		{
			PolicyData result = null;
			try
			{
				result = MobilePolicySettingsHelper.LoadPolicySetting(MobilePolicySettingsHelper.CreateScopedADSession(user, budget), user.ActiveSyncMailboxPolicy);
			}
			catch (ADTransientException arg)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string, ADTransientException>(0L, "MobilePolicySettingsHelper.GetPolicySetting -- AD lookup returned transient error for user \"{0}\": {1}", user.Alias, arg);
			}
			return result;
		}

		private static PolicyData LoadPolicySetting(IConfigurationSession scopedSession, ADObjectId policyId)
		{
			MobileMailboxPolicy mobileMailboxPolicy = scopedSession.Read<MobileMailboxPolicy>(policyId);
			if (mobileMailboxPolicy != null)
			{
				return MobilePolicySettingsHelper.CreatePolicyData(mobileMailboxPolicy);
			}
			return null;
		}

		private static PolicyData GetDefaultPolicySetting(ADUser user, IBudget budget)
		{
			PolicyData result = null;
			try
			{
				result = MobilePolicySettingsHelper.LoadDefaultPolicySetting(MobilePolicySettingsHelper.CreateScopedADSession(user, budget), user);
			}
			catch (ADTransientException arg)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string, ADTransientException>(0L, "MobilePolicySettingsHelper.GetDefaultPolicySetting -- AD lookup returned transient error for user \"{0}\": {1}", user.Alias, arg);
			}
			return result;
		}

		private static PolicyData LoadDefaultPolicySetting(IConfigurationSession scopedSession, ADUser user)
		{
			MobileMailboxPolicy[] array = scopedSession.Find<MobileMailboxPolicy>(scopedSession.GetOrgContainerId(), QueryScope.SubTree, MobilePolicySettingsHelper.mobileMailboxPolicyFilter, MobilePolicySettingsHelper.mobileMailboxPolicySortBy, 3);
			if (array != null && array.Length > 0)
			{
				return MobilePolicySettingsHelper.CreatePolicyData(array[0]);
			}
			ExTraceGlobals.FrameworkTracer.TraceDebug<OrganizationId>(0L, "[MobilePolicySettingsHelper.LoadPolicySetting()] No default policy found for organization {0}", user.OrganizationId);
			return null;
		}

		private static PolicyData CreatePolicyData(MobileMailboxPolicy mobileMaiboxPolicy)
		{
			return new PolicyData(mobileMaiboxPolicy, false);
		}

		private static IConfigurationSession CreateScopedADSession(ADUser user, IBudget budget)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(user.OrganizationId), 224, "CreateScopedADSession", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\MobilePolicySettingsHelper.cs");
			tenantOrTopologyConfigurationSession.SessionSettings.AccountingObject = budget;
			return tenantOrTopologyConfigurationSession;
		}

		private const bool IrmEnabledSetting = false;

		private const int AirSyncProtocolVersion = 121;

		private static readonly QueryFilter mobileMailboxPolicyFilter = new BitMaskAndFilter(MobileMailboxPolicySchema.MobileFlags, 4096UL);

		private static readonly SortBy mobileMailboxPolicySortBy = new SortBy(ADObjectSchema.WhenChanged, SortOrder.Descending);
	}
}
