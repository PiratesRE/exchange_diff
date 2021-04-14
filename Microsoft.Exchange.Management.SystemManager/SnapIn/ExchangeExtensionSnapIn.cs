using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SnapIn
{
	public abstract class ExchangeExtensionSnapIn : NamespaceExtension, IExchangeSnapIn, IServiceProvider
	{
		public ExchangeExtensionSnapIn()
		{
			this.helper = new ExchangeSnapInHelper(this, this);
			this.helper.InitializeSettingProvider();
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			return this.helper.Services.GetService(serviceType);
		}

		protected override void OnInitialize()
		{
			this.helper.OnInitialize();
		}

		public virtual void Initialize(IProgressProvider progressProvider)
		{
			this.helper.Initialize(progressProvider);
		}

		protected override void OnShutdown(AsyncStatus status)
		{
			this.helper.Shutdown(status);
		}

		public IContainer Components
		{
			get
			{
				return this.helper.Components;
			}
		}

		public IUIService ShellUI
		{
			get
			{
				return this.helper.ShellUI;
			}
		}

		public DialogResult ShowDialog(CommonDialog dialog)
		{
			return this.helper.ShowDialog(dialog);
		}

		public virtual string InstanceDisplayName
		{
			get
			{
				return this.helper.InstanceDisplayName;
			}
		}

		public ExchangeSettings Settings
		{
			get
			{
				return this.helper.Settings;
			}
		}

		public virtual ExchangeSettings CreateSettings(IComponent owner)
		{
			return this.helper.CreateSettings(owner);
		}

		protected override void OnLoadCustomData(AsyncStatus status, byte[] customData)
		{
			this.helper.OnLoadCustomData(status, customData);
		}

		protected override byte[] OnSaveCustomData(SyncStatus status)
		{
			return this.helper.OnSaveCustomData(status);
		}

		public override string ToString()
		{
			return this.helper.ToString();
		}

		public int RegisterIcon(string name, Icon icon)
		{
			return this.helper.RegisterIcon(name, icon);
		}

		public ScopeNodeCollection ScopeNodeCollection
		{
			get
			{
				return base.PrimaryNode.Children;
			}
		}

		public virtual string SnapInGuidString
		{
			get
			{
				return null;
			}
		}

		public string RootNodeDisplayName
		{
			get
			{
				return base.PrimaryNode.Children[0].DisplayName;
			}
			set
			{
				base.PrimaryNode.Children[0].DisplayName = value;
			}
		}

		public virtual Icon RootNodeIcon
		{
			get
			{
				return (base.PrimaryNode.Children[0] as ExchangeScopeNode).Icon;
			}
		}

		private ExchangeSnapInHelper helper;
	}
}
