using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class FlightConfigurationException : Exception
	{
		public FlightConfigurationException(string message) : base(message)
		{
		}

		public FlightConfigurationException(string message, Exception exception) : base(message, exception)
		{
		}
	}
}
