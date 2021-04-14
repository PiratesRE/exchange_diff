using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.GetItemEstimate
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetItemEstimateCommand : EasServerCommand<GetItemEstimateRequest, GetItemEstimateResponse, GetItemEstimateStatus>
	{
		internal GetItemEstimateCommand(EasConnectionSettings easConnectionSettings) : base(Command.GetItemEstimate, easConnectionSettings)
		{
		}
	}
}
