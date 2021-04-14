using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SoftDeletedMailboxFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-Mailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return string.Empty;
			}
		}
	}
}
