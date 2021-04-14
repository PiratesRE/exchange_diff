using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal abstract class CapabilityIdentifierEvaluator
	{
		protected CapabilityIdentifierEvaluator(Capability capability)
		{
			this.Capability = capability;
		}

		public Capability Capability { get; private set; }

		public abstract CapabilityEvaluationResult Evaluate(ADRawEntry adObject);

		public abstract bool TryGetFilter(OrganizationId organizationId, out QueryFilter queryFilter, out LocalizedString errorMessage);
	}
}
