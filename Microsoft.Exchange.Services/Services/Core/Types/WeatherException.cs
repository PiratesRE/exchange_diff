using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class WeatherException : Exception
	{
		public WeatherException(string message) : base(message)
		{
		}
	}
}
