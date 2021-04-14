using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class InboxRuleFilter : WebServiceParameters
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Get-InboxRule";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}
	}
}
