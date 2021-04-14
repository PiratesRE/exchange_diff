using System;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.SnapIn
{
	public class MMCCommunicationChannel
	{
		public WritableSharedDataItem Channel { get; set; }

		public bool Initiated { get; set; }
	}
}
