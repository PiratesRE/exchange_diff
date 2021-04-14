using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel.DataContracts
{
	[DataContract]
	public class GetMailboxPermissionParameters : MailboxPermissionParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MailboxPermission";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}

		[DataMember]
		public Identity User
		{
			get
			{
				return (Identity)base["User"];
			}
			set
			{
				base["User"] = value;
			}
		}

		private const string GetMailboxPermission = "Get-MailboxPermission";
	}
}
