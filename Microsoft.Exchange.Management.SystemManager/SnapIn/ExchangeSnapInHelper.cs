using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementConsole;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Services;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class ExchangeSnapInHelper : IDisposable
	{
		public ExchangeSnapInHelper(NamespaceSnapInBase snapIn, IExchangeSnapIn exchangeSnapIn)
		{
			this.snapIn = snapIn;
			this.exchangeSnapIn = exchangeSnapIn;
			Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
			Application.EnableVisualStyles();
			this.snapInName = this.snapIn.GetType().Name;
			try
			{
				Thread.CurrentThread.Name = this.snapInName;
			}
			catch (InvalidOperationException)
			{
			}
			SynchronizationContext synchronizationContext = new SynchronizeInvokeSynchronizationContext(this.snapIn);
			SynchronizationContext.SetSynchronizationContext(synchronizationContext);
			ManagementGUICommon.RegisterAssembly(0, "Microsoft.Exchange.ManagementGUI, Version=15.00.0000.000, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "ObjectPickerSchema.xml");
			ManagementGUICommon.RegisterAssembly(1, "Microsoft.Exchange.ManagementGUI, Version=15.00.0000.000, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "ResultPaneSchema.xml");
			ManagementGUICommon.RegisterAssembly(2, "Microsoft.Exchange.ManagementGUI, Version=15.00.0000.000, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "StrongTypeEditorSchema.xml");
			Assembly assembly = Assembly.Load("Microsoft.Exchange.ManagementGUI, Version=15.00.0000.000, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			ManagementGUICommon.RegisterResourcesAssembly(ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.ManagementGUI.Resources.Strings", assembly), ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.ManagementGUI.Resources.Icons", assembly));
			this.uiService = new UIService(null);
			this.settingsProvider = new ExchangeSettingsProvider();
			this.services = new ServiceContainer();
			this.services.AddService(typeof(IUIService), this.uiService);
			this.services.AddService(typeof(SynchronizationContext), synchronizationContext);
			this.services.AddService(typeof(ISettingsProviderService), this.settingsProvider);
			this.components = new ServicedContainer(this.services);
			this.LoadTestStub();
		}

		public void InitializeSettingProvider()
		{
			this.settingsProvider.Initialize(null, null);
		}

		void IDisposable.Dispose()
		{
			this.components.Dispose();
		}

		internal ServiceContainer Services
		{
			get
			{
				return this.services;
			}
		}

		internal void OnInitialize()
		{
			this.services.RemoveService(typeof(IUIService));
			this.uiService = new SnapInUIService(this.snapIn);
			this.services.AddService(typeof(IUIService), this.uiService);
		}

		public void Initialize(IProgressProvider progressProvider)
		{
			CommandLoggingSession.GetInstance().CommandLoggingEnabled = this.Settings.IsCommandLoggingEnabled;
			CommandLoggingSession.GetInstance().MaximumRecordCount = this.Settings.MaximumRecordCount;
			CommandLoggingDialog.GlobalSettings = this.Settings;
			if (this.Settings.IsCommandLoggingEnabled)
			{
				CommandLoggingDialog.StartDateTime = ((DateTime)ExDateTime.Now).ToString();
			}
			SnapInCallbackService.RegisterSnapInHelpTopicCallback(this.snapIn, new SnapInCallbackService.SnapInHelpTopicCallback(this.HelpCallBack));
		}

		private void HelpCallBack(object o)
		{
			ScopeNode scopeNode = o as ScopeNode;
			string helpTopic;
			if (scopeNode != null)
			{
				helpTopic = scopeNode.HelpTopic;
			}
			else
			{
				helpTopic = (o as SelectionData).HelpTopic;
			}
			ExchangeHelpService.ShowHelpFromHelpTopicId(helpTopic);
		}

		internal void Shutdown(AsyncStatus status)
		{
			MonadRemoteRunspaceFactory.ClearAppDomainRemoteRunspaceConnections();
			CommandLoggingDialog.CloseCommandLoggingDialg();
			if (this.settings != null)
			{
				this.settings.PropertyChanged -= this.Settings_PropertyChanged;
			}
			((IDisposable)this).Dispose();
		}

		public IContainer Components
		{
			get
			{
				return this.components;
			}
		}

		public IUIService ShellUI
		{
			get
			{
				return this.uiService;
			}
		}

		public DialogResult ShowDialog(CommonDialog dialog)
		{
			if (this.uiService is SnapInUIService)
			{
				return this.snapIn.Console.ShowDialog(dialog);
			}
			return dialog.ShowDialog(this.uiService.GetDialogOwnerWindow());
		}

		public int RegisterIcon(string name, Icon icon)
		{
			int num = -1;
			if (icon != null && !string.IsNullOrEmpty(name) && !this.imageListMap.TryGetValue(name, out num))
			{
				Bitmap bitmap = IconLibrary.ToBitmap(icon, SystemInformation.SmallIconSize);
				this.snapIn.SmallImages.Add(bitmap);
				num = this.snapIn.SmallImages.Count - 1;
				this.imageListMap[name] = num;
			}
			return num;
		}

		public string InstanceDisplayName
		{
			get
			{
				Type type = this.snapIn.GetType();
				object[] customAttributes = type.GetCustomAttributes(typeof(SnapInAboutAttribute), false);
				SnapInAboutAttribute snapInAboutAttribute = customAttributes[0] as SnapInAboutAttribute;
				return WinformsHelper.GetDllResourceString(snapInAboutAttribute.ResourceModule, snapInAboutAttribute.DisplayNameId);
			}
		}

		public ExchangeSettings Settings
		{
			get
			{
				if (this.settings == null)
				{
					this.settings = this.exchangeSnapIn.CreateSettings(this.components);
					this.settings.UpdateProviders(this.settingsProvider);
					this.settings.PropertyChanged += this.Settings_PropertyChanged;
					this.settings.SettingsLoaded += this.Settings_SettingsLoaded;
					this.settings.InstanceDisplayName = this.InstanceDisplayName;
				}
				return this.settings;
			}
		}

		private void Settings_SettingsLoaded(object sender, SettingsLoadedEventArgs e)
		{
			this.snapIn.IsModified = false;
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.snapIn.IsModified = true;
		}

		internal ExchangeSettings CreateSettings(IComponent owner)
		{
			return SettingsBase.Synchronized(new ExchangeSettings(owner)) as ExchangeSettings;
		}

		internal void OnLoadCustomData(AsyncStatus status, byte[] customData)
		{
			try
			{
				this.settingsProvider.ByteData = customData;
			}
			catch (SerializationException)
			{
				this.uiService.ShowMessage(Strings.CannotLoadSettings);
				this.settingsProvider.ByteData = null;
			}
			this.Settings.Reload();
		}

		internal byte[] OnSaveCustomData(SyncStatus status)
		{
			byte[] result = null;
			this.Settings.Save();
			try
			{
				result = this.settingsProvider.ByteData;
			}
			catch (Exception innerException)
			{
				throw new LocalizedException(Strings.CannotSaveSettings, innerException);
			}
			return result;
		}

		private void LoadTestStub()
		{
			string text = string.Empty;
			string text2 = string.Empty;
			foreach (string text3 in Environment.GetCommandLineArgs())
			{
				if (text3.StartsWith("/TestStub:", StringComparison.OrdinalIgnoreCase))
				{
					Match match = Regex.Match(text3, ":([^,]+),([^,]+)$");
					if (!match.Success)
					{
						this.uiService.ShowError(Strings.InvalidTestStubArguments);
						break;
					}
					text = match.Groups[1].Value;
					text2 = match.Groups[2].Value;
				}
			}
			Assembly assembly = null;
			ITestStub testStub = null;
			if (text != string.Empty)
			{
				try
				{
					assembly = Assembly.LoadFrom(text);
				}
				catch (FileLoadException ex)
				{
					this.uiService.ShowError(Strings.InvalidTestStubAssembly(ex.Message));
				}
			}
			if (assembly != null)
			{
				Type[] exportedTypes = assembly.GetExportedTypes();
				int j = 0;
				while (j < exportedTypes.Length)
				{
					Type type = exportedTypes[j];
					if (string.Compare(type.ToString(), text2, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (type.IsSubclassOf(typeof(ITestStub)))
						{
							this.uiService.ShowError(Strings.TestStubNotITestStub);
							break;
						}
						testStub = (assembly.CreateInstance(text2) as ITestStub);
						testStub.InstallStub(this.snapIn);
						break;
					}
					else
					{
						j++;
					}
				}
			}
			if (text != string.Empty && testStub == null)
			{
				this.uiService.ShowError(Strings.TestStubGenericError);
			}
		}

		public override string ToString()
		{
			return this.snapInName;
		}

		private NamespaceSnapInBase snapIn;

		private IExchangeSnapIn exchangeSnapIn;

		private string snapInName;

		private IUIService uiService;

		private ServiceContainer services;

		private ServicedContainer components;

		private ExchangeSettingsProvider settingsProvider;

		private ExchangeSettings settings;

		private Dictionary<string, int> imageListMap = new Dictionary<string, int>();
	}
}
