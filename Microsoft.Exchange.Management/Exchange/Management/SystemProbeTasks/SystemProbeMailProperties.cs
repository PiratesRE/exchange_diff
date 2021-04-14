using System;

namespace Microsoft.Exchange.Management.SystemProbeTasks
{
	[Serializable]
	public class SystemProbeMailProperties
	{
		public Guid Guid { get; set; }

		public string MailMessage { get; set; }
	}
}
