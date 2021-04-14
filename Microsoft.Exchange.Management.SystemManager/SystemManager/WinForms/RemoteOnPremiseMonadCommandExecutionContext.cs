using System;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class RemoteOnPremiseMonadCommandExecutionContext : MonadCommandExecutionContext
	{
		public string ServerName { get; set; }

		protected override MonadConnection CreateMonadConnection(IUIService uiService, CommandInteractionHandler commandInteractionHandler)
		{
			MonadConnectionInfo monadConnectionInfo = PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo();
			return new MonadConnection("timeout=30", commandInteractionHandler, null, new MonadConnectionInfo(PSConnectionInfoSingleton.GetRemotePowerShellUri(new Fqdn(this.ServerName)), monadConnectionInfo.Credentials, monadConnectionInfo.ShellUri, monadConnectionInfo.FileTypesXml, monadConnectionInfo.AuthenticationMechanism, monadConnectionInfo.SerializationLevel, monadConnectionInfo.ClientApplication, string.Empty, monadConnectionInfo.MaximumConnectionRedirectionCount, monadConnectionInfo.SkipServerCertificateCheck));
		}
	}
}
