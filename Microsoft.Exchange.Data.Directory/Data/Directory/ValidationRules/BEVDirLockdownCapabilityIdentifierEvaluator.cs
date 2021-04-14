using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class BEVDirLockdownCapabilityIdentifierEvaluator : CapabilityIdentifierEvaluator
	{
		public BEVDirLockdownCapabilityIdentifierEvaluator(Capability capability) : base(capability)
		{
		}

		public override CapabilityEvaluationResult Evaluate(ADRawEntry adObject)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			ExchangeVirtualDirectory exchangeVirtualDirectory = adObject as ExchangeVirtualDirectory;
			if (exchangeVirtualDirectory == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "BEVDirLockdownCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}'. Object isn't a ExchangeVirtualDirectory object.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.NotApplicable.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.NotApplicable;
			}
			if (exchangeVirtualDirectory.Name.Contains("Exchange Back End"))
			{
				return CapabilityEvaluationResult.Yes;
			}
			return CapabilityEvaluationResult.No;
		}

		public override bool TryGetFilter(OrganizationId organizationId, out QueryFilter queryFilter, out LocalizedString errorMessage)
		{
			errorMessage = LocalizedString.Empty;
			queryFilter = BEVDirLockdownCapabilityIdentifierEvaluator.filter;
			return false;
		}

		private static readonly QueryFilter filter;
	}
}
