using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxFolderPermissionFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MailboxFolderPermission";
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
		public Identity Identity
		{
			get
			{
				return (Identity)base["Identity"];
			}
			set
			{
				base["Identity"] = value;
			}
		}

		public const string RbacParameters = "?Identity";
	}
}
