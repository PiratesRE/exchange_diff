using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewSubscription : PimSubscriptionParameter
	{
		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			base["Name"] = base.EmailAddress;
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "New-Subscription";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		public new const string RbacParameters = "?Mailbox&Force&DisplayName&Name";

		public new const string RbacParametersWithIdentity = "?Mailbox&Force&DisplayName&Name&Identity";
	}
}
