using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetPopSubscription : PopSubscriptionBaseParameter
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-PopSubscription";
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
