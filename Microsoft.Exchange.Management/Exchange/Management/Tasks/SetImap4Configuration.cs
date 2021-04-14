using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.SetImap4ConfigurationTask)]
	[Cmdlet("Set", "ImapSettings", SupportsShouldProcess = true)]
	public sealed class SetImap4Configuration : SetPopImapConfiguration<Imap4AdConfiguration>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetImap4Config;
			}
		}

		protected override void ValidateSetServerRoleSpecificParameters()
		{
			base.ValidateSetServerRoleSpecificParameters();
			if ((base.ServerObject.IsClientAccessServer && base.ServerObject.IsCafeServer) || !base.ServerObject.IsE15OrLater)
			{
				return;
			}
			if (base.ServerObject.IsCafeServer)
			{
				foreach (string text in this.InvalidCafeRoleFieldsForImap4)
				{
					if (base.UserSpecifiedParameters[text] != null)
					{
						this.WriteError(new ExInvalidArgumentForServerRoleException(text, Strings.InstallCafeRoleDescription), ErrorCategory.InvalidArgument, null, false);
					}
				}
			}
		}

		private readonly string[] InvalidCafeRoleFieldsForImap4 = new string[]
		{
			"ShowHiddenFoldersEnabled"
		};
	}
}
