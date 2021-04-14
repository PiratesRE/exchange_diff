using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetUserMailboxFolderPermission : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MailboxFolderPermission";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		public string User
		{
			get
			{
				return (string)base["User"];
			}
			set
			{
				base["User"] = value;
			}
		}

		[DataMember]
		public string ReadAccessRights
		{
			get
			{
				return (string)base["AccessRights"];
			}
			set
			{
				base["AccessRights"] = value;
			}
		}
	}
}
