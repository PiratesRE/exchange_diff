using System;
using System.Management.Automation;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CmdletRequestDiagnosticData : RequestDiagnosticData
	{
		[DataMember]
		public ErrorRecord ErrorRecord { get; set; }

		[DataMember]
		public PowershellCommandDiagnosticData[] Command { get; set; }
	}
}
