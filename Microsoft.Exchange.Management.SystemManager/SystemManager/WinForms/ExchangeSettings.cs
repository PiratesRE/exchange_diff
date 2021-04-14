using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[SettingsProvider(typeof(ExchangeSettingsProvider))]
	public partial class ExchangeSettings : ApplicationSettingsBase
	{
		public ExchangeSettings(IComponent owner) : base(owner)
		{
		}

		public void DoBeginInit()
		{
			this.OnBeginInit(EventArgs.Empty);
		}

		protected virtual void OnBeginInit(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)this.Events[ExchangeSettings.EventBeginInit];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler BeginInit
		{
			add
			{
				this.Events.AddHandler(ExchangeSettings.EventBeginInit, value);
			}
			remove
			{
				this.Events.RemoveHandler(ExchangeSettings.EventBeginInit, value);
			}
		}

		public void DoEndInit(bool cancelAutoRefresh)
		{
			this.OnEndInit(new CancelEventArgs(cancelAutoRefresh));
		}

		protected virtual void OnEndInit(CancelEventArgs e)
		{
			CancelEventHandler cancelEventHandler = (CancelEventHandler)this.Events[ExchangeSettings.EventEndInit];
			if (cancelEventHandler != null)
			{
				cancelEventHandler(this, e);
			}
		}

		public event CancelEventHandler EndInit
		{
			add
			{
				this.Events.AddHandler(ExchangeSettings.EventEndInit, value);
			}
			remove
			{
				this.Events.RemoveHandler(ExchangeSettings.EventEndInit, value);
			}
		}

		protected EventHandlerList Events
		{
			get
			{
				return this.events;
			}
		}

		public void UpdateProviders(ISettingsProviderService provSvc)
		{
			if (provSvc != null)
			{
				foreach (object obj in this.Properties)
				{
					SettingsProperty settingsProperty = (SettingsProperty)obj;
					SettingsProvider settingsProvider = provSvc.GetSettingsProvider(settingsProperty);
					if (settingsProvider != null)
					{
						settingsProperty.Provider = settingsProvider;
					}
				}
				this.Providers.Clear();
				foreach (object obj2 in this.Properties)
				{
					SettingsProperty settingsProperty2 = (SettingsProperty)obj2;
					if (this.Providers[settingsProperty2.Provider.Name] == null)
					{
						this.Providers.Add(settingsProperty2.Provider);
					}
				}
			}
		}

		public string InstanceDisplayName { get; set; }

		public event CustomDataRefreshEventHandler RefreshResultPane
		{
			add
			{
				SynchronizedDelegate.Combine(this.Events, ExchangeSettings.EventRefreshResultPane, value);
			}
			remove
			{
				SynchronizedDelegate.Remove(this.Events, ExchangeSettings.EventRefreshResultPane, value);
			}
		}

		public void RaiseRefreshResultPane(CustomDataRefreshEventArgs e)
		{
			EventHandler eventHandler = (EventHandler)this.Events[ExchangeSettings.EventRefreshResultPane];
			if (eventHandler != null)
			{
				eventHandler.DynamicInvoke(new object[]
				{
					this,
					e
				});
			}
		}

		[UserScopedSetting]
		public bool IsCommandLoggingEnabled
		{
			get
			{
				return this[ExchangeSettings.isCommandLoggingEnabled] == null || (bool)this[ExchangeSettings.isCommandLoggingEnabled];
			}
			set
			{
				if (!value.Equals(this[ExchangeSettings.isCommandLoggingEnabled]))
				{
					this[ExchangeSettings.isCommandLoggingEnabled] = value;
				}
			}
		}

		[UserScopedSetting]
		public int MaximumRecordCount
		{
			get
			{
				if (this[ExchangeSettings.maximumRecordCount] == null)
				{
					return CommandLoggingSession.DefaultMaximumRecordCount;
				}
				return (int)this[ExchangeSettings.maximumRecordCount];
			}
			set
			{
				if (!value.Equals(this[ExchangeSettings.maximumRecordCount]))
				{
					this[ExchangeSettings.maximumRecordCount] = value;
				}
			}
		}

		[UserScopedSetting]
		public Point CommandLoggingDialogLocation
		{
			get
			{
				Point point = (this[ExchangeSettings.commandLoggingDialogLocation] != null) ? ((Point)this[ExchangeSettings.commandLoggingDialogLocation]) : CommandLoggingDialog.DefaultDialogLocation;
				if (!Screen.PrimaryScreen.WorkingArea.Contains(point))
				{
					point = new Point(0, 0);
				}
				return point;
			}
			set
			{
				if (!object.Equals(value, this[ExchangeSettings.commandLoggingDialogLocation]))
				{
					this[ExchangeSettings.commandLoggingDialogLocation] = value;
				}
			}
		}

		[UserScopedSetting]
		public Size CommandLoggingDialogSize
		{
			get
			{
				Size size = (this[ExchangeSettings.commandLoggingDialogSize] != null) ? ((Size)this[ExchangeSettings.commandLoggingDialogSize]) : CommandLoggingDialog.DefaultDialogSize;
				int width = Screen.PrimaryScreen.WorkingArea.Width;
				int height = Screen.PrimaryScreen.WorkingArea.Height;
				return new Size((size.Width > width) ? width : size.Width, (size.Height > height) ? height : size.Height);
			}
			set
			{
				if (!object.Equals(value, this[ExchangeSettings.commandLoggingDialogSize]))
				{
					this[ExchangeSettings.commandLoggingDialogSize] = value;
				}
			}
		}

		[UserScopedSetting]
		public float CommandLoggingDialogSplitterDistanceScale
		{
			get
			{
				if (this[ExchangeSettings.commandLoggingDialogSplitterDistanceScale] == null)
				{
					return CommandLoggingDialog.DefaultSplitterDistanceScale;
				}
				return (float)this[ExchangeSettings.commandLoggingDialogSplitterDistanceScale];
			}
			set
			{
				if (value != this.CommandLoggingDialogSplitterDistanceScale)
				{
					this[ExchangeSettings.commandLoggingDialogSplitterDistanceScale] = value;
				}
			}
		}

		private static readonly object EventBeginInit = new object();

		private static readonly object EventEndInit = new object();

		private EventHandlerList events = new EventHandlerList();

		private static readonly object EventRefreshResultPane = new object();

		private static string isCommandLoggingEnabled = "IsCommandLoggingEnabled";

		private static string maximumRecordCount = "MaximumRecordCount";

		private static string commandLoggingDialogLocation = "CommandLoggingDialogLocation";

		private static string commandLoggingDialogSize = "CommandLoggingDialogSize";

		private static string commandLoggingDialogSplitterDistanceScale = "CommandLoggingDialogSplitterDistanceScale";
	}
}
