using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangeConfigurationSection : ConfigurationSection
	{
		public ExchangeConfigurationSection()
		{
			ConfigurationPropertyCollection properties = this.Properties;
			this.Definitions = new Dictionary<string, ConfigurationProperty>(properties.Count, StringComparer.InvariantCultureIgnoreCase);
			foreach (object obj in properties)
			{
				ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
				this.Definitions[configurationProperty.Name] = configurationProperty;
			}
		}

		public IEnumerable<string> Settings
		{
			get
			{
				return this.Definitions.Keys;
			}
		}

		private protected Dictionary<string, ConfigurationProperty> Definitions { protected get; private set; }

		public bool CheckSettingExists(string name)
		{
			ConfigurationProperty configurationProperty;
			return this.Definitions.TryGetValue(name, out configurationProperty);
		}

		public bool TryGetConfigurationProperty(string name, out ConfigurationProperty property)
		{
			return this.Definitions.TryGetValue(name, out property);
		}

		public ConfigurationProperty GetConfigurationProperty(string name, Type type = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			ConfigurationProperty configurationProperty;
			if (!this.TryGetConfigurationProperty(name, out configurationProperty))
			{
				List<string> list = new List<string>(this.Settings);
				list.Sort();
				throw new ConfigurationSettingsPropertyNotFoundException(name, string.Join(", ", list));
			}
			if (type != null && configurationProperty.Type != type && !configurationProperty.Converter.CanConvertTo(type))
			{
				throw new ConfigurationSettingsPropertyBadTypeException(name, type.ToString());
			}
			return configurationProperty;
		}

		public object GetPropertyValue(string propertyName)
		{
			return base[propertyName];
		}

		public static void RunConfigOperation(Action configOperation, Func<Exception, ConfigurationSettingsException> errorHandler)
		{
			ExchangeConfigurationSection.InternalRunConfigOperation(configOperation, delegate(Exception ex)
			{
				throw errorHandler(ex);
			});
		}

		public static bool TryConvertFromInvariantString(ConfigurationProperty configProperty, string toConvert, out object converted)
		{
			object convertedValue = null;
			ExchangeConfigurationSection.InternalRunConfigOperation(delegate
			{
				convertedValue = configProperty.Converter.ConvertFromInvariantString(toConvert);
			}, delegate(Exception ex)
			{
				convertedValue = null;
			});
			converted = convertedValue;
			return converted != null;
		}

		protected static void InternalRunConfigOperation(Action configOperation, Action<Exception> errorHandler)
		{
			try
			{
				configOperation();
			}
			catch (ArgumentException obj)
			{
				errorHandler(obj);
			}
			catch (FormatException obj2)
			{
				errorHandler(obj2);
			}
			catch (NotSupportedException obj3)
			{
				errorHandler(obj3);
			}
			catch (Exception ex)
			{
				if (ex.InnerException == null || !(ex.InnerException is FormatException))
				{
					throw;
				}
				errorHandler(ex);
			}
		}

		protected virtual T InternalGetConfig<T>([CallerMemberName] string key = null)
		{
			return (T)((object)base[key]);
		}

		protected virtual void InternalSetConfig<T>(T value, [CallerMemberName] string key = null)
		{
			base[key] = value;
		}
	}
}
