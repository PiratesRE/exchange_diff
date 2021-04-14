using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConfigProviderBase : DisposeTrackableBase, IConfigProvider, IDisposable, IDiagnosable
	{
		protected ConfigProviderBase(IConfigSchema schema)
		{
			this.schema = schema;
			this.configDrivers = new List<IConfigDriver>(5);
		}

		protected static ConfigFlags OverrideFlags
		{
			get
			{
				int value = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Exchange_Test\\v15\\ConfigurationSettings", "ConfigFlags", 0);
				if (Enum.IsDefined(typeof(ConfigFlags), value))
				{
					return (ConfigFlags)value;
				}
				return ConfigFlags.None;
			}
		}

		public DateTime LastUpdated
		{
			get
			{
				return this.configDrivers.Max((IConfigDriver d) => d.LastUpdated);
			}
		}

		public bool IsInitialized
		{
			get
			{
				base.CheckDisposed();
				foreach (IConfigDriver configDriver in this.configDrivers)
				{
					if (!configDriver.IsInitialized)
					{
						return false;
					}
				}
				return true;
			}
		}

		protected IEnumerable<IConfigDriver> ConfigDrivers
		{
			get
			{
				base.CheckDisposed();
				return this.configDrivers;
			}
		}

		public void Initialize()
		{
			base.CheckDisposed();
			foreach (IConfigDriver configDriver in this.configDrivers)
			{
				if (!configDriver.IsInitialized)
				{
					configDriver.Initialize();
				}
			}
		}

		public virtual T GetConfig<T>(string settingName)
		{
			return this.GetConfig<T>(null, settingName);
		}

		public T GetConfig<T>(ISettingsContext context, string settingName)
		{
			base.CheckDisposed();
			return this.GetConfigInternal<T>(context, settingName, new Func<IConfigSchema, string, T>(ConfigSchemaBase.GetDefaultConfig<T>));
		}

		public T GetConfig<T>(ISettingsContext context, string settingName, T defaultValue)
		{
			return this.GetConfigInternal<T>(context, settingName, (IConfigSchema schema, string sName) => defaultValue);
		}

		public bool TryGetConfig<T>(ISettingsContext context, string settingName, out T settingValue)
		{
			base.CheckDisposed();
			object rawValue;
			if (this.TryGetBoxedSettingFromDrivers(context, settingName, typeof(T), out rawValue))
			{
				settingValue = ConfigSchemaBase.ConvertValue<T>(this.schema, settingName, rawValue);
				return true;
			}
			settingValue = default(T);
			return false;
		}

		public virtual string GetDiagnosticComponentName()
		{
			return "config";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			base.CheckDisposed();
			XElement xelement = new XElement(this.GetDiagnosticComponentName(), new XAttribute("name", this.schema.Name));
			xelement.Add(new XAttribute("LastUpdated", this.LastUpdated));
			ConfigDiagnosticArgument configDiagnosticArgument = new ConfigDiagnosticArgument(parameters.Argument);
			SettingsContextBase settingsContextBase = new DiagnosticSettingsContext(this.schema, configDiagnosticArgument);
			xelement.Add(settingsContextBase.GetDiagnosticInfo(parameters.Argument));
			if (configDiagnosticArgument.HasArgument("configname"))
			{
				string argument = configDiagnosticArgument.GetArgument<string>("configname");
				xelement.Add(new XElement("EffectiveSetting", new object[]
				{
					new XAttribute("name", argument),
					new XAttribute("value", this.GetConfig<string>(settingsContextBase, argument))
				}));
			}
			for (int i = 0; i < this.configDrivers.Count; i++)
			{
				IConfigDriver configDriver = this.configDrivers[i];
				xelement.Add(configDriver.GetDiagnosticInfo(parameters.Argument));
			}
			return xelement;
		}

		protected void AddConfigDriver(IConfigDriver configDriver)
		{
			base.CheckDisposed();
			this.configDrivers.Add(configDriver);
		}

		protected void RemoveConfigDriver(IConfigDriver configDriver)
		{
			base.CheckDisposed();
			this.configDrivers.Remove(configDriver);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ConfigProviderBase>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				foreach (IConfigDriver configDriver in this.configDrivers)
				{
					configDriver.Dispose();
				}
				this.configDrivers = null;
			}
		}

		private T GetConfigInternal<T>(ISettingsContext context, string settingName, Func<IConfigSchema, string, T> defaultValueGetter)
		{
			T result;
			if (this.TryGetConfig<T>(context, settingName, out result))
			{
				return result;
			}
			return defaultValueGetter(this.schema, settingName);
		}

		private bool TryGetBoxedSettingFromDrivers(ISettingsContext context, string settingName, Type settingType, out object boxedValue)
		{
			this.Initialize();
			foreach (IConfigDriver configDriver in this.configDrivers)
			{
				if (configDriver.TryGetBoxedSetting(context, settingName, settingType, out boxedValue))
				{
					return true;
				}
			}
			boxedValue = null;
			return false;
		}

		public const string DefaultDiagnosticComponentName = "config";

		public const string RegKeyName = "SOFTWARE\\Microsoft\\Exchange_Test\\v15\\ConfigurationSettings";

		public const string ConfigFlagsRegistryName = "ConfigFlags";

		private List<IConfigDriver> configDrivers;

		private IConfigSchema schema;
	}
}
