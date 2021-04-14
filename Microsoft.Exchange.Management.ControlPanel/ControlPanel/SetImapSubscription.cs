using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetImapSubscription : ImapSubscriptionBaseParameter
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-ImapSubscription";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public string ValidateSecret
		{
			get
			{
				return (string)(base["ValidateSecret"] ?? string.Empty);
			}
			set
			{
				base["ValidateSecret"] = value;
			}
		}

		[DataMember]
		public bool ResendVerification
		{
			get
			{
				return (bool)(base["ResendVerification"] ?? false);
			}
			set
			{
				base["ResendVerification"] = value;
			}
		}
	}
}
