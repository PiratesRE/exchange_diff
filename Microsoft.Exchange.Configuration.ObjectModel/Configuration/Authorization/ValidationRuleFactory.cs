using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ValidationRules;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal static class ValidationRuleFactory
	{
		internal static bool HasApplicableValidationRules(string cmdletFullName, ADRawEntry currentExecutingUser)
		{
			if (string.IsNullOrEmpty(cmdletFullName))
			{
				throw new ArgumentNullException("cmdletFullName");
			}
			if (currentExecutingUser == null)
			{
				throw new ArgumentNullException("currentExecutingUser");
			}
			return RBACValidationRulesList.Instance.HasApplicableValidationRules(cmdletFullName);
		}

		internal static IList<ValidationRule> GetApplicableValidationRules(string cmdletFullName, IList<string> parameters, ADRawEntry currentExecutingUser)
		{
			if (currentExecutingUser == null)
			{
				throw new ArgumentNullException("currentExecutingUser");
			}
			if (string.IsNullOrEmpty(cmdletFullName))
			{
				throw new ArgumentNullException("cmdletFullName");
			}
			return ValidationRuleFactory.GetApplicableValidationRules(cmdletFullName, parameters, ValidationRuleFactory.GetValidationRuleSkuForUser(currentExecutingUser));
		}

		internal static IList<ValidationRule> GetApplicableValidationRules(string cmdletFullName, IList<string> parameters, ValidationRuleSkus applicableSku)
		{
			IList<RoleEntryValidationRuleTuple> applicableRules = RBACValidationRulesList.Instance.GetApplicableRules(cmdletFullName, parameters, applicableSku);
			List<ValidationRule> list = new List<ValidationRule>(applicableRules.Count);
			foreach (RoleEntryValidationRuleTuple roleEntryValidationRuleTuple in applicableRules)
			{
				list.Add(ValidationRuleFactory.Create(roleEntryValidationRuleTuple.RuleDefinition, roleEntryValidationRuleTuple.MatchingRoleEntry));
			}
			return list;
		}

		internal static List<ValidationRule> GetValidationRulesByFeature(string feature)
		{
			if (string.IsNullOrEmpty(feature))
			{
				throw new ArgumentNullException("feature");
			}
			IList<ValidationRuleDefinition> applicableRules = RBACValidationRulesList.Instance.GetApplicableRules(feature);
			List<ValidationRule> list = new List<ValidationRule>(applicableRules.Count);
			foreach (ValidationRuleDefinition definition in applicableRules)
			{
				list.Add(ValidationRuleFactory.Create(definition, null));
			}
			return list;
		}

		private static ValidationRule Create(ValidationRuleDefinition definition, RoleEntry roleEntry)
		{
			ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string, string>((long)definition.GetHashCode(), "Entering ValidationRuleFactory.GetValidationRule({0}) - Creating ValidationRule. Name: '{1}'", (null == roleEntry) ? "<NULL>" : roleEntry.ToString(), definition.Name);
			OrganizationValidationRuleDefinition organizationValidationRuleDefinition = definition as OrganizationValidationRuleDefinition;
			if (organizationValidationRuleDefinition != null)
			{
				return new OrganizationValidationRule(organizationValidationRuleDefinition, roleEntry);
			}
			IEnumerable<Capability> restrictedCapabilities = definition.RestrictedCapabilities;
			List<CapabilityIdentifierEvaluator> list = new List<CapabilityIdentifierEvaluator>();
			foreach (Capability capability in restrictedCapabilities)
			{
				list.Add(CapabilityIdentifierEvaluatorFactory.Create(capability));
			}
			IEnumerable<Capability> overridingAllowCapabilities = definition.OverridingAllowCapabilities;
			List<CapabilityIdentifierEvaluator> list2 = new List<CapabilityIdentifierEvaluator>();
			foreach (Capability capability2 in overridingAllowCapabilities)
			{
				list2.Add(CapabilityIdentifierEvaluatorFactory.Create(capability2));
			}
			if (definition.Expressions == null)
			{
				return new RestrictedValidationRule(definition, list, list2, roleEntry);
			}
			return new ExpressionFilterValidationRule(definition, list, list2, roleEntry);
		}

		private static ValidationRuleSkus GetValidationRuleSkuForUser(ADRawEntry currentExecutingUser)
		{
			ValidationRuleSkus result = ValidationRuleSkus.None;
			bool flag = OrganizationId.ForestWideOrgId.Equals(currentExecutingUser[ADObjectSchema.OrganizationId]);
			switch (ValidationRuleFactory.ExchangeSku)
			{
			case Datacenter.ExchangeSku.Enterprise:
				if (flag)
				{
					result = ValidationRuleSkus.Enterprise;
				}
				break;
			case Datacenter.ExchangeSku.ExchangeDatacenter:
			case Datacenter.ExchangeSku.DatacenterDedicated:
				if (flag)
				{
					result = ValidationRuleSkus.Datacenter;
				}
				else
				{
					result = ValidationRuleSkus.DatacenterTenant;
				}
				break;
			case Datacenter.ExchangeSku.PartnerHosted:
				if (flag)
				{
					result = ValidationRuleSkus.Hosted;
				}
				else
				{
					result = ValidationRuleSkus.HostedTenant;
				}
				break;
			}
			return result;
		}

		private static readonly Datacenter.ExchangeSku ExchangeSku = Datacenter.GetExchangeSku();
	}
}
