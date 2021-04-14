using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface IExecuteStep
	{
		Step PrepareAndExecuteStep(StepContext stepContext);

		Step ExecuteStep(StepContext stepContext);
	}
}
