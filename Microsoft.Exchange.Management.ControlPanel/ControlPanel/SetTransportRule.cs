using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetTransportRule : TransportRuleParameters
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Set-TransportRule";
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
