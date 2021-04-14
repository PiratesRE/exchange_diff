using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TryDnsLookupOfSrvRecord : AutodiscoverStep
	{
		internal TryDnsLookupOfSrvRecord(EasConnectionSettings easConnectionSettings) : base(easConnectionSettings, Step.Failed)
		{
		}

		public override Step ExecuteStep(StepContext stepContext)
		{
			return base.NextStepOnFailure;
		}

		protected override bool IsStepAllowable(StepContext stepContext)
		{
			return stepContext.Request.AutodiscoverOption != AutodiscoverOption.ExistingEndpoint;
		}
	}
}
