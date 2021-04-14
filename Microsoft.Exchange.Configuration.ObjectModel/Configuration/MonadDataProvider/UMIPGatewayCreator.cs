using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class UMIPGatewayCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"WhenChanged",
				"Name",
				"OutcallsAllowed",
				"MessageWaitingIndicatorAllowed",
				"Address",
				"Status"
			};
		}
	}
}
