using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class RoboCopyException : Exception
	{
		public RoboCopyException()
		{
		}

		public RoboCopyException(string message) : base(message)
		{
		}
	}
}
