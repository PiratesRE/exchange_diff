using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetUMMailboxPinParameters : UMBasePinSetParameteres
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-UMMailboxPin";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}
	}
}
