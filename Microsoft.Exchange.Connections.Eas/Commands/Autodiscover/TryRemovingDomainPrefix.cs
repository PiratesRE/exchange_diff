using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TryRemovingDomainPrefix : AutodiscoverStep
	{
		internal TryRemovingDomainPrefix(EasConnectionSettings easConnectionSettings) : base(easConnectionSettings, Step.TryAddingAutodiscoverPrefix)
		{
		}

		public override Step ExecuteStep(StepContext stepContext)
		{
			string mostRecentDomain;
			if (base.TryStrippingPrefixFromDomain(stepContext.EasConnectionSettings.EasEndpointSettings.Domain, out mostRecentDomain))
			{
				stepContext.EasConnectionSettings.EasEndpointSettings.MostRecentDomain = mostRecentDomain;
				return Step.TrySmtpAddress;
			}
			return base.NextStepOnFailure;
		}

		protected override bool IsStepAllowable(StepContext stepContext)
		{
			return stepContext.Request.AutodiscoverOption != AutodiscoverOption.ExistingEndpoint;
		}
	}
}
