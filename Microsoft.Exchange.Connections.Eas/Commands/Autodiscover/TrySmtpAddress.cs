using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TrySmtpAddress : AutodiscoverStep
	{
		internal TrySmtpAddress(EasConnectionSettings easConnectionSettings) : base(easConnectionSettings, Step.TryRemovingDomainPrefix)
		{
		}

		public override Step ExecuteStep(StepContext stepContext)
		{
			stepContext.ProbeStack.Push(base.EasConnectionSettings.EasEndpointSettings.Domain);
			return base.ExecuteStep(stepContext);
		}

		protected override bool IsStepAllowable(StepContext stepContext)
		{
			return stepContext.Request.AutodiscoverOption != AutodiscoverOption.ExistingEndpoint;
		}
	}
}
