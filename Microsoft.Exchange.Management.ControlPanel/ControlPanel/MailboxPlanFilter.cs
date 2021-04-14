using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxPlanFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MailboxPlan";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}
	}
}
