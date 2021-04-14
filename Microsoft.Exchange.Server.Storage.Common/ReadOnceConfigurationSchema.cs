using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class ReadOnceConfigurationSchema<TConfig> : ConfigurationSchema<TConfig>
	{
		public ReadOnceConfigurationSchema(string name, TConfig defaultValue) : base(name, defaultValue)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, string registryKey, string registryValue) : base(name, defaultValue, registryKey, registryValue)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse) : base(name, defaultValue, tryParse)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, string registryKey, string registryValue) : base(name, defaultValue, tryParse, registryKey, registryValue)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, Func<TConfig, TConfig> postProcess) : base(name, defaultValue, postProcess)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, Func<TConfig, TConfig> postProcess, string registryKey, string registryValue) : base(name, defaultValue, postProcess, registryKey, registryValue)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, Func<TConfig, TConfig> postProcess, Func<TConfig, bool> validator) : base(name, defaultValue, postProcess, validator)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, Func<TConfig, TConfig> postProcess, Func<TConfig, bool> validator, string registryKey, string registryValue) : base(name, defaultValue, postProcess, validator, registryKey, registryValue)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, Func<TConfig, TConfig> postProcess) : base(name, defaultValue, tryParse, postProcess)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, Func<TConfig, TConfig> postProcess, string registryKey, string registryValue) : base(name, defaultValue, tryParse, postProcess, registryKey, registryValue)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, Func<TConfig, TConfig> postProcess, Func<TConfig, bool> validator) : base(name, defaultValue, tryParse, postProcess, validator)
		{
		}

		public ReadOnceConfigurationSchema(string name, TConfig defaultValue, ConfigurationSchema<TConfig>.TryParse tryParse, Func<TConfig, TConfig> postProcess, Func<TConfig, bool> validator, string registryKey, string registryValue) : base(name, defaultValue, tryParse, postProcess, validator, registryKey, registryValue)
		{
		}

		public override void Reload()
		{
			if (!this.loaded)
			{
				this.loaded = true;
				base.Reload();
			}
		}

		private bool loaded;
	}
}
