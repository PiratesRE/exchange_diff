using System;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SnapIn
{
	[SettingsProvider(typeof(ExchangeSettingsProvider))]
	public class ExchangeADServerSettings : ExchangeSettings
	{
		public ExchangeADServerSettings(IComponent owner) : base(owner)
		{
		}

		[DefaultSettingValue("false")]
		public bool ForestViewEnabled
		{
			get
			{
				return this.ADServerSettings == null || this.ADServerSettings.ViewEntireForest;
			}
			set
			{
				if (this.ADServerSettings == null)
				{
					throw new NotSupportedException();
				}
				if (this.ForestViewEnabled != value)
				{
					this.ADServerSettings.ViewEntireForest = value;
					this.OnPropertyChanged(this, new PropertyChangedEventArgs("ForestViewEnabled"));
				}
			}
		}

		[DefaultSettingValue("")]
		public string OrganizationalUnit
		{
			get
			{
				if (this.ADServerSettings == null)
				{
					return null;
				}
				return this.ADServerSettings.RecipientViewRoot;
			}
			set
			{
				if (this.ADServerSettings == null)
				{
					throw new NotSupportedException();
				}
				if (string.Compare(this.OrganizationalUnit, value, StringComparison.OrdinalIgnoreCase) != 0)
				{
					this.ADServerSettings.RecipientViewRoot = value;
					this.OnPropertyChanged(this, new PropertyChangedEventArgs("OrganizationalUnit"));
				}
			}
		}

		[DefaultSettingValue("")]
		public Fqdn DomainController
		{
			get
			{
				if (this.ADServerSettings == null)
				{
					return null;
				}
				if (this.ADServerSettings.UserPreferredDomainControllers != null && this.ADServerSettings.UserPreferredDomainControllers.Count != 0)
				{
					return this.ADServerSettings.UserPreferredDomainControllers[0];
				}
				return null;
			}
			set
			{
				if (this.ADServerSettings == null)
				{
					throw new NotSupportedException();
				}
				if (!object.Equals(this.DomainController, value))
				{
					this.ADServerSettings.UserPreferredDomainControllers = new MultiValuedProperty<Fqdn>(value);
					this.OnPropertyChanged(this, new PropertyChangedEventArgs("DomainController"));
				}
			}
		}

		[DefaultSettingValue("")]
		public Fqdn GlobalCatalog
		{
			get
			{
				if (this.ADServerSettings == null)
				{
					return null;
				}
				return this.ADServerSettings.UserPreferredGlobalCatalog;
			}
			set
			{
				if (this.ADServerSettings == null)
				{
					throw new NotSupportedException();
				}
				if (!object.Equals(this.GlobalCatalog, value))
				{
					this.ADServerSettings.UserPreferredGlobalCatalog = value;
					this.OnPropertyChanged(this, new PropertyChangedEventArgs("GlobalCatalog"));
				}
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("")]
		public ADObjectId ConfigDomain
		{
			get
			{
				return ((ADObjectId)this["ConfigDomain"]) ?? new ADObjectId();
			}
			set
			{
				this["ConfigDomain"] = value;
			}
		}

		[DefaultSettingValue("")]
		public Fqdn ConfigurationDomainController
		{
			get
			{
				if (this.ADServerSettings == null)
				{
					return null;
				}
				return this.ADServerSettings.UserPreferredConfigurationDomainController;
			}
			set
			{
				if (this.ADServerSettings == null)
				{
					throw new NotSupportedException();
				}
				if (!object.Equals(this.ConfigurationDomainController, value))
				{
					this.ADServerSettings.UserPreferredConfigurationDomainController = value;
					this.OnPropertyChanged(this, new PropertyChangedEventArgs("ConfigurationDomainController"));
				}
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("")]
		public RunspaceServerSettingsPresentationObject ADServerSettings
		{
			get
			{
				this.EnsureADSettingsEnforced();
				return (RunspaceServerSettingsPresentationObject)this["ADServerSettings"];
			}
			set
			{
				this["ADServerSettings"] = value;
			}
		}

		private void EnsureADSettingsEnforced()
		{
			if (this["ADServerSettings"] == null && !EnvironmentAnalyzer.IsWorkGroup())
			{
				this.waitHandle.WaitOne();
			}
		}

		internal void EnforceADSettings()
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeADServerSettings>(0L, "-->ExchangeSystemManagerSettings.EnforceAdSettings: {0}", this);
			if (this["ADServerSettings"] == null && !EnvironmentAnalyzer.IsWorkGroup() && OrganizationType.Cloud != PSConnectionInfoSingleton.GetInstance().Type)
			{
				try
				{
					try
					{
						using (MonadConnection monadConnection = new MonadConnection("timeout=30", new CommandInteractionHandler(), null, PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo()))
						{
							monadConnection.Open();
							LoggableMonadCommand loggableMonadCommand = new LoggableMonadCommand("Get-ADServerSettingsForLogonUser", monadConnection);
							object[] array = loggableMonadCommand.Execute();
							if (array != null && array.Length > 0)
							{
								RunspaceServerSettingsPresentationObject runspaceServerSettingsPresentationObject = array[0] as RunspaceServerSettingsPresentationObject;
								this.ADServerSettings = runspaceServerSettingsPresentationObject;
								this.OrganizationalUnit = runspaceServerSettingsPresentationObject.RecipientViewRoot;
								this.ForestViewEnabled = runspaceServerSettingsPresentationObject.ViewEntireForest;
								this.GlobalCatalog = runspaceServerSettingsPresentationObject.UserPreferredGlobalCatalog;
								this.ConfigurationDomainController = runspaceServerSettingsPresentationObject.UserPreferredConfigurationDomainController;
								if (runspaceServerSettingsPresentationObject.UserPreferredDomainControllers != null && runspaceServerSettingsPresentationObject.UserPreferredDomainControllers.Count != 0)
								{
									this.DomainController = runspaceServerSettingsPresentationObject.UserPreferredDomainControllers[0];
								}
							}
							else
							{
								this.SetDefaultSettings();
							}
						}
					}
					catch (Exception)
					{
						this.SetDefaultSettings();
					}
					goto IL_11A;
				}
				finally
				{
					this.waitHandle.Set();
				}
			}
			this.waitHandle.Set();
			IL_11A:
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeADServerSettings>(0L, "<--ExchangeSystemManagerSettings.EnforceAdSettings: {0}", this);
		}

		internal void SetDefaultSettings()
		{
			base.DoBeginInit();
			this.ADServerSettings = new RunspaceServerSettingsPresentationObject();
			this.ForestViewEnabled = true;
			base.DoEndInit(false);
		}

		public RunspaceServerSettingsPresentationObject CreateRunspaceServerSettingsObject()
		{
			if (EnvironmentAnalyzer.IsWorkGroup() || OrganizationType.Cloud == PSConnectionInfoSingleton.GetInstance().Type || this.ADServerSettings == null)
			{
				return null;
			}
			if (this.ADServerSettings != null)
			{
				lock (this.syncRoot)
				{
					return this.ADServerSettings.Clone() as RunspaceServerSettingsPresentationObject;
				}
			}
			return null;
		}

		public const string ForestViewEnabledString = "ForestViewEnabled";

		public const string OrganizationalUnitString = "OrganizationalUnit";

		public const string DomainControllerString = "DomainController";

		public const string GlobalCatalogString = "GlobalCatalog";

		public const string ConfigDomainString = "ConfigDomain";

		public const string ConfigurationDomainControllerString = "ConfigurationDomainController";

		private object syncRoot = new object();

		private ManualResetEvent waitHandle = new ManualResetEvent(false);
	}
}
