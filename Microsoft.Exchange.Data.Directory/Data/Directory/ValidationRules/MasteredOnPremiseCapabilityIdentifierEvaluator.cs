using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class MasteredOnPremiseCapabilityIdentifierEvaluator : TenantScopedPropertyCapabilityEvaluator
	{
		public MasteredOnPremiseCapabilityIdentifierEvaluator(Capability capability) : base(capability)
		{
		}

		public override CapabilityEvaluationResult Evaluate(ADRawEntry adObject)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Entering MasteredOnPremiseCapabilityIdentifierEvaluator.Evaluate('{0}') CapabilityToCheck '{1}'.", adObject.GetDistinguishedNameOrName(), base.Capability.ToString());
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "MasteredOnPremiseCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}' - not datacenter mode.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.NotApplicable.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.NotApplicable;
			}
			ADRecipient adrecipient = adObject as ADRecipient;
			ReducedRecipient reducedRecipient = adObject as ReducedRecipient;
			if (adrecipient == null && reducedRecipient == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "MasteredOnPremiseCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}' - adObject in not ADRecipient or ReducedRecipient.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.NotApplicable.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.NotApplicable;
			}
			if ((adrecipient == null || !adrecipient.IsDirSyncEnabled) && (reducedRecipient == null || !reducedRecipient.IsDirSyncEnabled))
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "MasteredOnPremiseCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}' - recipient is not Dirsynced.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.No.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.No;
			}
			OrganizationId organizationId = (adrecipient != null) ? adrecipient.OrganizationId : reducedRecipient.OrganizationId;
			if (organizationId == null || OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "MasteredOnPremiseCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}' - recipient does not belong to tenant scope.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.NotApplicable.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.NotApplicable;
			}
			ExchangeConfigurationUnit exchangeConfigurationUnit = this.GetExchangeConfigurationUnit(organizationId);
			if (exchangeConfigurationUnit == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "MasteredOnPremiseCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}' - recipient is not in tenant scope.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.NotApplicable.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.NotApplicable;
			}
			CapabilityEvaluationResult capabilityEvaluationResult = exchangeConfigurationUnit.IsDirSyncEnabled ? CapabilityEvaluationResult.Yes : CapabilityEvaluationResult.No;
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "MasteredOnPremiseCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}'", adObject.GetDistinguishedNameOrName(), capabilityEvaluationResult.ToString(), base.Capability.ToString());
			return capabilityEvaluationResult;
		}

		public override bool TryGetFilter(OrganizationId organizationId, out QueryFilter queryFilter, out LocalizedString errorMessage)
		{
			if (null == organizationId)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				queryFilter = null;
				errorMessage = DirectoryStrings.MasteredOnPremiseCapabilityUndefinedNotTenant(base.Capability.ToString());
				return false;
			}
			ExchangeConfigurationUnit exchangeConfigurationUnit = this.GetExchangeConfigurationUnit(organizationId);
			if (exchangeConfigurationUnit == null)
			{
				queryFilter = null;
				errorMessage = DirectoryStrings.MasteredOnPremiseCapabilityUndefinedNotTenant(base.Capability.ToString());
				return false;
			}
			if (!exchangeConfigurationUnit.IsDirSyncEnabled)
			{
				queryFilter = null;
				errorMessage = DirectoryStrings.MasteredOnPremiseCapabilityUndefinedTenantNotDirSyncing(base.Capability.ToString(), OrganizationSchema.IsDirSyncRunning.Name);
				return false;
			}
			queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.IsDirSynced, true);
			errorMessage = LocalizedString.Empty;
			return true;
		}

		private ExchangeConfigurationUnit GetExchangeConfigurationUnit(OrganizationId organizationId)
		{
			IConfigurationSession tenantScopedSystemConfigurationSession = base.GetTenantScopedSystemConfigurationSession(organizationId);
			return tenantScopedSystemConfigurationSession.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit);
		}
	}
}
