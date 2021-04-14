using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class ResourceMailboxCapabilityIdentifierEvaluator : CapabilityIdentifierEvaluator
	{
		public ResourceMailboxCapabilityIdentifierEvaluator(Capability capability) : base(capability)
		{
		}

		public override CapabilityEvaluationResult Evaluate(ADRawEntry adObject)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Entering ResourceMailboxCapabilityIdentifierEvaluator.Evaluate('{0}') CapabilityToCheck '{1}'.", adObject.GetDistinguishedNameOrName(), base.Capability.ToString());
			CapabilityEvaluationResult capabilityEvaluationResult = CapabilityEvaluationResult.NotApplicable;
			ADRecipient adrecipient = adObject as ADRecipient;
			if (!(adObject is ReducedRecipient) && adrecipient == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "ResourceMailboxCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}' - adObject in not ReducedRecipient or ADUser.", adObject.GetDistinguishedNameOrName(), capabilityEvaluationResult.ToString(), base.Capability.ToString());
				return capabilityEvaluationResult;
			}
			capabilityEvaluationResult = ((adObject[ReducedRecipientSchema.ResourceType] != null) ? CapabilityEvaluationResult.Yes : CapabilityEvaluationResult.No);
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "ResourceMailboxCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}'", adObject.GetDistinguishedNameOrName(), capabilityEvaluationResult.ToString(), base.Capability.ToString());
			return capabilityEvaluationResult;
		}

		public override bool TryGetFilter(OrganizationId organizationId, out QueryFilter queryFilter, out LocalizedString errorMessage)
		{
			queryFilter = ResourceMailboxCapabilityIdentifierEvaluator.filter;
			errorMessage = LocalizedString.Empty;
			return true;
		}

		private static QueryFilter filter = new OrFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ReducedRecipientSchema.ResourceType, ExchangeResourceType.Room),
			new ComparisonFilter(ComparisonOperator.Equal, ReducedRecipientSchema.ResourceType, ExchangeResourceType.Equipment)
		});
	}
}
