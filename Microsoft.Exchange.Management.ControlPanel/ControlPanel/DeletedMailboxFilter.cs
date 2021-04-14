using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DeletedMailboxFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-RemovedMailbox";
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
