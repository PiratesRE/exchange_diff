using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TryAddingAutodiscoverPrefix : AutodiscoverStep
	{
		internal TryAddingAutodiscoverPrefix(EasConnectionSettings easConnectionSettings) : base(easConnectionSettings, Step.TryUnauthenticatedGet)
		{
		}

		public override Step ExecuteStep(StepContext stepContext)
		{
			string autodiscoverDomain = base.GetAutodiscoverDomain(base.EasConnectionSettings.EasEndpointSettings.Domain);
			stepContext.ProbeStack.Push(autodiscoverDomain);
			return base.ExecuteStep(stepContext);
		}

		protected override bool IsStepAllowable(StepContext stepContext)
		{
			return stepContext.Request.AutodiscoverOption != AutodiscoverOption.ExistingEndpoint;
		}
	}
}
