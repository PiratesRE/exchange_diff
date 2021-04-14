using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class StepContext
	{
		public StepContext(AutodiscoverRequest request, EasConnectionSettings easConnectionSettings)
		{
			this.Request = request;
			this.EasConnectionSettings = easConnectionSettings;
			this.ProbeStack = new Stack<string>();
		}

		internal AutodiscoverRequest Request { get; private set; }

		internal EasConnectionSettings EasConnectionSettings { get; private set; }

		internal Stack<string> ProbeStack { get; private set; }

		internal AutodiscoverResponse Response { get; set; }

		internal HttpStatusCode HttpStatusCode { get; set; }

		internal Exception Error { get; set; }
	}
}
