using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.SetPop3ConfigurationTask)]
	[Cmdlet("Set", "PopSettings", SupportsShouldProcess = true)]
	public sealed class SetPop3Configuration : SetPopImapConfiguration<Pop3AdConfiguration>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetPop3Config;
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
				foreach (string text in this.InvalidCafeRoleFieldsForPop3)
				{
					if (base.UserSpecifiedParameters[text] != null)
					{
						this.WriteError(new ExInvalidArgumentForServerRoleException(text, Strings.InstallCafeRoleDescription), ErrorCategory.InvalidArgument, null, false);
					}
				}
			}
		}

		private readonly string[] InvalidCafeRoleFieldsForPop3 = new string[]
		{
			"MessageRetrievalSortOrder"
		};
	}
}
