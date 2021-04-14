using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web.Configuration;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class ConfigFileHandler : IDisposable
	{
		public FileHandler FileHandler
		{
			get
			{
				return this.fileHandler;
			}
		}

		public string ConfigFilePath
		{
			get
			{
				return this.configFilePath;
			}
		}

		public ConfigFileHandler(string key, string defaultFileName)
		{
			this.key = key;
			this.defaultFileName = defaultFileName;
			this.configFilePath = this.GetFilePath();
			this.fileHandler = new FileHandler(this.configFilePath);
		}

		public void Dispose()
		{
			this.fileHandler.Dispose();
		}

		internal void SetConfigSource(string configSource, string siteName)
		{
			this.configSource = configSource;
			this.siteName = siteName;
			this.UpdateConfigFilePath();
		}

		internal void UpdateConfigFilePath()
		{
			string filePath = this.GetFilePath();
			if (!StringComparer.OrdinalIgnoreCase.Equals(this.configFilePath, filePath))
			{
				this.configFilePath = filePath;
				this.fileHandler.ChangeFile(filePath);
			}
		}

		private string GetFilePath()
		{
			string text = null;
			for (int i = 0; i < 10; i++)
			{
				try
				{
					text = this.ReadKeyFromConfig(this.configSource, this.key);
					break;
				}
				catch (ConfigurationErrorsException ex)
				{
					InternalBypassTrace.TracingConfigurationTracer.TraceError(0, 0L, "Configsource: {0}, had ConfigurationErrorsException, will retry in 500ms. Exception: {1}", new object[]
					{
						this.configSource,
						ex
					});
					Thread.Sleep(500);
				}
			}
			if (text == null)
			{
				text = ConfigFiles.GetConfigFilePath(this.defaultFileName);
				InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, 0L, "Using default file path: {0}", new object[]
				{
					text
				});
			}
			else
			{
				InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, 0L, "Appconfig redirection, using file: {0}", new object[]
				{
					text
				});
			}
			return text;
		}

		private string ReadKeyFromConfig(string configSource, string fileKey)
		{
			if (string.IsNullOrEmpty(configSource))
			{
				ConfigurationManager.RefreshSection("appSettings");
				return ConfigurationManager.AppSettings[fileKey];
			}
			Configuration configuration;
			try
			{
				configuration = this.LoadWebConfiguration(configSource);
			}
			catch (Exception)
			{
				return null;
			}
			AppDomain.CurrentDomain.SetupInformation.ConfigurationFile = configuration.FilePath;
			KeyValueConfigurationElement keyValueConfigurationElement = configuration.AppSettings.Settings[fileKey];
			if (keyValueConfigurationElement != null)
			{
				return keyValueConfigurationElement.Value;
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private Configuration LoadWebConfiguration(string configSource)
		{
			if (string.IsNullOrEmpty(this.siteName))
			{
				return WebConfigurationManager.OpenWebConfiguration(configSource);
			}
			return WebConfigurationManager.OpenWebConfiguration(configSource, this.siteName);
		}

		private const string AppSettingsSection = "appSettings";

		private string key;

		private string defaultFileName;

		private string configFilePath;

		private FileHandler fileHandler;

		private string configSource;

		private string siteName;
	}
}
