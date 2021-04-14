using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewTransportRule : TransportRuleParameters
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "New-TransportRule";
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
