using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox.Escalation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupEscalationFlightInfo : IGroupEscalationFlightInfo
	{
		public GroupEscalationFlightInfo(IConstraintProvider constraintProvider)
		{
			this.constraintProvider = constraintProvider;
		}

		public bool IsGroupEscalationFooterEnabled()
		{
			return true;
		}

		private VariantConfigurationSnapshot VariantConfig
		{
			get
			{
				if (this.variantConfiguration == null)
				{
					this.variantConfiguration = VariantConfiguration.GetSnapshot(this.constraintProvider, null, null);
				}
				return this.variantConfiguration;
			}
		}

		private readonly IConstraintProvider constraintProvider;

		private VariantConfigurationSnapshot variantConfiguration;
	}
}
