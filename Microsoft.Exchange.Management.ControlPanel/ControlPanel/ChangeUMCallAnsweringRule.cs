using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ChangeUMCallAnsweringRule : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-UMCallAnsweringRule";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}
	}
}
