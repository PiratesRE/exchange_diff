using System;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands.Anonymous;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class OWAAnonymousService : IOWAAnonymousCalendarService
	{
		public FindItemJsonResponse FindItem(FindItemJsonRequest request)
		{
			return new FindItemAnonymous(request).Execute();
		}

		public GetItemJsonResponse GetItem(GetItemJsonRequest request)
		{
			return new GetItemAnonymous(request).Execute();
		}
	}
}
