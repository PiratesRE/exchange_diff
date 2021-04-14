using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementConsole;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.Services;

namespace Microsoft.Exchange.Management.SnapIn
{
	public abstract class ExchangeScopeNode : ScopeNode
	{
		public ExchangeScopeNode() : this(false)
		{
		}

		public ExchangeScopeNode(bool hideExpandIcon) : base(hideExpandIcon)
		{
			Type type = base.GetType();
			base.LanguageIndependentName = type.Name;
			base.HelpTopic = base.GetType().FullName;
			if (Guid.Empty == base.NodeType)
			{
				throw new ApplicationException(type.FullName + " is missing the NodeTypeAttribute.");
			}
			if (!type.IsSealed)
			{
				throw new ApplicationException(type.FullName + " is a scope node of a non-sealed class.");
			}
			FieldInfo field = type.GetField("NodeGuid");
			if (null == field)
			{
				throw new ApplicationException(type.FullName + " is missing the public const string NodeGuid field.");
			}
			string strB = field.GetValue(null) as string;
			if (string.Compare(base.NodeType.ToString(), strB, true, CultureInfo.InvariantCulture) != 0)
			{
				throw new ApplicationException(type.FullName + " NodeGuid field does not correspond to the NodeType property. Check that the NodeType attribute is using the correct constant.");
			}
			ServiceContainer serviceContainer = new ServiceContainer(this.SnapIn);
			this.progressProvider = new ScopeNodeProgressProvider(this);
			serviceContainer.AddService(typeof(IProgressProvider), this.progressProvider);
			this.components = new ServicedContainer(serviceContainer);
			base.EnabledStandardVerbs |= 64;
		}

		protected void RegisterConnectionToPSServerAction()
		{
			this.propertiesCommand = new Command();
			this.propertiesCommand.Name = "Properties";
			this.propertiesCommand.Icon = Icons.Properties;
			this.propertiesCommand.Text = Strings.ShowPropertiesCommand;
			this.propertiesCommand.Execute += this.connectToServerCommand_Execute;
			this.Commands.Add(this.propertiesCommand);
		}

		public Icon Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("icon");
				}
				if (this.Icon != value)
				{
					this.icon = value;
					base.SelectedImageIndex = (base.ImageIndex = this.SnapIn.RegisterIcon(base.LanguageIndependentName, this.Icon));
					this.OnIconChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnIconChanged(EventArgs e)
		{
			EventHandler iconChanged = this.IconChanged;
			if (iconChanged != null)
			{
				iconChanged(this, e);
			}
		}

		public event EventHandler IconChanged;

		public IExchangeSnapIn SnapIn
		{
			get
			{
				return (IExchangeSnapIn)base.SnapIn;
			}
		}

		public IUIService ShellUI
		{
			get
			{
				return this.SnapIn.ShellUI;
			}
		}

		public IProgressProvider ProgressProvider
		{
			get
			{
				return this.progressProvider;
			}
		}

		public CommandCollection Commands
		{
			get
			{
				if (this.commands == null)
				{
					this.commands = new CommandCollection();
					new CommandsActionsAdapter(this.SnapIn, base.ActionsPaneItems, this.commands, false, this.SnapIn, null);
				}
				return this.commands;
			}
		}

		public virtual void InitializeView(Control control, IProgress status)
		{
		}

		protected sealed override void OnRefresh(AsyncStatus status)
		{
			try
			{
				SynchronizationContext.SetSynchronizationContext(new SynchronizeInvokeSynchronizationContext(base.SnapIn));
				if (this.DataSource != null)
				{
					status.EnableManualCompletion();
					this.DataSource.Refresh(new StatusProgress(status, base.SnapIn));
				}
				if (this.Refreshing != null)
				{
					this.Refreshing(this, EventArgs.Empty);
				}
			}
			catch (Exception ex)
			{
				if (ExceptionHelper.IsUICriticalException(ex))
				{
					throw;
				}
				this.ShellUI.ShowError(ex);
			}
		}

		public event EventHandler Refreshing;

		public IRefreshableNotification DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				if (this.DataSource != value)
				{
					IComponent component = this.DataSource as IComponent;
					if (component != null)
					{
						this.components.Remove(component);
					}
					this.dataSource = value;
					component = (this.DataSource as IComponent);
					if (component != null && component.Site == null)
					{
						this.components.Add(component, this.DataSource.GetHashCode().ToString());
					}
					if (this.DataSource != null)
					{
						base.EnabledStandardVerbs |= 64;
						return;
					}
					base.EnabledStandardVerbs &= -65;
				}
			}
		}

		protected sealed override void OnExpand(AsyncStatus status)
		{
			base.OnExpand(status);
			if (this.IsSnapInRootNode)
			{
				this.InitializeSnapIn();
			}
			this.OnExpand();
		}

		protected virtual void OnExpand()
		{
			if (this.IsSnapInRootNode)
			{
				ExchangeHelpService.Initialize();
			}
		}

		protected IContainer Components
		{
			get
			{
				return this.components;
			}
		}

		private bool IsSnapInRootNode
		{
			get
			{
				return 1 == this.SnapIn.ScopeNodeCollection.Count && this.SnapIn.ScopeNodeCollection[0] == this;
			}
		}

		protected void InitializeSnapIn()
		{
			CmdletAssemblyHelper.LoadingAllCmdletAssembliesAndReference(AppDomain.CurrentDomain.BaseDirectory, new string[0]);
			this.SnapIn.Initialize(this.ProgressProvider);
		}

		public virtual DialogResult ShowDialog(Form form)
		{
			DialogResult result = DialogResult.Cancel;
			try
			{
				this.Components.Add(form, form.Name + form.GetHashCode());
				result = (this.SnapIn as NamespaceSnapInBase).Console.ShowDialog(form);
			}
			finally
			{
				this.Components.Remove(form);
			}
			return result;
		}

		public DialogResult ShowDialog(ExchangePropertyPageControl propertyPage)
		{
			propertyPage.AutoScaleDimensions = ExchangeUserControl.DefaultAutoScaleDimension;
			propertyPage.AutoScaleMode = AutoScaleMode.Font;
			DialogResult result;
			using (PropertyPageDialog propertyPageDialog = new PropertyPageDialog(propertyPage))
			{
				result = this.ShowDialog(propertyPageDialog);
			}
			return result;
		}

		public DialogResult ShowDialog(string caption, string dialogHelpTopic, ExchangePropertyPageControl[] pages)
		{
			foreach (ExchangePropertyPageControl exchangePropertyPageControl in pages)
			{
				exchangePropertyPageControl.AutoScaleDimensions = ExchangeUserControl.DefaultAutoScaleDimension;
				exchangePropertyPageControl.AutoScaleMode = AutoScaleMode.Font;
			}
			DialogResult result;
			using (PropertySheetDialog propertySheetDialog = new PropertySheetDialog(caption, pages))
			{
				propertySheetDialog.HelpTopic = dialogHelpTopic;
				result = this.ShowDialog(propertySheetDialog);
			}
			return result;
		}

		public override string ToString()
		{
			return base.LanguageIndependentName;
		}

		protected override bool OnExpandFromLoad(SyncStatus status)
		{
			return base.OnExpandFromLoad(status);
		}

		private void connectToServerCommand_Execute(object sender, EventArgs e)
		{
			string snapInGuidString = this.SnapIn.SnapInGuidString;
			ConnectionToRemotePSServerControl connectionToRemotePSServerControl = new ConnectionToRemotePSServerControl(this.SnapIn.RootNodeIcon);
			RemotePSDataHandler dataHandler = new RemotePSDataHandler(this.SnapIn.RootNodeDisplayName);
			connectionToRemotePSServerControl.Context = new DataContext(dataHandler);
			using (PropertySheetDialog propertySheetDialog = new PropertySheetDialog(Strings.SingleSelectionProperties(this.SnapIn.RootNodeDisplayName), new ExchangePropertyPageControl[]
			{
				connectionToRemotePSServerControl
			}))
			{
				propertySheetDialog.Name = snapInGuidString;
				propertySheetDialog.HelpTopic = connectionToRemotePSServerControl.HelpTopic;
				this.ShowDialog(propertySheetDialog);
			}
		}

		private ServicedContainer components;

		private IProgressProvider progressProvider;

		private IRefreshableNotification dataSource;

		private CommandCollection commands;

		private Command propertiesCommand;

		private Icon icon;
	}
}
