using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal class PushNotificationServiceBudgetKey : LookupBudgetKey
	{
		internal PushNotificationServiceBudgetKey(ADObjectId policyId) : this(policyId, ThrottlingPolicyCache.Singleton)
		{
		}

		internal PushNotificationServiceBudgetKey(ADObjectId policyId, ThrottlingPolicyCache throttlingPolicyCache) : base(BudgetType.PushNotificationTenant, false)
		{
			ArgumentValidator.ThrowIfNull("policyId", policyId);
			ArgumentValidator.ThrowIfNull("throttlingPolicyCache", throttlingPolicyCache);
			this.policyId = policyId;
			this.toString = PushNotificationServiceBudgetKey.ToString(policyId);
			this.getHashCode = this.toString.GetHashCode();
			this.throttlingPolicyCache = throttlingPolicyCache;
		}

		public override bool Equals(object obj)
		{
			PushNotificationServiceBudgetKey pushNotificationServiceBudgetKey = obj as PushNotificationServiceBudgetKey;
			return !(pushNotificationServiceBudgetKey == null) && (object.ReferenceEquals(pushNotificationServiceBudgetKey, this) || ADObjectId.Equals(this.policyId, pushNotificationServiceBudgetKey.policyId));
		}

		public override string ToString()
		{
			return this.toString;
		}

		public override int GetHashCode()
		{
			return this.getHashCode;
		}

		internal static ADObjectId ResolveServiceThrottlingPolicyId()
		{
			IConfigurationSession session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 119, "ResolveServiceThrottlingPolicyId", "f:\\15.00.1497\\sources\\dev\\PushNotifications\\src\\server\\Services\\PushNotificationServiceBudgetKey.cs");
			IConfigurable[] policies = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				policies = session.Find<ThrottlingPolicy>(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "PushNotificationServiceThrottlingPolicy"), null, true, null);
			});
			if (!adoperationResult.Succeeded || policies == null || policies.Length < 1)
			{
				string text = adoperationResult.Exception.ToTraceString();
				PushNotificationsCrimsonEvents.CannotResolvePushNotificationServicePolicy.Log<string, string>("PushNotificationServiceThrottlingPolicy", text);
				ExTraceGlobals.PushNotificationServiceTracer.TraceError(0L, string.Format("Failed to resolve the PushNotification Service policy '{0}' with error: {1}.", "PushNotificationServiceThrottlingPolicy", text));
				return null;
			}
			return (ADObjectId)policies[0].Identity;
		}

		internal override IThrottlingPolicy InternalLookup()
		{
			return base.ADRetryLookup(() => this.throttlingPolicyCache.Get(OrganizationId.ForestWideOrgId, this.policyId));
		}

		private static string ToString(ADObjectId policyId)
		{
			return string.Format("policyName~{0}", policyId.Name);
		}

		private readonly ThrottlingPolicyCache throttlingPolicyCache;

		private readonly ADObjectId policyId;

		private readonly string toString;

		private readonly int getHashCode;
	}
}
