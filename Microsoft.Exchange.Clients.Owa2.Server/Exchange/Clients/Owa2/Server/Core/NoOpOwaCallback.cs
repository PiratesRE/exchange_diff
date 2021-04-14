using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class NoOpOwaCallback : IOwaCallback
	{
		public void ProcessCallback(object owaContext)
		{
		}

		public static readonly NoOpOwaCallback Prototype = new NoOpOwaCallback();
	}
}
