using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class SimpleCapabilityIdentifierEvaluator : CapabilityIdentifierEvaluator
	{
		public SimpleCapabilityIdentifierEvaluator(Capability capability) : base(capability)
		{
			this.filter = new ComparisonFilter(ComparisonOperator.Equal, SharedPropertyDefinitions.RawCapabilities, base.Capability);
		}

		public override CapabilityEvaluationResult Evaluate(ADRawEntry adObject)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Entering SimpleCapabilityIdentifierEvaluator.Evaluate('{0}') CapabilityToCheck '{1}'.", adObject.GetDistinguishedNameOrName(), base.Capability.ToString());
			if (!adObject.propertyBag.Contains(SharedPropertyDefinitions.RawCapabilities))
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "SimpleCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}' - object doesn't have the Capabilities property.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.NotApplicable.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.NotApplicable;
			}
			CapabilityEvaluationResult capabilityEvaluationResult;
			if (OpathFilterEvaluator.FilterMatches(this.filter, adObject))
			{
				capabilityEvaluationResult = CapabilityEvaluationResult.Yes;
			}
			else
			{
				capabilityEvaluationResult = CapabilityEvaluationResult.No;
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "SimpleCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}'", adObject.GetDistinguishedNameOrName(), capabilityEvaluationResult.ToString(), base.Capability.ToString());
			return capabilityEvaluationResult;
		}

		public override bool TryGetFilter(OrganizationId organizationId, out QueryFilter queryFilter, out LocalizedString errorMessage)
		{
			queryFilter = this.filter;
			errorMessage = LocalizedString.Empty;
			return true;
		}

		private readonly QueryFilter filter;
	}
}
