using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab.Probes
{
	public class InvalidOabManifestFileException : Exception
	{
		public InvalidOabManifestFileException(string user, string message) : base(string.Format("Invalid manifest file returned by the server for user {0}. {1}", user, message))
		{
		}
	}
}
