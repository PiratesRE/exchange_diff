using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SnapIn
{
	public abstract class ExchangeDynamicServerSnapIn : ExchangeSnapIn
	{
		public ExchangeDynamicServerSnapIn()
		{
			PSConnectionInfoSingleton.GetInstance().RemotePSServerChanged += delegate(object param0, EventArgs param1)
			{
				this.UpdateRemotePSServerSettings();
			};
		}

		private void UpdateRemotePSServerSettings()
		{
			bool cancelAutoRefresh = true;
			try
			{
				this.Settings.DoBeginInit();
				this.Settings.RemotePSServer = PSConnectionInfoSingleton.GetInstance().RemotePSServer;
				cancelAutoRefresh = false;
			}
			finally
			{
				this.Settings.DoEndInit(cancelAutoRefresh);
			}
		}

		public new ExchangeDynamicServerSettings Settings
		{
			get
			{
				return (ExchangeDynamicServerSettings)base.Settings;
			}
		}

		public override ExchangeSettings CreateSettings(IComponent owner)
		{
			return SettingsBase.Synchronized(new ExchangeDynamicServerSettings(owner)) as ExchangeSettings;
		}

		public override void Initialize(IProgressProvider progressProvider)
		{
			PSConnectionInfoSingleton.GetInstance().DisplayName = this.RootNodeDisplayName;
			PSConnectionInfoSingleton.GetInstance().Type = OrganizationType.ToolOrEdge;
			PSConnectionInfoSingleton.GetInstance().Uri = PSConnectionInfoSingleton.GetRemotePowerShellUri(this.Settings.RemotePSServer);
			PSConnectionInfoSingleton.GetInstance().LogonWithDefaultCredential = true;
			PSConnectionInfoSingleton.GetInstance().Enabled = true;
			base.Initialize(progressProvider);
		}
	}
}
