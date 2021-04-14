using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	[Serializable]
	internal class OrganizationValidationRule : ValidationRule
	{
		public OrganizationValidationRule(OrganizationValidationRuleDefinition ruleDefinition, RoleEntry roleEntry) : base(ruleDefinition, new List<CapabilityIdentifierEvaluator>(), new List<CapabilityIdentifierEvaluator>(), roleEntry)
		{
		}

		protected override bool InternalTryValidate(ADRawEntry adObject, out RuleValidationException validationException)
		{
			validationException = null;
			OrganizationValidationRuleDefinition organizationValidationRuleDefinition = base.RuleDefinition as OrganizationValidationRuleDefinition;
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "OrganizationValidationRule.InternalTryValidate('{0}') return '{1}'. - not datacenter mode.", adObject.GetDistinguishedNameOrName(), true);
				return true;
			}
			OrganizationId organizationId = (OrganizationId)adObject[ADObjectSchema.OrganizationId];
			ADSessionSettings sessionSettings = OrganizationId.ForestWideOrgId.Equals(organizationId) ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
			IConfigurationSession session = DirectorySessionFactory.Default.CreateTenantConfigurationSession(adObject.OriginatingServer, true, ConsistencyMode.IgnoreInvalid, sessionSettings, 377, "InternalTryValidate", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ValidationRules\\ValidationRule.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnit = ProvisioningCache.Instance.TryAddAndGetOrganizationData<ExchangeConfigurationUnit>(CannedProvisioningCacheKeys.OrganizationCUContainer, organizationId, () => session.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit));
			if (exchangeConfigurationUnit == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "OrganizationValidationRule.InternalTryValidate('{0}') return '{1}'. - organization (ExchangeConfigurationUnit) object is not found.", adObject.GetDistinguishedNameOrName(), true);
				return true;
			}
			foreach (ValidationRuleExpression validationRuleExpression in organizationValidationRuleDefinition.OverridingAllowExpressions)
			{
				bool flag = true;
				foreach (PropertyDefinition propertyDefinition in validationRuleExpression.QueryFilter.FilterProperties())
				{
					if (!exchangeConfigurationUnit.propertyBag.Contains((ProviderPropertyDefinition)propertyDefinition))
					{
						ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ExpressionFilterValidationRule.InternalTryValidate({0}). Missing Property {1}.", exchangeConfigurationUnit.GetDistinguishedNameOrName(), propertyDefinition.Name);
						flag = false;
						break;
					}
				}
				if (flag && OpathFilterEvaluator.FilterMatches(validationRuleExpression.QueryFilter, exchangeConfigurationUnit))
				{
					ExTraceGlobals.AccessCheckTracer.TraceDebug<string, bool, string>((long)this.GetHashCode(), "OrganizationValidationRule.InternalTryValidate('{0}') return '{1}'. - matched filter: {2}.", adObject.GetDistinguishedNameOrName(), true, validationRuleExpression.QueryString);
					return true;
				}
			}
			foreach (ValidationRuleExpression validationRuleExpression2 in organizationValidationRuleDefinition.RestrictionExpressions)
			{
				bool flag2 = true;
				foreach (PropertyDefinition propertyDefinition2 in validationRuleExpression2.QueryFilter.FilterProperties())
				{
					if (!exchangeConfigurationUnit.propertyBag.Contains((ProviderPropertyDefinition)propertyDefinition2))
					{
						ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ExpressionFilterValidationRule.InternalTryValidate({0}). Missing Property {1}.", exchangeConfigurationUnit.GetDistinguishedNameOrName(), propertyDefinition2.Name);
						flag2 = false;
						break;
					}
				}
				if (flag2 && OpathFilterEvaluator.FilterMatches(validationRuleExpression2.QueryFilter, exchangeConfigurationUnit))
				{
					validationException = new RuleValidationException(base.GetValidationRuleErrorMessage(adObject, validationRuleExpression2.QueryString));
					return false;
				}
			}
			return true;
		}
	}
}
