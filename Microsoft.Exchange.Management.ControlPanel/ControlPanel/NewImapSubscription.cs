using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewImapSubscription : ImapSubscriptionBaseParameter
	{
		public NewImapSubscription()
		{
			this.OnDeserialized(default(StreamingContext));
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "New-ImapSubscription";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext contex)
		{
			base["Name"] = base.EmailAddress;
		}

		public new const string RbacParameters = "?Mailbox&Name";

		public new const string RbacParametersWithIdentity = "?Mailbox&Name&Identity";
	}
}
