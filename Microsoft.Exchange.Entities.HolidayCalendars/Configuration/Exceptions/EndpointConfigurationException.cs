using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions
{
	public class EndpointConfigurationException : HolidayCalendarException
	{
		public EndpointConfigurationException(EndPointConfigurationError error, string message, params object[] args) : base("Message: '{0}', Error: '{1}'", new object[]
		{
			string.Format(message, args),
			error.ToString()
		})
		{
			this.Error = error;
		}

		public EndpointConfigurationException(EndPointConfigurationError error, Exception innerException, string message, params object[] args) : base("Message: '{0}', Error: '{1}' InnerException:'{2}'", new object[]
		{
			string.Format(message, args),
			error.ToString(),
			innerException
		})
		{
			this.Error = error;
		}

		public EndPointConfigurationError Error { get; private set; }
	}
}
