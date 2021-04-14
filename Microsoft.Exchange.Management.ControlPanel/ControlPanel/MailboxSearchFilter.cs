using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxSearchFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MailboxSearch";
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
