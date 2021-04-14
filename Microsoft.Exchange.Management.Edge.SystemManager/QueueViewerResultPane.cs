using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	public class QueueViewerResultPane : TabbedResultPane
	{
		public QueueViewerResultPane()
		{
			this.InitializeComponent();
			base.CaptionText = Strings.QueueViewerNodeName(Strings.QueueViewerNotConnected);
			base.Icon = Icons.QueueViewerTool;
			this.commandQueuesView = new Command();
			this.commandQueuesView.Description = LocalizedString.Empty;
			this.commandQueuesView.Name = "commandQueuesView";
			this.commandQueuesView.Text = Strings.QueueViewerQueuesText;
			this.commandQueuesView.Execute += this.commandQueuesView_Execute;
			this.commandAllMessagesView = new Command();
			this.commandAllMessagesView.Description = LocalizedString.Empty;
			this.commandAllMessagesView.Name = "commandAllMessagesView";
			this.commandAllMessagesView.Text = Strings.QueueViewerMessagesText;
			this.commandAllMessagesView.Execute += this.commandAllMessagesView_Execute;
			this.commandMessagesView = new Command();
			this.commandMessagesView.Description = LocalizedString.Empty;
			this.commandMessagesView.Name = "commandMessagesView";
			this.commandMessagesView.Text = Strings.QueueViewerMessagesText;
			this.commandMessagesView.Execute += this.commandMessagesView_Execute;
			this.commandMessagesView.Visible = false;
			this.commandOptions = new Command();
			this.commandOptions.Description = LocalizedString.Empty;
			this.commandOptions.Name = "commandMessagesView";
			this.commandOptions.Text = Strings.QueueViewerOptionsText;
			this.commandOptions.Execute += this.commandOptions_Execute;
			base.ViewModeCommands.Add(this.commandQueuesView);
			base.ViewModeCommands.Add(this.commandAllMessagesView);
			base.ViewModeCommands.Add(this.commandMessagesView);
			base.ViewModeCommands.Add(this.commandOptions);
			this.commandConnectToServer = new Command();
			this.commandConnectToServer.Description = LocalizedString.Empty;
			this.commandConnectToServer.Name = "ConnectToServer";
			this.commandConnectToServer.Icon = Icons.ConnectToServer;
			this.commandConnectToServer.Text = Strings.ConnectToServerCommand;
			this.commandConnectToServer.Execute += this.commandConnectToServer_Execute;
			this.commandConnectToServer.Visible = false;
			base.ResultPaneCommands.Add(this.commandConnectToServer);
			base.ResultPaneTabs.Add(this.queueListResultPane);
			base.ResultPaneTabs.Add(this.allMessagesListResultPane);
			this.messageListResultPane.ViewModeCommands.Remove(this.messageListResultPane.SaveDefaultFilterCommand);
			this.Dock = DockStyle.Fill;
			this.timer = new Timer();
			this.timer.Tick += delegate(object param0, EventArgs param1)
			{
				if (this.AutoRefreshEnabled)
				{
					QueueViewerResultPaneBase queueViewerResultPaneBase = base.SelectedResultPane as QueueViewerResultPaneBase;
					queueViewerResultPaneBase.SetRefreshWhenActivated();
				}
			};
			this.timer.Interval = this.RefreshInterval * 1000;
		}

		protected override void OnSetActive(EventArgs e)
		{
			base.OnSetActive(e);
			if (!this.alreadySetActive)
			{
				this.alreadySetActive = true;
				bool visible = true;
				RoleCollection installedRoles = RoleManager.GetInstalledRoles();
				foreach (Role role in installedRoles)
				{
					if ((role.ServerRole == ServerRole.Edge || role.ServerRole == ServerRole.HubTransport || role.ServerRole == ServerRole.Mailbox) && string.IsNullOrEmpty(this.ServerName))
					{
						this.ServerName = NativeHelpers.GetLocalComputerFqdn(false);
					}
					if (role.ServerRole == ServerRole.Edge)
					{
						visible = false;
					}
				}
				this.commandConnectToServer.Visible = visible;
				if (string.IsNullOrEmpty(this.ServerName))
				{
					this.commandConnectToServer.Invoke();
				}
			}
		}

		protected override ExchangeSettings CreatePrivateSettings(IComponent owner)
		{
			return new QueueViewerSettings(this);
		}

		private void InitializeComponent()
		{
			this.queueListResultPane = new QueueViewerQueuesResultPane();
			this.allMessagesListResultPane = new QueueViewerMessagesResultPane();
			this.messageListResultPane = new QueueViewerMessagesResultPane();
			this.queueListResultPane.Name = "QueuesListResultPane";
			this.queueListResultPane.Text = Strings.QueueViewerQueues;
			this.allMessagesListResultPane.Name = "AllMessagesListResultPane";
			this.allMessagesListResultPane.Text = Strings.QueueViewerMessages;
			this.messageListResultPane.Name = "MessageListResultPane";
			this.messageListResultPane.Text = Strings.QueueViewerMessages;
			this.DoubleBuffered = true;
			base.Name = "QueueViewerResultPane";
		}

		private void commandConnectToServer_Execute(object sender, EventArgs e)
		{
			ConnectToServerParams connectToServerParams = new ConnectToServerParams(false, string.Empty);
			DataContext context = new DataContext(new ExchangeDataHandler
			{
				DataSource = connectToServerParams
			});
			ConnectToServerControl connectToServerControl = new ConnectToServerControl();
			connectToServerControl.Text = Strings.ConnectToServer;
			connectToServerControl.ConnectServerLabelText = Strings.SelectTransportServerLabelText;
			connectToServerControl.SetDefaultServerCheckBoxText = Strings.SetDefaultServerCheckBoxText;
			connectToServerControl.AutoSize = true;
			connectToServerControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			connectToServerControl.ServerRoleToPicker = (ServerRole.Mailbox | ServerRole.HubTransport);
			connectToServerControl.Context = context;
			using (PropertyPageDialog propertyPageDialog = new PropertyPageDialog(connectToServerControl))
			{
				propertyPageDialog.OkButtonText = Strings.ConnectButtonText;
				propertyPageDialog.LinkIsDirtyToOkEnabled = true;
				propertyPageDialog.HelpTopic = HelpId.ConnectToServerControl.ToString();
				if (this.QueueSettings.UseDefaultServer)
				{
					connectToServerParams.ServerName = this.QueueSettings.DefaultServerName;
					connectToServerControl.IsDirty = true;
				}
				if (this.ShowDialog(propertyPageDialog) == DialogResult.OK)
				{
					this.ServerName = connectToServerParams.ServerName;
					if (connectToServerParams.SetAsDefaultServer)
					{
						this.QueueSettings.UseDefaultServer = connectToServerParams.SetAsDefaultServer;
						this.QueueSettings.DefaultServerName = connectToServerParams.ServerName;
					}
				}
			}
		}

		internal string ServerName
		{
			get
			{
				return this.serverName;
			}
			set
			{
				if (value != this.serverName)
				{
					if (base.ResultPaneTabs.Contains(this.messageListResultPane))
					{
						base.ResultPaneTabs.Remove(this.messageListResultPane);
						base.ViewModeCommands.Remove(this.commandMessagesView);
					}
					this.serverName = value;
					base.CaptionText = Strings.QueueViewerNodeName(this.serverName);
					this.QueueListResultPane.ServerName = this.serverName;
					this.AllMessagesListResultPane.ServerName = this.serverName;
					this.MessageListResultPane.ServerName = this.serverName;
				}
			}
		}

		internal QueueViewerResultPane.ViewModes CurrentView
		{
			get
			{
				QueueViewerResultPane.ViewModes result = QueueViewerResultPane.ViewModes.QueueView;
				if (base.SelectedResultPane == this.QueueListResultPane)
				{
					result = QueueViewerResultPane.ViewModes.QueueView;
				}
				else if (base.SelectedResultPane == this.AllMessagesListResultPane)
				{
					result = QueueViewerResultPane.ViewModes.AllMessageView;
				}
				else if (base.SelectedResultPane == this.MessageListResultPane)
				{
					result = QueueViewerResultPane.ViewModes.MessageView;
				}
				return result;
			}
			set
			{
				switch (value)
				{
				case QueueViewerResultPane.ViewModes.QueueView:
					base.SelectedResultPane = this.QueueListResultPane;
					return;
				case QueueViewerResultPane.ViewModes.AllMessageView:
					base.SelectedResultPane = this.AllMessagesListResultPane;
					return;
				case QueueViewerResultPane.ViewModes.MessageView:
					base.SelectedResultPane = this.MessageListResultPane;
					return;
				default:
					return;
				}
			}
		}

		internal QueueViewerQueuesResultPane QueueListResultPane
		{
			get
			{
				return this.queueListResultPane;
			}
		}

		internal QueueViewerMessagesResultPane MessageListResultPane
		{
			get
			{
				return this.messageListResultPane;
			}
		}

		internal QueueViewerMessagesResultPane AllMessagesListResultPane
		{
			get
			{
				return this.allMessagesListResultPane;
			}
		}

		internal int PageSize
		{
			get
			{
				return this.pageSize;
			}
			set
			{
				if (value != this.pageSize)
				{
					this.pageSize = value;
					this.AllMessagesListResultPane.PageSize = this.pageSize;
					this.MessageListResultPane.PageSize = this.pageSize;
					this.QueueListResultPane.PageSize = this.pageSize;
				}
			}
		}

		internal bool AutoRefreshEnabled
		{
			get
			{
				return this.autoRefreshEnabled;
			}
			set
			{
				this.autoRefreshEnabled = value;
				this.timer.Enabled = this.autoRefreshEnabled;
			}
		}

		internal int RefreshInterval
		{
			get
			{
				return this.refreshInterval;
			}
			set
			{
				this.refreshInterval = value;
				this.timer.Interval = (int)EnhancedTimeSpan.FromSeconds((double)this.refreshInterval).TotalMilliseconds;
			}
		}

		internal QueueViewerSettings QueueSettings
		{
			get
			{
				return (QueueViewerSettings)base.PrivateSettings;
			}
		}

		internal Command CommandQueuesView
		{
			get
			{
				return this.commandQueuesView;
			}
		}

		internal Command CommandAllMessagesView
		{
			get
			{
				return this.commandAllMessagesView;
			}
		}

		internal Command CommandMessagesView
		{
			get
			{
				return this.commandMessagesView;
			}
		}

		private void commandQueuesView_Execute(object sender, EventArgs e)
		{
			this.CurrentView = QueueViewerResultPane.ViewModes.QueueView;
		}

		private void commandMessagesView_Execute(object sender, EventArgs e)
		{
			this.CurrentView = QueueViewerResultPane.ViewModes.MessageView;
		}

		private void commandAllMessagesView_Execute(object sender, EventArgs e)
		{
			this.CurrentView = QueueViewerResultPane.ViewModes.AllMessageView;
		}

		internal Command CommandConnectToServer
		{
			get
			{
				return this.commandConnectToServer;
			}
		}

		internal Command CommandOptions
		{
			get
			{
				return this.commandOptions;
			}
		}

		private void commandOptions_Execute(object sender, EventArgs e)
		{
			QueueViewerOptions queueViewerOptions = new QueueViewerOptions(this.AutoRefreshEnabled, EnhancedTimeSpan.FromSeconds((double)this.RefreshInterval), (uint)this.PageSize);
			DataContext context = new DataContext(new ExchangeDataHandler
			{
				DataSource = queueViewerOptions
			});
			using (PropertyPageDialog propertyPageDialog = new PropertyPageDialog(new QueueViewerPropertyPage
			{
				Context = context
			}))
			{
				propertyPageDialog.LinkIsDirtyToOkEnabled = true;
				if (this.ShowDialog(propertyPageDialog) == DialogResult.OK)
				{
					this.AutoRefreshEnabled = queueViewerOptions.AutoRefreshEnabled;
					this.RefreshInterval = (int)queueViewerOptions.RefreshInterval.TotalSeconds;
					this.PageSize = (int)queueViewerOptions.PageSize;
				}
			}
		}

		internal void SetDatasourcesOnView()
		{
			this.QueueListResultPane.Datasource = new QueueViewerDataSource("queue");
			this.MessageListResultPane.Datasource = new QueueViewerDataSource("message");
			this.AllMessagesListResultPane.Datasource = new QueueViewerDataSource("message");
			base.Components.Add(this.QueueListResultPane.Datasource);
			base.Components.Add(this.MessageListResultPane.Datasource);
			base.Components.Add(this.AllMessagesListResultPane.Datasource);
		}

		public override void LoadComponentSettings()
		{
			base.LoadComponentSettings();
			this.PageSize = this.QueueSettings.PageSize;
			this.AutoRefreshEnabled = this.QueueSettings.AutoRefreshEnabled;
			this.RefreshInterval = (int)this.QueueSettings.RefreshInterval.TotalSeconds;
			if (this.QueueSettings.UseDefaultServer)
			{
				this.ServerName = this.QueueSettings.DefaultServerName;
			}
		}

		public override void ResetComponentSettings()
		{
			base.ResetComponentSettings();
			this.PageSize = this.QueueSettings.PageSize;
			this.AutoRefreshEnabled = this.QueueSettings.AutoRefreshEnabled;
			this.RefreshInterval = (int)this.QueueSettings.RefreshInterval.TotalSeconds;
		}

		public override void SaveComponentSettings()
		{
			this.QueueSettings.PageSize = this.PageSize;
			this.QueueSettings.AutoRefreshEnabled = this.AutoRefreshEnabled;
			this.QueueSettings.RefreshInterval = EnhancedTimeSpan.FromSeconds((double)this.RefreshInterval);
			base.SaveComponentSettings();
		}

		private QueueViewerQueuesResultPane queueListResultPane;

		private QueueViewerMessagesResultPane allMessagesListResultPane;

		private QueueViewerMessagesResultPane messageListResultPane;

		private Command commandQueuesView;

		private Command commandAllMessagesView;

		private Command commandMessagesView;

		private Command commandOptions;

		private Command commandConnectToServer;

		private Timer timer;

		private bool alreadySetActive;

		private string serverName = string.Empty;

		private int pageSize = 1000;

		private bool autoRefreshEnabled = true;

		private int refreshInterval = 30;

		internal enum ViewModes
		{
			QueueView,
			AllMessageView,
			MessageView
		}
	}
}
