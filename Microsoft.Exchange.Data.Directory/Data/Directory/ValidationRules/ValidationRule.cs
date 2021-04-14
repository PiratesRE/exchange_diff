using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	[Serializable]
	internal abstract class ValidationRule
	{
		public ValidationRule(ValidationRuleDefinition ruleDefinition, IList<CapabilityIdentifierEvaluator> restrictedCapabilityEvaluators, IList<CapabilityIdentifierEvaluator> overridingAllowCapabilityEvaluators, RoleEntry roleEntry)
		{
			if (ruleDefinition == null)
			{
				throw new ArgumentNullException("ruleDefinition");
			}
			if (restrictedCapabilityEvaluators == null)
			{
				throw new ArgumentNullException("restrictedCapabilityEvaluators");
			}
			if (overridingAllowCapabilityEvaluators == null)
			{
				throw new ArgumentNullException("overridingAllowCapabilityEvaluators");
			}
			this.RestrictedCapabilityEvaluators = restrictedCapabilityEvaluators;
			this.OverridingAllowCapabilityEvaluators = overridingAllowCapabilityEvaluators;
			this.ruleDefinition = ruleDefinition;
			this.roleEntry = roleEntry;
		}

		public string Name
		{
			get
			{
				return this.ruleDefinition.Name;
			}
		}

		public string Cmdlet
		{
			get
			{
				if (!(null != this.roleEntry))
				{
					return null;
				}
				return this.roleEntry.Name;
			}
		}

		public ICollection<string> Parameters
		{
			get
			{
				if (!(null != this.roleEntry))
				{
					return null;
				}
				return this.roleEntry.Parameters;
			}
		}

		public IList<CapabilityIdentifierEvaluator> RestrictedCapabilityEvaluators { get; private set; }

		public IList<CapabilityIdentifierEvaluator> OverridingAllowCapabilityEvaluators { get; private set; }

		protected ValidationRuleDefinition RuleDefinition
		{
			get
			{
				return this.ruleDefinition;
			}
		}

		public bool TryValidate(ADRawEntry adObject, out RuleValidationException validationException)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Entering {0}.TryValidate({1}). Rule {2}.", base.GetType().Name, adObject.GetDistinguishedNameOrName(), this.ruleDefinition.Name);
			bool result = this.InternalTryValidate(adObject, out validationException);
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "{0}.TryValidate({1}). returns {2}", base.GetType().Name, adObject.GetDistinguishedNameOrName(), result.ToString());
			return result;
		}

		protected abstract bool InternalTryValidate(ADRawEntry adObject, out RuleValidationException validationException);

		protected LocalizedString GetValidationRuleErrorMessage(ADRawEntry adObject, Capability culpritCapability)
		{
			return this.GetValidationRuleErrorMessage(adObject, culpritCapability.ToString());
		}

		protected LocalizedString GetValidationRuleErrorMessage(ADRawEntry adObject, string culpritCapabilityOrFilter)
		{
			if (null == this.roleEntry)
			{
				return LocalizedString.Empty;
			}
			return this.ruleDefinition.ErrorString((adObject.Id != null) ? adObject.Id.Name : ((string)adObject[ADObjectSchema.Name]), this.roleEntry.Name, string.Join(",", this.roleEntry.Parameters.ToArray<string>()), culpritCapabilityOrFilter);
		}

		protected bool IsOverridingAllowCapabilityFound(ADRawEntry adObject)
		{
			CapabilityIdentifierEvaluator capabilityIdentifierEvaluator = this.OverridingAllowCapabilityEvaluators.FirstOrDefault((CapabilityIdentifierEvaluator x) => x.Evaluate(adObject) == CapabilityEvaluationResult.Yes);
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ValidationRule.IsOverridingAllowCapabilityFound({0}). OverridingAllowCapability: {1}.", adObject.GetDistinguishedNameOrName(), (capabilityIdentifierEvaluator != null) ? capabilityIdentifierEvaluator.Capability.ToString() : "<NULL>");
			return capabilityIdentifierEvaluator != null;
		}

		private ValidationRuleDefinition ruleDefinition;

		private RoleEntry roleEntry;
	}
}
