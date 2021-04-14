using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMCallAnsweringRuleFilter : WebServiceParameters
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Get-UMCallAnsweringRule";
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
