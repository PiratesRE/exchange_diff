using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SubscriptionItemFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-Subscription";
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
