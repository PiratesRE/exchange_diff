using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class ConfigurationSchema<TConfig> : ConfigurationSchema
	{
		public ConfigurationSchema(string name) : base(name)
		{
			this.defaultValueHook = Hookable<TConfig>.Create<ConfigurationSchema<TConfig>>(true, "defaultValue", this);
			this.valueHook = Hookable<TConfig>.Create<ConfigurationSchema<TConfig>>(true, "value", this);
		}

		public ConfigurationSchema(string name, TConfig defaultValue) : this(name, defaultValue, null, null)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, string registryKey, string registryValue) : this(name, defaultValue, null, registryKey, registryValue)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse) : this(name, defaultValue, tryParse, null, null)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, string registryKey, string registryValue) : this(name, defaultValue, tryParse, null, registryKey, registryValue)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, Func<TConfig, TConfig> postProcess) : this(name, defaultValue, postProcess, null, null)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, Func<TConfig, TConfig> postProcess, string registryKey, string registryValue) : this(name, defaultValue, postProcess, null, registryKey, registryValue)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, Func<TConfig, TConfig> postProcess, Func<TConfig, bool> validator) : this(name, defaultValue, postProcess, validator, null, null)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, Func<TConfig, TConfig> postProcess, Func<TConfig, bool> validator, string registryKey, string registryValue) : this(name, defaultValue, postProcess, validator, TypeDescriptor.GetConverter(typeof(TConfig)), registryKey, registryValue)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, Func<TConfig, TConfig> postProcess) : this(name, defaultValue, tryParse, postProcess, null, null)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, Func<TConfig, TConfig> postProcess, string registryKey, string registryValue) : this(name, defaultValue, tryParse, postProcess, null, registryKey, registryValue)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, Func<TConfig, TConfig> postProcess, Func<TConfig, bool> validator) : this(name, defaultValue, tryParse, postProcess, validator, null, null)
		{
		}

		public ConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, Func<TConfig, TConfig> postProcess, Func<TConfig, bool> validator, string registryKey, string registryValue) : this(name, defaultValue, postProcess, validator, new ConfigurationSchema<TConfig>.GenericConverter(tryParse), registryKey, registryValue)
		{
		}

		private ConfigurationSchema(string name, TConfig defaultValue, Func<TConfig, TConfig> postProcess, Func<TConfig, bool> validator, TypeConverter typeConverter, string registryKey, string registryValue) : this(name)
		{
			this.defaultValue = defaultValue;
			this.postProcess = postProcess;
			this.typeConverter = typeConverter;
			this.validator = new ConfigurationSchema<TConfig>.GenericValidator(validator);
			this.registrySubkey = registryKey;
			this.registryValueName = registryValue;
		}

		public TConfig DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		public TConfig Value
		{
			get
			{
				return this.value;
			}
			private set
			{
				this.value = value;
			}
		}

		public string RegistryValueName
		{
			get
			{
				return this.registryValueName;
			}
		}

		internal override ConfigurationProperty ConfigurationProperty
		{
			get
			{
				return new ConfigurationProperty(base.Name, typeof(TConfig), this.defaultValue, this.typeConverter, this.validator, ConfigurationPropertyOptions.None, string.Empty);
			}
		}

		public TConfig GetConfig(string settingName, TConfig defaultValue)
		{
			if (this.registrySubkey != null)
			{
				string localDatabaseRegistryKey = this.registrySubkey;
				if (localDatabaseRegistryKey == "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\{1}-{2}")
				{
					localDatabaseRegistryKey = ConfigurationSchema.LocalDatabaseRegistryKey;
				}
				if (RegistryReader.Instance.DoesValueExist(Registry.LocalMachine, localDatabaseRegistryKey, this.registryValueName))
				{
					TConfig tconfig = RegistryReader.Instance.GetValue<TConfig>(Registry.LocalMachine, localDatabaseRegistryKey, this.registryValueName, defaultValue);
					string text = tconfig.ToString();
					TConfig tconfig2 = (TConfig)((object)this.typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, text));
					if (this.validator != null)
					{
						this.validator.Validate(tconfig2);
					}
					return tconfig2;
				}
			}
			TConfig result;
			try
			{
				result = StoreConfigContext.Default.GetConfig<TConfig>(settingName, defaultValue);
			}
			catch (ConfigurationSettingsException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_InvalidAppConfig, new object[]
				{
					settingName,
					ex,
					defaultValue
				});
				result = defaultValue;
			}
			return result;
		}

		public override void Reload()
		{
			this.Value = this.GetConfig(base.Name, this.defaultValue);
			if (this.postProcess != null)
			{
				this.Value = this.postProcess(this.Value);
			}
		}

		internal IDisposable SetDefaultValueHook(TConfig value)
		{
			DisposeGuard disposeGuard = default(DisposeGuard);
			disposeGuard.Add<IDisposable>(this.defaultValueHook.SetTestHook(value));
			disposeGuard.Add<IDisposable>(this.SetValueHook(value));
			return disposeGuard;
		}

		internal IDisposable SetValueHook(TConfig value)
		{
			return this.valueHook.SetTestHook(value);
		}

		private readonly Hookable<TConfig> defaultValueHook;

		private readonly Hookable<TConfig> valueHook;

		private readonly TConfig defaultValue;

		private readonly Func<TConfig, TConfig> postProcess;

		private readonly TypeConverter typeConverter;

		private readonly ConfigurationSchema<TConfig>.GenericValidator validator;

		private readonly string registrySubkey;

		private readonly string registryValueName;

		private TConfig value;

		public delegate bool TryParse(string data, out TConfig value);

		private class GenericValidator : ConfigurationValidatorBase
		{
			public GenericValidator(Func<TConfig, bool> validator)
			{
				this.validator = validator;
			}

			public override bool CanValidate(Type type)
			{
				return type == typeof(TConfig);
			}

			public override void Validate(object o)
			{
				if (this.validator != null && !this.validator((TConfig)((object)o)))
				{
					throw new ArgumentException(o.ToString());
				}
			}

			private readonly Func<TConfig, bool> validator;
		}

		private class GenericConverter : TypeConverter
		{
			public GenericConverter(ConfigurationSchema<TConfig>.TryParse tryParse)
			{
				this.tryParse = tryParse;
			}

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				TConfig tconfig;
				if (!this.tryParse((string)value, out tconfig))
				{
					throw new NotSupportedException();
				}
				return tconfig;
			}

			private readonly ConfigurationSchema<TConfig>.TryParse tryParse;
		}
	}
}
