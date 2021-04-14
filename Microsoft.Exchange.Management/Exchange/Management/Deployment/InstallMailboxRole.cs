using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "MailboxRole", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class InstallMailboxRole : ManageMailboxRole
	{
		public InstallMailboxRole()
		{
			base.Fields["MailboxDatabaseName"] = "Mailbox Database";
			base.Fields["PublicFolderDatabaseName"] = "Public Folder Database";
			base.Fields["CustomerFeedbackEnabled"] = null;
			base.Fields["MdbName"] = null;
			base.Fields["DbFilePath"] = null;
			base.Fields["LogFolderPath"] = null;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.InstallMailboxRoleDescription;
			}
		}

		[Parameter]
		public string MdbName
		{
			get
			{
				return (string)base.Fields["MdbName"];
			}
			set
			{
				base.Fields["MdbName"] = value;
			}
		}

		[Parameter]
		public string DbFilePath
		{
			get
			{
				return (string)base.Fields["DbFilePath"];
			}
			set
			{
				base.Fields["DbFilePath"] = value;
			}
		}

		[Parameter]
		public string LogFolderPath
		{
			get
			{
				return (string)base.Fields["LogFolderPath"];
			}
			set
			{
				base.Fields["LogFolderPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)base.Fields["CustomerFeedbackEnabled"];
			}
			set
			{
				base.Fields["CustomerFeedbackEnabled"] = value;
			}
		}
	}
}
