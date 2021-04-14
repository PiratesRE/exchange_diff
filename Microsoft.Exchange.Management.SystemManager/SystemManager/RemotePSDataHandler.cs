using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal class RemotePSDataHandler : ExchangeDataHandler
	{
		public RemotePSDataHandler(string displayName)
		{
			this.displayName = displayName;
		}

		internal override void OnReadData(CommandInteractionHandler interactionHandler, string pageName)
		{
			PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo();
			this.mockSettings = new PSRemoteServer();
			this.mockSettings.DisplayName = this.displayName;
			this.mockSettings.UserAccount = PSConnectionInfoSingleton.GetInstance().UserAccount;
			this.mockSettings.RemotePSServer = PSConnectionInfoSingleton.GetInstance().RemotePSServer;
			this.mockSettings.AutomaticallySelect = (this.mockSettings.RemotePSServer == null);
			base.DataSource = this.mockSettings;
			base.OnReadData(interactionHandler, pageName);
		}

		internal override void OnSaveData(CommandInteractionHandler interactionHandler)
		{
			if (this.mockSettings.ObjectState == ObjectState.Changed)
			{
				PSConnectionInfoSingleton.GetInstance().UpdateRemotePSServer(this.mockSettings.AutomaticallySelect ? null : this.mockSettings.RemotePSServer);
			}
			base.OnSaveData(interactionHandler);
		}

		private PSRemoteServer mockSettings;

		private string displayName;
	}
}
