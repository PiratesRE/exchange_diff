using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab.Probes
{
	public class InvalidOabDataFileException : Exception
	{
		public InvalidOabDataFileException(string user, string message) : base(string.Format("Invalid server response for user {0}. {1}", user, message))
		{
		}
	}
}
