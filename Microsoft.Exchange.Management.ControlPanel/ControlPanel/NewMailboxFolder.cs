using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewMailboxFolder : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-MailboxFolder";
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
		public string Name
		{
			get
			{
				return (string)base[MailboxFolderSchema.Name];
			}
			set
			{
				base[MailboxFolderSchema.Name] = value;
			}
		}

		[DataMember]
		public string Parent
		{
			get
			{
				return (string)base["Parent"];
			}
			set
			{
				base["Parent"] = value;
			}
		}

		public const string RbacParameters = "?Name&Parent";

		private const string StringParent = "Parent";
	}
}
